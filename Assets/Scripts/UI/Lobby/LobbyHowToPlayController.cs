using System;
using UnityEngine;
using UnityEngine.UIElements;
using UI.Lobby;

namespace UI.Lobby
{
    /// <summary>
    /// Controller for the How to Play overlay, implementing the ILobbyOverlay interface.
    /// Manages UXML loading, content population, user interactions, and overlay lifecycle.
    /// </summary>
    public class LobbyHowToPlayController : MonoBehaviour, ILobbyOverlay
    {
        /// <summary>
        /// Unique identifier for this overlay type, used by LobbyOverlayManager.
        /// </summary>
        public string OverlayId => "HowToPlay";

        /// <summary>
        /// Gets the root VisualElement containing the overlay UI.
        /// </summary>
        public VisualElement RootElement { get; private set; }

        /// <summary>
        /// Indicates whether the overlay is currently visible.
        /// </summary>
        public bool IsVisible { get; private set; }

        // UI element references
        private Button closeButton;
        private ScrollView instructionsScroll;

        // Content strings - could be localized in the future
        private const string TitleText = "How to Play";
        private const string WelcomeText = "Welcome to Mix-Drop!";
        private const string IntroText = "Mix-Drop is a challenging puzzle game where you must combine colored liquids to create new mixtures. Here's how to play:";
        private const string ObjectiveTitle = "🎯 Objective";
        private const string ObjectiveText = "Fill the tubes with the correct liquid combinations to complete each level. Match the target colors shown at the top of the screen.";
        private const string ControlsTitle = "🎮 Controls";
        private const string ControlsText1 = "• Tap a tube to select it";
        private const string ControlsText2 = "• Tap another tube to pour liquid into it";
        private const string ControlsText3 = "• Liquids mix automatically when poured";
        private const string ControlsText4 = "• Complete a tube by filling it with a single color";
        private const string TipsTitle = "💡 Tips";
        private const string TipsText1 = "• Plan your moves carefully - you can't undo pours";
        private const string TipsText2 = "• Look for opportunities to create intermediate colors";
        private const string TipsText3 = "• Use the hint system if you get stuck";
        private const string TipsText4 = "• Some levels require multiple mixing steps";
        private const string WinningTitle = "🏆 Winning";
        private const string WinningText = "Complete all tubes with the correct colors to advance to the next level. Good luck!";

        private void Awake()
        {
            InitializeOverlay();
            RegisterWithManager();
        }

        /// <summary>
        /// Initializes the overlay by loading UXML and setting up the UI structure.
        /// </summary>
        private void InitializeOverlay()
        {
            // Load the UXML asset
            var uxmlAsset = Resources.Load<VisualTreeAsset>("UI Toolkit/Lobby/LobbyHowToPlay");
            if (uxmlAsset == null)
            {
                Debug.LogError("Failed to load LobbyHowToPlay UXML asset");
                return;
            }

            // Instantiate the UXML
            RootElement = uxmlAsset.Instantiate();
            RootElement.style.display = DisplayStyle.None; // Start hidden

            // Load and apply the USS stylesheet
            var ussAsset = Resources.Load<StyleSheet>("UI Toolkit/Lobby/LobbyHowToPlay");
            if (ussAsset != null)
            {
                RootElement.styleSheets.Add(ussAsset);
            }
            else
            {
                Debug.LogWarning("Failed to load LobbyHowToPlay USS asset");
            }

            // Find UI elements
            closeButton = RootElement.Q<Button>("close-button");
            instructionsScroll = RootElement.Q<ScrollView>("instructions-scroll");

            // Validate required elements
            if (closeButton == null)
            {
                Debug.LogError("Close button not found in LobbyHowToPlay UXML");
            }
            if (instructionsScroll == null)
            {
                Debug.LogError("Instructions scroll view not found in LobbyHowToPlay UXML");
            }

            // Populate content
            PopulateContent();
        }

        /// <summary>
        /// Registers this overlay with the LobbyOverlayManager.
        /// </summary>
        private void RegisterWithManager()
        {
            if (LobbyOverlayManager.Instance != null)
            {
                LobbyOverlayManager.Instance.RegisterOverlay(this);
                Debug.Log("LobbyHowToPlayController registered with LobbyOverlayManager");
            }
            else
            {
                Debug.LogWarning("LobbyOverlayManager instance not found during registration");
            }
        }

        /// <summary>
        /// Populates the UI with content strings and sets up event handlers.
        /// </summary>
        private void PopulateContent()
        {
            if (instructionsScroll == null) return;

            // The content is already defined in the UXML, but we could dynamically populate if needed
            // For now, the static content in UXML is sufficient
        }

        /// <summary>
        /// Shows the overlay and sets up event handlers.
        /// </summary>
        public void Show()
        {
            if (RootElement == null) return;

            IsVisible = true;
            RootElement.style.display = DisplayStyle.Flex;

            // Set up event handlers
            if (closeButton != null)
            {
                closeButton.clicked += OnCloseButtonClicked;
            }

            // Focus management - set initial focus to close button for accessibility
            if (closeButton != null)
            {
                closeButton.Focus();
            }

            // Emit analytics event for overlay opened
            EmitAnalyticsEvent("how_to_play_opened");

            // Notify manager that overlay is ready
            LobbyOverlayManager.Instance.NotifyOverlayReady(OverlayId);
        }

        /// <summary>
        /// Hides the overlay and cleans up event handlers.
        /// </summary>
        public void Hide()
        {
            if (RootElement == null) return;

            IsVisible = false;
            RootElement.style.display = DisplayStyle.None;

            // Clean up event handlers
            if (closeButton != null)
            {
                closeButton.clicked -= OnCloseButtonClicked;
            }

            // Emit analytics event for overlay closed
            EmitAnalyticsEvent("how_to_play_closed");
        }

        /// <summary>
        /// Handles the close button click event.
        /// </summary>
        private void OnCloseButtonClicked()
        {
            // Emit analytics event for close button interaction
            EmitAnalyticsEvent("how_to_play_close_clicked");

            // Request overlay hide through manager
            LobbyOverlayManager.Instance.RequestHideOverlay(OverlayId);
        }

        /// <summary>
        /// Emits analytics events for tracking user interactions.
        /// </summary>
        /// <param name="eventName">The name of the analytics event.</param>
        private void EmitAnalyticsEvent(string eventName)
        {
            // Placeholder for analytics integration
            // In a real implementation, this would send events to an analytics service
            Debug.Log($"[Analytics] {eventName}");

            // Example of what a real analytics call might look like:
            // AnalyticsManager.Instance.TrackEvent(eventName, new Dictionary<string, object>
            // {
            //     { "overlay_id", OverlayId },
            //     { "timestamp", DateTime.UtcNow }
            // });
        }

        private void OnDestroy()
        {
            // Clean up event handlers in case of unexpected destruction
            if (closeButton != null)
            {
                closeButton.clicked -= OnCloseButtonClicked;
            }
        }

        #region Unit Test Stubs

        /// <summary>
        /// Test-only method to simulate close button click.
        /// </summary>
        internal void TestSimulateCloseButtonClick()
        {
            OnCloseButtonClicked();
        }

        /// <summary>
        /// Test-only property to access close button for verification.
        /// </summary>
        internal Button TestCloseButton => closeButton;

        /// <summary>
        /// Test-only property to access instructions scroll view for verification.
        /// </summary>
        internal ScrollView TestInstructionsScroll => instructionsScroll;

        #endregion
    }
}