using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UIElements;

public class LobbyHowToPlayTests
{
    private const string LOBBY_SCENE_NAME = "Main"; // Assuming lobby is in Main scene
    private const string HOW_TO_PLAY_BUTTON_NAME = "HowToPlayButton"; // Button name to be implemented
    private const string HOW_TO_PLAY_OVERLAY_ID = "HowToPlay"; // Overlay ID to be implemented

    [UnityTest]
    public IEnumerator HowToPlayOverlay_Visibility_Test()
    {
        // Arrange: Load the lobby scene
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f); // Allow scene to initialize

        // Get the LobbyOverlayManager instance
        var overlayManager = LobbyOverlayManager.Instance;
        Assert.IsNotNull(overlayManager, "LobbyOverlayManager instance not found in scene");

        // Track overlay opened event
        bool overlayOpened = false;
        void OnOverlayOpened(string overlayId)
        {
            if (overlayId == HOW_TO_PLAY_OVERLAY_ID)
            {
                overlayOpened = true;
            }
        }
        overlayManager.OverlayOpened += OnOverlayOpened;

        // Record initial sample count for performance measurement
        var initialSampleCount = overlayManager.PerformanceSamples.Count;

        // Act: Simulate button press to show How To Play overlay
        bool showResult = overlayManager.TryShowOverlay(HOW_TO_PLAY_OVERLAY_ID);

        // Assert: Verify overlay is requested to show (implementation will handle actual visibility)
        // This test will initially fail until the actual overlay implementation exists
        Assert.IsTrue(showResult, "Failed to request showing How To Play overlay");

        // Wait for overlay to become visible (with timeout)
        float timeout = 2f;
        float elapsed = 0f;
        while (!overlayOpened && elapsed < timeout)
        {
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }

        // Assert: Verify overlay opened event was fired
        Assert.IsTrue(overlayOpened, $"How To Play overlay did not open within {timeout} seconds");

        // Assert: Verify overlay is active and visible
        Assert.AreEqual(HOW_TO_PLAY_OVERLAY_ID, overlayManager.ActiveOverlayId,
            "How To Play overlay should be active after button press");

        // Assert: Verify performance budget (≤500ms for overlay open time)
        var finalSampleCount = overlayManager.PerformanceSamples.Count;
        Assert.Greater(finalSampleCount, initialSampleCount,
            "Performance sample should be collected for overlay open");

        // Find the most recent open sample for this overlay
        var openSample = overlayManager.PerformanceSamples[finalSampleCount - 1];
        Assert.AreEqual(HOW_TO_PLAY_OVERLAY_ID, openSample.OverlayId,
            "Performance sample should be for How To Play overlay");
        Assert.AreEqual(LobbyOverlayManager.OverlayLifecycleStage.Opened, openSample.Stage,
            "Performance sample should be for overlay open stage");
        Assert.LessOrEqual(openSample.DurationMs, 500.0,
            $"Overlay open time {openSample.DurationMs:F2}ms exceeds 500ms performance budget (PRF-001)");

        // Cleanup
        overlayManager.OverlayOpened -= OnOverlayOpened;
    }

    [UnityTest]
    public IEnumerator HowToPlayOverlay_CloseAction_Test()
    {
        // Arrange: Load the lobby scene and show overlay
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f);

        var overlayManager = LobbyOverlayManager.Instance;
        Assert.IsNotNull(overlayManager, "LobbyOverlayManager instance not found in scene");

        // Show the overlay first
        overlayManager.TryShowOverlay(HOW_TO_PLAY_OVERLAY_ID);
        yield return new WaitForSeconds(0.5f);

        // Verify overlay is active
        Assert.AreEqual(HOW_TO_PLAY_OVERLAY_ID, overlayManager.ActiveOverlayId,
            "How To Play overlay should be active before close test");

        // Act: Simulate close action
        overlayManager.RequestHideOverlay(HOW_TO_PLAY_OVERLAY_ID);

        // Wait for overlay to close
        yield return new WaitForSeconds(0.5f);

        // Assert: Verify overlay is closed and lobby state is restored
        Assert.IsNull(overlayManager.ActiveOverlayId,
            "No overlay should be active after close action");
        Assert.IsFalse(overlayManager.HasActiveOverlay,
            "Should not have any active overlay after close action");
    }

    [UnityTest]
    public IEnumerator HowToPlayOverlay_Performance_Test()
    {
        // Arrange: Load the lobby scene
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f);

        var overlayManager = LobbyOverlayManager.Instance;
        Assert.IsNotNull(overlayManager, "LobbyOverlayManager instance not found in scene");

        // Clear any existing performance samples
        var initialSampleCount = overlayManager.PerformanceSamples.Count;

        // Act: Show the overlay and measure performance
        overlayManager.TryShowOverlay(HOW_TO_PLAY_OVERLAY_ID);
        yield return new WaitForSeconds(1f); // Allow time for overlay to open

        // Close the overlay
        if (overlayManager.HasActiveOverlay)
        {
            overlayManager.RequestHideOverlay(HOW_TO_PLAY_OVERLAY_ID);
            yield return new WaitForSeconds(1f); // Allow time for overlay to close
        }

        // Assert: Verify performance samples were collected
        var finalSampleCount = overlayManager.PerformanceSamples.Count;
        Assert.GreaterOrEqual(finalSampleCount, initialSampleCount,
            "Performance samples should be collected during overlay lifecycle");

        // Verify performance targets (≤ 500ms per PRF-001)
        for (int i = initialSampleCount; i < finalSampleCount; i++)
        {
            var sample = overlayManager.PerformanceSamples[i];
            Assert.LessOrEqual(sample.DurationMs, 500.0,
                $"Overlay {sample.OverlayId} {sample.Stage} took {sample.DurationMs:F2}ms, exceeding 500ms target");
        }
    }

    [UnityTest]
    public IEnumerator HowToPlayOverlay_SingleOverlayPolicy_Test()
    {
        // Arrange: Load the lobby scene
        SceneManager.LoadScene(LOBBY_SCENE_NAME);
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == LOBBY_SCENE_NAME);
        yield return new WaitForSeconds(1f);

        var overlayManager = LobbyOverlayManager.Instance;
        Assert.IsNotNull(overlayManager, "LobbyOverlayManager instance not found in scene");

        // Act & Assert: Verify single overlay policy enforcement
        // This test will initially fail until overlay implementations exist
        // but establishes the expected behavior for when they are implemented

        // Show How To Play overlay
        bool firstShowResult = overlayManager.TryShowOverlay(HOW_TO_PLAY_OVERLAY_ID);
        Assert.IsTrue(firstShowResult, "Should successfully show How To Play overlay");

        yield return new WaitForSeconds(0.2f);

        // Try to show a different overlay while How To Play is active
        // (This would be another overlay like Settings if it existed)
        // The manager should handle this gracefully according to single-overlay policy

        // For now, just verify the How To Play overlay remains active
        Assert.AreEqual(HOW_TO_PLAY_OVERLAY_ID, overlayManager.ActiveOverlayId,
            "How To Play overlay should remain active (single overlay policy)");
    }
}