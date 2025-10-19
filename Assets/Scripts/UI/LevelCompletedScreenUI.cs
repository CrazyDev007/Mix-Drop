using System;
using ScreenFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class LevelCompletedScreenUI : ScreenUI
    {
        [Header("UI Elements")]
        private string sucessMessageLableName = "SuccessMessageLabel";
        private string nextButtonName = "NextButton";
        private string retryButtonName = "RetryButton";
        private string homeButtonName = "HomeButton";
        
        private Button nextButton;
        private Button retryButton;
        private Button homeButton;
        private Label successMsgText;

        private void OnEnable()
        {
            //Event subscriptions can be done here if needed
            EventManager.OnLevelComplteted += ShowCompletedScreen;
            Debug.Log("Level completed screen enabled");
        }

        private void OnDestroy()
        {
            //Event unsubscriptions can be done here if needed
            EventManager.OnLevelComplteted -= ShowCompletedScreen;
        }
        
        void ShowCompletedScreen()
        {
            Debug.Log("Level completed screen shown");
            //this.Show();
            ScreenManager.Instance.ShowScreen("level-completed-screen");
        }

        private void InitializeUIElements(VisualElement root)
        {
            successMsgText = root.Q<Label>(sucessMessageLableName);
            nextButton = root.Q<Button>(nextButtonName);
            retryButton = root.Q<Button>(retryButtonName);
            homeButton = root.Q<Button>(homeButtonName);
            
            if (successMsgText == null) Debug.LogWarning($"Success message lable '{successMsgText}' not found");
            if (nextButton == null) Debug.LogWarning($"Next button '{nextButton}' not found");
            if (retryButton == null) Debug.LogWarning($"Retry button '{retryButton}' not found");
            if (homeButton == null) Debug.LogWarning($"Home button '{homeButton}' not found");
        }
        
        private void SetupEventHandlers()
        {
            if (nextButton != null)
            {
                nextButton.clicked += OnClickBtnNext;
            }
            if (retryButton != null)
            {
                retryButton.clicked += OnClickBtnRestart;
            }
            if (homeButton != null)
            {
                homeButton.clicked += OnClickBtnHome;
            }
        }

        private void OnClickBtnNext()
        {
            GameManager.Instance.GameWin();
            ScreenManager.Instance.ShowScreen("gameplay-screen");
        }

        private void OnClickBtnRestart()
        {
            //GameManager.Instance.RestartGame();
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnClickBtnHome()
        {
            SceneManager.LoadSceneAsync("Main");
        }

        protected override void SetupScreen(VisualElement screen)
        {
            InitializeUIElements(screen);
            SetupEventHandlers();
        }
    }
}
