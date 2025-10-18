using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controls the level failed screen UI, handling button interactions and screen visibility.
/// Manages the display of failure messages and navigation options.
/// </summary>
public class LevelFailedScreenController : MonoBehaviour
{
    /// <summary>
    /// The UIDocument component that contains the UI elements.
    /// </summary>
    [SerializeField] private UIDocument uiDocument;

    /// <summary>
    /// The root visual element of the UI document.
    /// </summary>
    private VisualElement root;

    /// <summary>
    /// Button for retrying the level.
    /// </summary>
    private Button retryButton;

    /// <summary>
    /// Button for navigating to the home menu.
    /// </summary>
    private Button homeButton;

    /// <summary>
    /// Label displaying the failure reason message.
    /// </summary>
    private Label failureMessageLabel;

    /// <summary>
    /// Reference to the level failed manager.
    /// </summary>
    private ILevelFailedManager levelFailedManager;

    /// <summary>
    /// Initializes the controller, finds the manager, and sets up the UI.
    /// </summary>
    private void Awake()
    {
        levelFailedManager = FindFirstObjectByType<LevelFailedManager>();
        if (levelFailedManager == null)
        {
            Debug.LogError("LevelFailedManager not found in scene!");
            return;
        }

        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        SetupUI();
        HideScreen();
    }

    /// <summary>
    /// Sets up the UI elements and event handlers.
    /// </summary>
    private void SetupUI()
    {
        root = uiDocument.rootVisualElement;
        retryButton = root.Q<Button>("RetryButton");
        homeButton = root.Q<Button>("HomeButton");
        failureMessageLabel = root.Q<Label>("FailureMessageLabel");

        if (retryButton != null)
            retryButton.clicked += OnRetryClicked;
        if (homeButton != null)
            homeButton.clicked += OnHomeClicked;

        levelFailedManager.OnLevelFailed += OnLevelFailed;
    }

    /// <summary>
    /// Handles the level failed event by updating the message and showing the screen.
    /// </summary>
    /// <param name="reason">The reason for the level failure.</param>
    private void OnLevelFailed(string reason)
    {
        if (failureMessageLabel != null)
        {
            failureMessageLabel.text = reason;
        }
        ShowScreen();
    }

    /// <summary>
    /// Handles the retry button click.
    /// </summary>
    private void OnRetryClicked()
    {
        levelFailedManager.RetryLevel();
        HideScreen();
    }

    /// <summary>
    /// Handles the home button click.
    /// </summary>
    private void OnHomeClicked()
    {
        levelFailedManager.GoToHome();
    }

    /// <summary>
    /// Shows the level failed screen.
    /// </summary>
    private void ShowScreen()
    {
        if (root != null)
        {
            root.style.display = DisplayStyle.Flex;
        }
    }

    /// <summary>
    /// Hides the level failed screen.
    /// </summary>
    private void HideScreen()
    {
        if (root != null)
        {
            root.style.display = DisplayStyle.None;
        }
    }

    /// <summary>
    /// Cleans up event subscriptions when the object is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        if (levelFailedManager != null)
        {
            levelFailedManager.OnLevelFailed -= OnLevelFailed;
        }
    }
}