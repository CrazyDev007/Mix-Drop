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
        private string starsContainerName = "stars-container";
        
        [Header("References")]
        // TextFileGameDataStorage reference removed - now using GameManager
        
        private Button nextButton;
        private Button retryButton;
        private Button homeButton;
        private Label successMsgText;
        private VisualElement starsContainer;

        private void OnEnable()
        {
            //Event subscriptions can be done here if needed
            EventManager.OnLevelComplteted += ShowCompletedScreen;
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
            
            // Display stars after screen is shown
            DisplayStars();
        }
        
        private void DisplayStars()
        {
            if (starsContainer == null) return;
            
            // Get current level and stars from GameManager
            int currentLevel = 1;
            int starsEarned = 0;
            
            if (GameManager.Instance != null)
            {
                // CurrentLevel is already incremented in GameWin, so we need to subtract 1
                currentLevel = Mathf.Max(1, GameManager.Instance.GetCurrentLevel() - 1);
                starsEarned = GameManager.Instance.GetLevelStars(currentLevel);
            }
            else
            {
                Debug.LogWarning("GameManager instance not available in LevelCompletedScreenUI");
            }
            
            // Clear existing stars
            starsContainer.Clear();
            
            // Create star elements
            for (int i = 1; i <= 3; i++)
            {
                var star = new Label();
                star.text = i <= starsEarned ? "★" : "☆";
                star.name = $"star-{i}";
                star.AddToClassList(i <= starsEarned ? "filled-star" : "empty-star");
                starsContainer.Add(star);
            }
            
            // Update success message
            if (successMsgText != null)
            {
                successMsgText.text = $"Level {currentLevel} Completed!";
            }
            
            Debug.Log($"Displayed {starsEarned} stars for level {currentLevel}");
        }

        private void InitializeUIElements(VisualElement root)
        {
            successMsgText = root.Q<Label>(sucessMessageLableName);
            nextButton = root.Q<Button>(nextButtonName);
            retryButton = root.Q<Button>(retryButtonName);
            homeButton = root.Q<Button>(homeButtonName);
            starsContainer = root.Q<VisualElement>(starsContainerName);
            
            if (successMsgText == null) Debug.LogWarning($"Success message lable '{successMsgText}' not found");
            if (nextButton == null) Debug.LogWarning($"Next button '{nextButton}' not found");
            if (retryButton == null) Debug.LogWarning($"Retry button '{retryButton}' not found");
            if (homeButton == null) Debug.LogWarning($"Home button '{homeButton}' not found");
            if (starsContainer == null) Debug.LogWarning($"Stars container '{starsContainerName}' not found");
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
            RefreshLevelData();
            ScreenManager.Instance.ShowScreen("gameplay-screen");
            GameManager.Instance.ProceedToNextLevel();
        }

        private void OnClickBtnRestart()
        {
            //GameManager.Instance.RestartGame();
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        private void OnClickBtnHome()
        {
            RefreshLevelData();
            SceneManager.LoadSceneAsync("Main");
        }
        
        /// <summary>
        /// Refreshes level data when returning to level select screen
        /// </summary>
        private void RefreshLevelData()
        {
            // Find the LevelScreen and refresh its data
            var levelScreen = FindObjectOfType<LevelScreen>();
            if (levelScreen != null)
            {
                levelScreen.RefreshLevelData();
            }
        }

        protected override void SetupScreen(VisualElement screen)
        {
            InitializeUIElements(screen);
            SetupEventHandlers();
        }
    }
}
