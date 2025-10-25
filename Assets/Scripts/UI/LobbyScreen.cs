using System.Linq;
using ScreenFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class LobbyScreen : ScreenUI
    {
        private string playButtonName = "play-button";
        private string settingsButtonName = "settings-button";
        private string howToPlayButtonName = "how-to-play-button";

        private Button playButton;
        private Button settingsButton;
        private Button howToPlayButton;

        private void InitializeUIElements(VisualElement root)
        {
            playButton = root.Q<Button>(playButtonName);
            settingsButton = root.Q<Button>(settingsButtonName);
            howToPlayButton = root.Q<Button>(howToPlayButtonName);

            if (playButton == null) Debug.LogWarning($"Play button '{playButtonName}' not found");
            if (settingsButton == null) Debug.LogWarning($"Settings button '{settingsButtonName}' not found");
            if (howToPlayButton == null) Debug.LogWarning($"How to Play button '{howToPlayButtonName}' not found");
        }

        private void SetupEventHandlers()
        {
            if (playButton != null)
            {
                playButton.clicked += OnPlayButtonClicked;
            }
            if (settingsButton != null)
            {
                settingsButton.clicked += OnSettingsButtonClicked;
            }
            if (howToPlayButton != null)
            {
                howToPlayButton.clicked += OnHowToPlayButtonClicked;
            }
        }

        private void OnPlayButtonClicked()
        {
            // Show level selection screen
            ScreenManager.Instance.ShowScreen("LevelScreen");
        }

        private void OnSettingsButtonClicked()
        {
            // Show settings screen
            ScreenManager.Instance.ShowScreen("SettingScreen");
        }

        private void OnHowToPlayButtonClicked()
        {
            // Show How to Play overlay through ScreenManager
            if (ScreenManager.Instance.GetAvailableScreenTypes().Contains("HowToPlay"))
            {
                ScreenManager.Instance.ShowScreen("HowToPlay");
            }
            else
            {
                Debug.LogWarning("HowToPlay Screen not found");
            }
        }

        protected override void SetupScreen(VisualElement screen)
        {
            InitializeUIElements(screen);
            SetupEventHandlers();
        }
    }
}