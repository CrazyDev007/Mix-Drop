using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UI.Lobby;
using Debug = UnityEngine.Debug;

[DisallowMultipleComponent]
[DefaultExecutionOrder(-250)]
public class LobbyOverlayManager : MonoBehaviour
{
    /// <summary>
    /// Singleton-style accessor for the active <see cref="LobbyOverlayManager"/>.
    /// </summary>
    public static LobbyOverlayManager Instance { get; private set; }

    /// <summary>
    /// Raised when an overlay reports that it has fully opened (target ≤ 500ms per PRF-001).
    /// </summary>
    public event Action<string> OverlayOpened;

    /// <summary>
    /// Raised when an overlay reports it has fully closed (target ≤ 500ms per PRF-001).
    /// </summary>
    public event Action<string> OverlayClosed;

    /// <summary>
    /// Raised whenever instrumentation captures an overlay lifecycle duration, enabling tests and analytics hooks.
    /// </summary>
    public event Action<OverlayInstrumentationSample> OnInstrumentationSampled;

    private readonly Dictionary<string, ILobbyOverlay> overlays = new Dictionary<string, ILobbyOverlay>();
    private readonly List<OverlayInstrumentationSample> instrumentationSamples = new List<OverlayInstrumentationSample>();

    private string activeOverlayId;
    private string pendingOverlayId;
    private InstrumentationSession activeInstrumentation;

    /// <summary>
    /// Gets the identifier of the currently visible overlay, or <c>null</c> if none.
    /// </summary>
    public string ActiveOverlayId => activeOverlayId;

    /// <summary>
    /// Returns true when any overlay is visible.
    /// </summary>
    public bool HasActiveOverlay => !string.IsNullOrEmpty(activeOverlayId);

    /// <summary>
    /// Provides a read-only view of collected instrumentation samples for diagnostics and Play Mode assertions.
    /// </summary>
    public IReadOnlyList<OverlayInstrumentationSample> PerformanceSamples => instrumentationSamples;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Attempted to instantiate a second LobbyOverlayManager. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Registers an overlay implementation so it can be orchestrated by the manager.
    /// </summary>
    public void RegisterOverlay(ILobbyOverlay overlay)
    {
        if (overlay == null) throw new ArgumentNullException(nameof(overlay));
        if (string.IsNullOrWhiteSpace(overlay.OverlayId))
        {
            throw new ArgumentException("Overlay must expose a non-empty identifier.", nameof(overlay));
        }

        overlays[overlay.OverlayId] = overlay;
    }

    /// <summary>
    /// Removes a previously registered overlay. If it is currently active it will be dismissed first.
    /// </summary>
    public void UnregisterOverlay(string overlayId)
    {
        if (string.IsNullOrWhiteSpace(overlayId)) return;

        if (activeOverlayId == overlayId)
        {
            RequestHideOverlay(overlayId);
        }

        overlays.Remove(overlayId);
    }

    /// <summary>
    /// Attempts to present the requested overlay, enforcing the single-overlay policy.
    /// When another overlay is visible it is closed before the new overlay is shown.
    /// </summary>
    public bool TryShowOverlay(string overlayId)
    {
        if (!overlays.TryGetValue(overlayId, out var overlay))
        {
            Debug.LogWarning($"LobbyOverlayManager: Overlay '{overlayId}' has not been registered.");
            return false;
        }

        if (activeOverlayId == overlayId)
        {
            return true; // Already visible.
        }

        if (HasActiveOverlay)
        {
            pendingOverlayId = overlayId;
            HideActiveOverlay();
            return true;
        }

        ActivateOverlay(overlayId, overlay);
        return true;
    }

    /// <summary>
    /// Requests the active overlay (or a specific overlay) to hide itself.
    /// </summary>
    public void RequestHideOverlay(string overlayId)
    {
        if (string.IsNullOrWhiteSpace(overlayId)) return;

        if (!overlays.TryGetValue(overlayId, out var overlay))
        {
            Debug.LogWarning($"LobbyOverlayManager: Cannot hide unregistered overlay '{overlayId}'.");
            return;
        }

        if (activeOverlayId == overlayId)
        {
            BeginInstrumentation(overlayId, OverlayLifecycleStage.Closed);
            overlay.Hide();
        }
        else
        {
            overlay.Hide();
        }
    }

    /// <summary>
    /// Notifies the manager that the active overlay has completed its show transition.
    /// </summary>
    public void NotifyOverlayReady(string overlayId)
    {
        if (activeOverlayId != overlayId)
        {
            Debug.LogWarning($"LobbyOverlayManager: Received ready notification from '{overlayId}' while '{activeOverlayId}' is active.");
            return;
        }

        CompleteInstrumentation(overlayId, OverlayLifecycleStage.Opened);
        OverlayOpened?.Invoke(overlayId);
    }

    /// <summary>
    /// Notifies the manager that an overlay has fully hidden itself.
    /// </summary>
    public void NotifyOverlayHidden(string overlayId)
    {
        if (activeOverlayId == overlayId)
        {
            CompleteInstrumentation(overlayId, OverlayLifecycleStage.Closed);
            OverlayClosed?.Invoke(overlayId);
            activeOverlayId = null;
        }

        if (!string.IsNullOrEmpty(pendingOverlayId))
        {
            var nextId = pendingOverlayId;
            pendingOverlayId = null;

            if (overlays.TryGetValue(nextId, out var next))
            {
                ActivateOverlay(nextId, next);
            }
        }
    }

    private void ActivateOverlay(string overlayId, ILobbyOverlay overlay)
    {
        activeOverlayId = overlayId;
        BeginInstrumentation(overlayId, OverlayLifecycleStage.Opened);
        overlay.Show();
    }

    private void HideActiveOverlay()
    {
        if (!HasActiveOverlay) return;

        var overlay = overlays[activeOverlayId];
        BeginInstrumentation(activeOverlayId, OverlayLifecycleStage.Closed);
        overlay.Hide();
    }

    private void BeginInstrumentation(string overlayId, OverlayLifecycleStage stage)
    {
        activeInstrumentation = new InstrumentationSession(overlayId, stage);
    }

    private void CompleteInstrumentation(string overlayId, OverlayLifecycleStage stage)
    {
        if (activeInstrumentation == null ||
            activeInstrumentation.OverlayId != overlayId ||
            activeInstrumentation.Stage != stage)
        {
            Debug.LogWarning($"LobbyOverlayManager: Instrumentation mismatch for overlay '{overlayId}' stage '{stage}'.");
            return;
        }

        activeInstrumentation.Stopwatch.Stop();
        var sample = new OverlayInstrumentationSample(
            overlayId,
            stage,
            activeInstrumentation.Stopwatch.Elapsed.TotalMilliseconds,
            DateTime.UtcNow);

        instrumentationSamples.Add(sample);
        OnInstrumentationSampled?.Invoke(sample);

        Debug.Log($"[LobbyOverlayManager] Overlay '{overlayId}' {stage} in {sample.DurationMs:F2} ms (target ≤ 500 ms).");

        activeInstrumentation = null;
    }

    /// <summary>
    /// Indicates the lifecycle phase under measurement.
    /// </summary>
    public enum OverlayLifecycleStage
    {
        Opened,
        Closed
    }

    /// <summary>
    /// Captures timing data for an overlay lifecycle measurement.
    /// </summary>
    public readonly struct OverlayInstrumentationSample
    {
        public OverlayInstrumentationSample(string overlayId, OverlayLifecycleStage stage, double durationMs, DateTime timestampUtc)
        {
            OverlayId = overlayId;
            Stage = stage;
            DurationMs = durationMs;
            TimestampUtc = timestampUtc;
        }

        public string OverlayId { get; }
        public OverlayLifecycleStage Stage { get; }
        public double DurationMs { get; }
        public DateTime TimestampUtc { get; }
    }

    private sealed class InstrumentationSession
    {
        public InstrumentationSession(string overlayId, OverlayLifecycleStage stage)
        {
            OverlayId = overlayId;
            Stage = stage;
            Stopwatch = Stopwatch.StartNew();
        }

        public string OverlayId { get; }
        public OverlayLifecycleStage Stage { get; }
        public Stopwatch Stopwatch { get; }
    }
}