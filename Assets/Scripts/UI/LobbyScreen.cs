using ScreenFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class LobbyScreen : ScreenUI
    {
        [Header("UI Elements")]
        [SerializeField] private string playButtonName = "play-button";
        [SerializeField] private string settingsButtonName = "settings-button";
        [SerializeField] private string hitsButtonName = "hits-button";

        private Button playButton;
        private Button settingsButton;
        private Button hitsButton;

        private void InitializeUIElements(VisualElement root)
        {
            playButton = root.Q<Button>(playButtonName);
            settingsButton = root.Q<Button>(settingsButtonName);
            hitsButton = root.Q<Button>(hitsButtonName);

            if (playButton == null) Debug.LogWarning($"Play button '{playButtonName}' not found");
            if (settingsButton == null) Debug.LogWarning($"Settings button '{settingsButtonName}' not found");
            if (hitsButton == null) Debug.LogWarning($"Hits button '{hitsButtonName}' not found");
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
            if (hitsButton != null)
            {
                hitsButton.clicked += OnHitsButtonClicked;
            }
        }

        private void OnPlayButtonClicked()
        {
            // Load gameplay scene or show level screen
            SceneManager.LoadScene("GamePlay");
        }

        private void OnSettingsButtonClicked()
        {
            // Show settings screen
            ScreenManager.Instance.ShowScreen("SettingScreen");
        }

        private void OnHitsButtonClicked()
        {
            // Show hits/high scores screen
            // Assuming there's a screen for that, or implement later
            Debug.Log("Hits button clicked");
        }

        protected override void SetupScreen(VisualElement screen)
        {
            InitializeUIElements(screen);
            SetupEventHandlers();
        }
    }
}