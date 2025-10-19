using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the restart confirmation dialog, handling display and confirmation logic.
/// Provides events for restart confirmation and back navigation.
/// This manager ensures users don't accidentally restart levels by requiring explicit confirmation.
/// </summary>
public class RestartConfirmManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the RestartConfirmManager.
    /// Ensures only one instance exists throughout the application lifecycle.
    /// </summary>
    public static RestartConfirmManager Instance { get; private set; }

    /// <summary>
    /// Indicates whether the restart confirmation screen is currently visible.
    /// Used by UI controllers to manage screen state.
    /// </summary>
    public bool IsConfirmScreenVisible { get; private set; }

    /// <summary>
    /// Event triggered when the player confirms the restart action.
    /// Subscribers can perform additional cleanup before restart.
    /// </summary>
    public event Action OnRestartConfirmed;

    /// <summary>
    /// Event triggered when the player selects back from the confirmation screen.
    /// Allows other systems to respond to cancellation.
    /// </summary>
    public event Action OnBackSelected;

    /// <summary>
    /// The description text to display on the confirmation screen.
    /// Set when showing the confirmation dialog.
    /// </summary>
    public string ConfirmDescription { get; private set; }

    /// <summary>
    /// Initializes the singleton instance.
    /// Ensures only one manager exists and prevents duplicates.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Shows the restart confirmation screen with the specified description.
    /// Sets the confirmation state and prepares the UI for display.
    /// </summary>
    /// <param name="description">The description to display on the confirmation screen. Cannot be null or empty.</param>
    /// <exception cref="ArgumentException">Thrown when description is null or empty.</exception>
    public void ShowConfirm(string description)
    {
        if (string.IsNullOrEmpty(description))
        {
            throw new ArgumentException("Description cannot be null or empty", nameof(description));
        }

        if (IsConfirmScreenVisible) return;

        ConfirmDescription = description;
        IsConfirmScreenVisible = true;

        // Show the confirmation screen - handled by UI controller
        Debug.Log($"Showing restart confirmation: {description}");
    }

    /// <summary>
    /// Hides the restart confirmation screen.
    /// Resets the confirmation state and clears the description.
    /// </summary>
    public void HideConfirm()
    {
        if (!IsConfirmScreenVisible) return;

        IsConfirmScreenVisible = false;
        ConfirmDescription = null;

        // Hide the confirmation screen - handled by UI controller
        Debug.Log("Hiding restart confirmation");
    }

    /// <summary>
    /// Confirms the restart action and triggers the restart.
    /// Hides the confirmation screen and reloads the current scene.
    /// </summary>
    public void ConfirmRestart()
    {
        HideConfirm();
        OnRestartConfirmed?.Invoke();

        // Restart the current level by reloading the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Handles the back selection, hiding the confirmation screen.
    /// Allows the user to return to the previous screen without restarting.
    /// </summary>
    public void SelectBack()
    {
        HideConfirm();
        OnBackSelected?.Invoke();
    }
}