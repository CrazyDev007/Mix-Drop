using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controls the restart confirmation screen UI, handling button interactions and screen visibility.
/// Manages the display of confirmation messages and user selections.
/// This controller bridges the UI elements with the RestartConfirmManager logic.
/// </summary>
public class RestartConfirmScreenController : MonoBehaviour
{
    /// <summary>
    /// The UIDocument component that contains the UI elements.
    /// Automatically assigned if not set in inspector.
    /// </summary>
    [SerializeField] private UIDocument uiDocument;

    /// <summary>
    /// The root visual element of the UI document.
    /// Used to show/hide the entire confirmation screen.
    /// </summary>
    private VisualElement root;

    /// <summary>
    /// Button for confirming the restart action.
    /// Triggers the level restart when clicked.
    /// </summary>
    private Button restartButton;

    /// <summary>
    /// Button for canceling and going back.
    /// Returns to the previous screen without restarting.
    /// </summary>
    private Button backButton;

    /// <summary>
    /// Label displaying the confirmation description message.
    /// Updates dynamically based on the confirmation context.
    /// </summary>
    private Label descriptionLabel;

    /// <summary>
    /// Reference to the restart confirm manager.
    /// Used to communicate with the confirmation logic.
    /// </summary>
    private RestartConfirmManager restartConfirmManager;

    /// <summary>
    /// Initializes the controller, finds the manager, and sets up the UI.
    /// Ensures all components are properly connected.
    /// </summary>
    private void Awake()
    {
        restartConfirmManager = RestartConfirmManager.Instance;
        if (restartConfirmManager == null)
        {
            Debug.LogError("RestartConfirmManager not found!");
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
    /// Queries the visual elements and wires up button click events.
    /// </summary>
    private void SetupUI()
    {
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument is not assigned!");
            return;
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Root visual element not found!");
            return;
        }

        restartButton = root.Q<Button>("RestartButton");
        backButton = root.Q<Button>("BackButton");
        descriptionLabel = root.Q<Label>("DescriptionLabel");

        if (restartButton == null)
            Debug.LogWarning("RestartButton not found in UXML!");
        else
            restartButton.clicked += OnRestartClicked;

        if (backButton == null)
            Debug.LogWarning("BackButton not found in UXML!");
        else
            backButton.clicked += OnBackClicked;

        if (descriptionLabel == null)
            Debug.LogWarning("DescriptionLabel not found in UXML!");

        restartConfirmManager.OnRestartConfirmed += OnRestartConfirmed;
        restartConfirmManager.OnBackSelected += OnBackSelected;
    }

    /// <summary>
    /// Handles the restart button click.
    /// Delegates to the manager to confirm and execute the restart.
    /// </summary>
    private void OnRestartClicked()
    {
        restartConfirmManager.ConfirmRestart();
    }

    /// <summary>
    /// Handles the back button click.
    /// Delegates to the manager to cancel the confirmation.
    /// </summary>
    private void OnBackClicked()
    {
        restartConfirmManager.SelectBack();
    }

    /// <summary>
    /// Handles the restart confirmed event by hiding the screen.
    /// Ensures the UI is hidden after restart is initiated.
    /// </summary>
    private void OnRestartConfirmed()
    {
        HideScreen();
    }

    /// <summary>
    /// Handles the back selected event by hiding the screen.
    /// Returns the UI to hidden state when user cancels.
    /// </summary>
    private void OnBackSelected()
    {
        HideScreen();
    }

    /// <summary>
    /// Shows the restart confirmation screen and updates the description.
    /// Makes the screen visible and refreshes the message text.
    /// </summary>
    private void ShowScreen()
    {
        if (root != null)
        {
            root.style.display = DisplayStyle.Flex;
            UpdateDescription();
        }
    }

    /// <summary>
    /// Hides the restart confirmation screen.
    /// Completely hides the UI elements.
    /// </summary>
    private void HideScreen()
    {
        if (root != null)
        {
            root.style.display = DisplayStyle.None;
        }
    }

    /// <summary>
    /// Updates the description label with the current confirmation message.
    /// Retrieves the latest description from the manager.
    /// </summary>
    private void UpdateDescription()
    {
        if (descriptionLabel != null && restartConfirmManager != null)
        {
            descriptionLabel.text = restartConfirmManager.ConfirmDescription;
        }
    }

    /// <summary>
    /// Checks for screen visibility changes in Update.
    /// Synchronizes the UI state with the manager's visibility flag.
    /// </summary>
    private void Update()
    {
        if (restartConfirmManager != null)
        {
            if (restartConfirmManager.IsConfirmScreenVisible && root.style.display == DisplayStyle.None)
            {
                ShowScreen();
            }
            else if (!restartConfirmManager.IsConfirmScreenVisible && root.style.display == DisplayStyle.Flex)
            {
                HideScreen();
            }
        }
    }

    /// <summary>
    /// Cleans up event subscriptions when the object is destroyed.
    /// Prevents memory leaks and null reference exceptions.
    /// </summary>
    private void OnDestroy()
    {
        if (restartConfirmManager != null)
        {
            restartConfirmManager.OnRestartConfirmed -= OnRestartConfirmed;
            restartConfirmManager.OnBackSelected -= OnBackSelected;
        }
    }
}