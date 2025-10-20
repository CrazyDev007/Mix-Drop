using System;
using UnityEngine;
using UnityEngine.UIElements;
using ScreenFlow;
using System.Linq;

namespace UI.Lobby
{
    /// <summary>
    /// Controller for the How to Play screen, implementing the ScreenUI base class.
    /// Manages UXML loading, content population, user interactions, and screen lifecycle.
    /// </summary>
    public class HowToPlayScreen : ScreenUI
    {
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
            // Screen initialization is handled by the ScreenUI base class
        }

        /// <summary>
        /// Sets up the screen UI elements and event handlers.
        /// </summary>
        /// <param name="screen">The root VisualElement of the screen.</param>
        protected override void SetupScreen(VisualElement screen)
        {
            // Find UI elements
            closeButton = screen.Q<Button>("close-button");
            instructionsScroll = screen.Q<ScrollView>("instructions-scroll");

            // Validate required elements
            if (closeButton == null)
            {
                Debug.LogError("Close button not found in LobbyHowToPlay UXML");
            }
            if (instructionsScroll == null)
            {
                Debug.LogError("Instructions scroll view not found in LobbyHowToPlay UXML");
            }

            // Set up event handlers
            if (closeButton != null)
            {
                closeButton.clicked += OnCloseButtonClicked;
            }

            // Populate content
            PopulateContent();
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
        /// Shows the screen and sets up event handlers.
        /// </summary>
        public override void Show()
        {
            base.Show();

            // Focus management - set initial focus to close button for accessibility
            if (closeButton != null)
            {
                closeButton.Focus();
            }

            // Emit analytics event for screen opened
            EmitAnalyticsEvent("how_to_play_opened");
        }

        /// <summary>
        /// Hides the screen and cleans up event handlers.
        /// </summary>
        public void Hide()
        {
            // Clean up event handlers
            if (closeButton != null)
            {
                closeButton.clicked -= OnCloseButtonClicked;
            }

            // Emit analytics event for screen closed
            EmitAnalyticsEvent("how_to_play_closed");
        }

        /// <summary>
        /// Handles the close button click event.
        /// </summary>
        private void OnCloseButtonClicked()
        {
            // Emit analytics event for close button interaction
            EmitAnalyticsEvent("how_to_play_close_clicked");

            // Navigate back to the lobby screen
            if (ScreenManager.Instance.GetAvailableScreenTypes().Contains("LobbyScreen"))
            {
                ScreenManager.Instance.ShowScreen("LobbyScreen");
            }
            else
            {
                Debug.LogWarning("Lobby screen not found");
            }
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
            //     { "overlay_id", "HowToPlay" },
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