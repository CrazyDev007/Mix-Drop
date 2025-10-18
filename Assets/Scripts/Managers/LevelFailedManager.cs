using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages level failure state and provides retry/home navigation functionality.
/// Implements the ILevelFailedManager interface for level failure handling.
/// </summary>
public class LevelFailedManager : MonoBehaviour, ILevelFailedManager
{
    /// <summary>
    /// Indicates whether the current level has failed.
    /// </summary>
    public bool IsLevelFailed { get; private set; }

    /// <summary>
    /// The reason for the level failure, if provided.
    /// </summary>
    public string FailureReason { get; private set; }

    /// <summary>
    /// Event triggered when a level failure occurs.
    /// </summary>
    public event Action<string> OnLevelFailed;

    /// <summary>
    /// Initializes the manager state.
    /// </summary>
    private void Awake()
    {
        // Initialize state
        IsLevelFailed = false;
        FailureReason = null;
    }

    /// <summary>
    /// Triggers a level failure with an optional reason.
    /// </summary>
    /// <param name="reason">The reason for the failure (optional).</param>
    public void TriggerLevelFailed(string reason = null)
    {
        if (IsLevelFailed) return; // Already failed

        IsLevelFailed = true;
        FailureReason = reason ?? "Level Failed";

        // Trigger event for UI
        OnLevelFailed?.Invoke(FailureReason);

        // TODO: Show level failed screen
        Debug.Log($"Level failed: {FailureReason}");
    }

    /// <summary>
    /// Retries the current level by reloading the scene.
    /// </summary>
    public void RetryLevel()
    {
        if (!IsLevelFailed) return;

        // Reset state
        IsLevelFailed = false;
        FailureReason = null;

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Navigates to the home/main menu scene.
    /// </summary>
    public void GoToHome()
    {
        // Navigate to main menu scene
        // Assuming "Main" is the home scene
        SceneManager.LoadScene("Main");
    }
}