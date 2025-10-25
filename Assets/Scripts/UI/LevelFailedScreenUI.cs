using ScreenFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class LevelFailedScreenUI : ScreenUI
    {
        [Header("UI Elements")]
        private string retryButtonName = "RetryButton";
        private string homeButtonName = "HomeButton";
        
        private Button retryButton;
        private Button homeButton;

        private void InitializeUIElements(VisualElement root)
        {
            retryButton = root.Q<Button>(retryButtonName);
            homeButton = root.Q<Button>(homeButtonName);
            
            if (homeButton == null) Debug.LogWarning($"Home button '{homeButton}' not found");
            if (retryButton == null) Debug.LogWarning($"Retry button '{retryButton}' not found");
        }
        
        private void SetupEventHandlers()
        {
            if (retryButton != null)
            {
                retryButton.clicked += OnClickBtnRestart;
            }
            if (homeButton != null)
            {
                homeButton.clicked += OnClickBtnHome;
            }
        }
        
        private void OnClickBtnRestart()
        {
            AudioManager.Instance?.PlayButtonTap();
            GameManager.Instance.RestartGame();
            ScreenManager.Instance.ShowScreen("gameplay-screen");
        }

        private void OnClickBtnHome()
        {
            AudioManager.Instance?.PlayButtonTap();
            SceneManager.LoadSceneAsync("Main");
        }

        protected override void SetupScreen(VisualElement screen)
        {
            InitializeUIElements(screen);
            SetupEventHandlers();
        }
    }
}
