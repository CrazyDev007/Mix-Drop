using ScreenFlow;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using MixDrop.Data;

namespace UI
{
    public class LevelScreen : ScreenUI
    {
        // Header elements
        private Label headerTitle;
        private Label headerSubtitle;
        private Label progressLabel;
        private VisualElement progressBarFill;
        private Label progressText;
        private Label statValueStars;
        private Label statValueLevels;
        
        // Level grid elements
        private ScrollView levelScrollView;
        private VisualElement levelGridContainer;
        private VisualTreeAsset levelRowTemplate;
        
        // Pagination elements
        private Button previousPageButton;
        private Button nextPageButton;
        private VisualElement pageIndicators;
        
        // Navigation elements
        private Button backButton;
        
        // Data
        private int currentPage = 0;
        private int levelsPerPage = 16; // 4x5 grid
        private int totalLevels = 220;
        private int totalPages;

        protected override void SetupScreen(VisualElement screen)
        {
            // Header elements
            headerTitle = screen.Q<Label>("header-title");
            headerSubtitle = screen.Q<Label>("header-subtitle");
            progressLabel = screen.Q<Label>("progress-label");
            progressBarFill = screen.Q<VisualElement>("progress-bar-fill");
            progressText = screen.Q<Label>("progress-text");
            statValueStars = screen.Q<Label>("stat-value-stars");
            statValueLevels = screen.Q<Label>("stat-value-levels");
            
            // Level grid elements
            levelScrollView = screen.Q<ScrollView>("level-scroll-view");
            levelGridContainer = screen.Q<VisualElement>("level-grid-container");
            
            // Pagination elements
            previousPageButton = screen.Q<Button>("previous-page-button");
            nextPageButton = screen.Q<Button>("next-page-button");
            pageIndicators = screen.Q<VisualElement>("page-indicators");
            
            // Navigation elements
            backButton = screen.Q<Button>("back-button");

            // Check for critical UI elements
            if (levelGridContainer == null)
            {
                Debug.LogError("Level grid container not found in UXML!");
                return;
            }

            // Setup button click events
            if (backButton != null)
            {
                backButton.clicked += OnClickBackButton;
            }
            
            if (previousPageButton != null)
            {
                previousPageButton.clicked += OnClickPreviousPage;
            }
            
            if (nextPageButton != null)
            {
                nextPageButton.clicked += OnClickNextPage;
            }

            // Load templates
            levelRowTemplate = Resources.Load<VisualTreeAsset>("LevelButton");
            if (levelRowTemplate == null)
            {
                //Debug.LogError("LevelButton template not found in Resources!");
                return;
            }
            
            // Calculate total pages
            totalPages = Mathf.CeilToInt((float)totalLevels / levelsPerPage);
            
            // Add diagnostic logging for initial setup
            //Debug.Log("[LevelScreen] SetupScreen - Starting setup");
            //Debug.Log($"[LevelScreen] Total Levels: {totalLevels}, Levels Per Page: {levelsPerPage}, Total Pages: {totalPages}");
            
            // Initialize screen
            RefreshLevelData();
            
            // Log initial layout after a delay to ensure UI is fully initialized
            //StartCoroutine(LogInitialLayoutInfo());
        }
        
        private IEnumerator LogInitialLayoutInfo()
        {
            // Wait for a few frames to ensure UI is fully initialized
            yield return null;
            yield return null;
            
            // Log root container layout
            var rootContainer = levelScrollView.parent;
            Debug.Log($"[LevelScreen] Root Container Layout: {rootContainer.layout.width}x{rootContainer.layout.height}");
            
            // Log all main sections layout
            var headerSection = rootContainer.Q<VisualElement>("header-banner-container");
            var scrollViewSection = rootContainer.Q<ScrollView>("level-scroll-view");
            var paginationSection = rootContainer.Q<VisualElement>("pagination-container");
            var navigationSection = rootContainer.Q<VisualElement>("navigation-container");
            
            Debug.Log($"[LevelScreen] Header Section Layout: {headerSection?.layout.width}x{headerSection?.layout.height}");
            Debug.Log($"[LevelScreen] ScrollView Section Layout: {scrollViewSection?.layout.width}x{scrollViewSection?.layout.height}");
            Debug.Log($"[LevelScreen] Pagination Section Layout: {paginationSection?.layout.width}x{paginationSection?.layout.height}");
            Debug.Log($"[LevelScreen] Navigation Section Layout: {navigationSection?.layout.width}x{navigationSection?.layout.height}");
            
            // Check flex styles
            Debug.Log($"[LevelScreen] ScrollView FlexGrow: {scrollViewSection?.resolvedStyle.flexGrow}");
            Debug.Log($"[LevelScreen] ScrollView Height: {scrollViewSection?.resolvedStyle.height}");
        }

        private void UpdateHeaderStats()
        {
            // Get data from TextFileGameDataStorage instead of PlayerPrefs
            int completedLevels = 0;
            int totalStars = 0;
            TextFileGameDataStorage textFileStorage = null;
            
            // First try to get the TextFileGameDataStorage from GameManager
            if (GameManager.Instance != null)
            {
                // Use reflection to get the private field since it's not exposed publicly
                var field = typeof(GameManager).GetField("textFileStorage",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    textFileStorage = field.GetValue(GameManager.Instance) as TextFileGameDataStorage;
                }
            }
            
            // If not found in GameManager, try to find it in the scene
            if (textFileStorage == null)
            {
                textFileStorage = FindObjectOfType<TextFileGameDataStorage>();
            }
            
            if (textFileStorage != null)
            {
                completedLevels = textFileStorage.CompletedLevelsCount;
                totalStars = textFileStorage.TotalStars;
                Debug.Log($"[LevelScreen] UpdateHeaderStats - Completed Levels: {completedLevels}, Total Stars: {totalStars}");
            }
            else
            {
                // Fallback to PlayerPrefs if TextFileGameDataStorage is not available
                completedLevels = PlayerPrefs.GetInt("CompletedLevels", 0);
                totalStars = PlayerPrefs.GetInt("TotalStars", 0);
                Debug.LogWarning("[LevelScreen] UpdateHeaderStats - TextFileGameDataStorage not found, using PlayerPrefs fallback");
            }
            
            // Update progress
            if (progressBarFill != null)
            {
                float progress = totalLevels > 0 ? (float)completedLevels / totalLevels : 0;
                progressBarFill.style.width = new Length(progress * 100, LengthUnit.Percent);
            }
            
            if (progressText != null)
            {
                progressText.text = $"{completedLevels}/{totalLevels}";
            }
            
            // Update stats
            if (statValueStars != null)
            {
                statValueStars.text = totalStars.ToString();
            }
            
            if (statValueLevels != null)
            {
                statValueLevels.text = completedLevels.ToString();
            }
        }

        private void SetupLevelGrid()
        {
            levelGridContainer.Clear();
            
            // Get data from TextFileGameDataStorage instead of PlayerPrefs
            int completedLevels = 0;
            TextFileGameDataStorage textFileStorage = FindObjectOfType<TextFileGameDataStorage>();
            
            
            if (textFileStorage != null)
            {
                // Ensure textFileStorage is initialized
                if (!textFileStorage.enabled)
                {
                    Debug.LogWarning("[LevelScreen] TextFileGameDataStorage found but not enabled");
                }
                
                completedLevels = textFileStorage.CompletedLevelsCount;
                Debug.Log($"[LevelScreen] TextFileGameDataStorage found - Completed Levels: {completedLevels}, Total Stars: {textFileStorage.TotalStars}");
                
                // Log some sample level data for debugging
                for (int i = 1; i <= Mathf.Min(5, completedLevels); i++)
                {
                    bool isCompleted = textFileStorage.IsLevelCompleted(i);
                    int stars = textFileStorage.GetLevelStars(i);
                    //Debug.Log($"[LevelScreen] Sample Data - Level {i}: Completed={isCompleted}, Stars={stars}");
                }
            }
            
            int startLevel = currentPage * levelsPerPage + 1;
            int endLevel = Mathf.Min(startLevel + levelsPerPage - 1, totalLevels);
            
            // Add diagnostic logging
            //Debug.Log($"[LevelScreen] SetupLevelGrid - Page: {currentPage}, StartLevel: {startLevel}, EndLevel: {endLevel}");
            //Debug.Log($"[LevelScreen] Completed Levels: {completedLevels}");
            
            // Create level buttons for each level in this page
            for (int levelNumber = startLevel; levelNumber <= endLevel; levelNumber++)
            {
                var levelButtonElement = levelRowTemplate.CloneTree();
                
                // Get the container element
                var buttonContainer = levelButtonElement.Q<VisualElement>("level-button-container");
                if (buttonContainer != null)
                {
                    // Update level number
                    var levelNumberLabel = buttonContainer.Q<Label>("level-number");
                    if (levelNumberLabel != null)
                    {
                        levelNumberLabel.text = $"{levelNumber}";
                    }
                    
                    // Update level name
                    var levelNameLabel = buttonContainer.Q<Label>("level-name");
                    if (levelNameLabel != null)
                    {
                        levelNameLabel.text = $"Level {levelNumber}";
                    }
                    
                    // Check if level is completed
                    bool isCompleted = false;
                    isCompleted = textFileStorage.IsLevelCompleted(levelNumber);

                    // Set locked state
                    bool isLocked = levelNumber > completedLevels + 1;
                    var lockIcon = buttonContainer.Q<VisualElement>("lock-icon");
                    if (lockIcon != null)
                    {
                        lockIcon.style.display = isLocked ? DisplayStyle.Flex : DisplayStyle.None;
                    }

                    // Only set up click handler for unlocked levels
                    if (!isLocked)
                    {
                        // Add diagnostic logging to verify closure issue
                        //Debug.Log($"[LevelScreen] Setting up click handler for Level {levelNumber}");
                        
                        // Capture the levelNumber in a local variable to fix closure issue
                        int capturedLevelNumber = levelNumber;
                        //Debug.Log($"[LevelScreen] Captured level number: {capturedLevelNumber} for Level {levelNumber}");
                        
                        buttonContainer.AddManipulator(new Clickable(() =>
                        {
                            //Debug.Log($"[LevelScreen] Button clicked - Original levelNumber: {levelNumber}, Captured levelNumber: {capturedLevelNumber}");
                            OnLevelSelected(capturedLevelNumber);
                        }));
                    }
                    
                    // Set new state for the first unlocked level
                    var newIcon = buttonContainer.Q<VisualElement>("new-icon");
                    if (newIcon != null)
                    {
                        newIcon.style.display = (levelNumber == completedLevels + 1) ? DisplayStyle.Flex : DisplayStyle.None;
                    }
                    
                    // Set completed state
                    var completedIcon = buttonContainer.Q<VisualElement>("completed-icon");
                    if (completedIcon != null)
                    {
                        completedIcon.style.display = isCompleted ? DisplayStyle.Flex : DisplayStyle.None;
                    }
                    
                    // Update stars (for completed levels)
                    if (isCompleted)
                    {
                        int starsEarned = 0;
                        if (textFileStorage != null)
                        {
                            starsEarned = textFileStorage.GetLevelStars(levelNumber);
                            //Debug.Log($"[LevelScreen] TextFileStorage - Level {levelNumber} completed with {starsEarned} stars");
                        }
                        else
                        {
                            // Fallback to PlayerPrefs if TextFileGameDataStorage is not available
                            starsEarned = PlayerPrefs.GetInt($"Level{levelNumber}Stars", 0);
                            //Debug.Log($"[LevelScreen] PlayerPrefs - Level {levelNumber} completed with {starsEarned} stars");
                        }                      
                        
                        for (int i = 1; i <= 3; i++)
                        {
                            var star = buttonContainer.Q<VisualElement>($"star-{i}");
                            if (star != null)
                            {
                                // Clear existing star classes
                                star.RemoveFromClassList("level-button__star--earned");
                                star.RemoveFromClassList("level-button__star--empty");
                                star.RemoveFromClassList("star");
                                star.RemoveFromClassList("empty-star");
                                star.RemoveFromClassList("filled-star");
                                
                                // Add base star class
                                star.AddToClassList("star");
                                
                                // Add appropriate class based on stars earned
                                if (i <= starsEarned)
                                {
                                    star.AddToClassList("level-button__star--earned");
                                    //Debug.Log($"[LevelScreen] Added earned class to star {i} for level {levelNumber}");
                                }
                                else
                                {
                                    star.AddToClassList("level-button__star--empty");
                                    //Debug.Log($"[LevelScreen] Added empty class to star {i} for level {levelNumber}");
                                }
                            }
                            else
                            {
                                //Debug.LogError($"[LevelScreen] Star element {i} not found for level {levelNumber}");
                            }
                        }
                    }
                }
                
                levelGridContainer.Add(levelButtonElement);
            }
            
            // Add layout diagnostic logging
            //StartCoroutine(LogLayoutInfoAfterFrame());
        }
        
        private System.Collections.IEnumerator LogLayoutInfoAfterFrame()
        {
            // Wait for the next frame to ensure layout is calculated
            yield return null;
            
            // Log layout information
            Debug.Log($"[LevelScreen] Layout Info - ScrollView: {levelScrollView.layout.width}x{levelScrollView.layout.height}");
            Debug.Log($"[LevelScreen] Layout Info - GridContainer: {levelGridContainer.layout.width}x{levelGridContainer.layout.height}");
            Debug.Log($"[LevelScreen] Layout Info - GridContainer ChildCount: {levelGridContainer.childCount}");
            
            // Check if ScrollView has flex-grow style
            var scrollViewStyle = levelScrollView.resolvedStyle;
            Debug.Log($"[LevelScreen] ScrollView FlexGrow: {scrollViewStyle.flexGrow}");
            
            // Check if GridContainer has proper layout
            var gridContainerStyle = levelGridContainer.resolvedStyle;
            Debug.Log($"[LevelScreen] GridContainer FlexGrow: {gridContainerStyle.flexGrow}");
            Debug.Log($"[LevelScreen] GridContainer Width: {gridContainerStyle.width}");
            Debug.Log($"[LevelScreen] GridContainer Height: {gridContainerStyle.height}");
        }

        private void UpdatePagination()
        {
            // Update button states
            if (previousPageButton != null)
            {
                previousPageButton.SetEnabled(currentPage > 0);
            }
            
            if (nextPageButton != null)
            {
                nextPageButton.SetEnabled(currentPage < totalPages - 1);
            }
            
            // Update page indicators
            if (pageIndicators != null)
            {
                pageIndicators.Clear();
                for (int i = 0; i < totalPages; i++)
                {
                    var indicator = new VisualElement();
                    indicator.AddToClassList(i == currentPage ? "page-indicator-active" : "page-indicator");
                    pageIndicators.Add(indicator);
                }
            }
        }

        private void OnClickPreviousPage()
        {
            if (currentPage > 0)
            {
                currentPage--;
                SetupLevelGrid();
                UpdatePagination();
                AudioManager.Instance?.PlayButtonTap();
            }
        }

        private void OnClickNextPage()
        {
            if (currentPage < totalPages - 1)
            {
                currentPage++;
                SetupLevelGrid();
                UpdatePagination();
                AudioManager.Instance?.PlayButtonTap();
            }
        }

        private void OnLevelSelected(int levelNumber)
        {
            Debug.Log($"[LevelScreen] Level {levelNumber} selected, loading gameplay scene");
            
            // Update the current level in TextFileGameDataStorage
            TextFileGameDataStorage textFileStorage = FindObjectOfType<TextFileGameDataStorage>();
            
            if (textFileStorage != null)
            {
                textFileStorage.SetCurrentLevel(levelNumber);
                Debug.Log($"[LevelScreen] Set current level to {levelNumber} in TextFileGameDataStorage");
            }
            else
            {
                Debug.LogWarning("[LevelScreen] TextFileGameDataStorage not found, could not update current level");
            }
            
            SceneManager.LoadSceneAsync("GamePlay", LoadSceneMode.Single);
        }

        public void OnClickBackButton()
        {
            AudioManager.Instance?.PlayButtonTap();
            ScreenManager.Instance.ShowScreen("LobbyScreen");
        }
        
        /// <summary>
        /// Refreshes the level screen data, useful when returning from gameplay
        /// </summary>
        public void RefreshLevelData()
        {
            UpdateHeaderStats();
            SetupLevelGrid();
            UpdatePagination();
        }
        
        /// <summary>
        /// Test method to verify TextFileGameDataStorage is working correctly
        /// Call this from the Unity Console: FindObjectOfType<LevelScreen>().TestStorageData()
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void TestStorageData()
        {
            Debug.Log("=== Testing TextFileGameDataStorage ===");
            
            TextFileGameDataStorage textFileStorage = null;
            
            // First try to get the TextFileGameDataStorage from GameManager
            if (GameManager.Instance != null)
            {
                // Use reflection to get the private field since it's not exposed publicly
                var field = typeof(GameManager).GetField("textFileStorage",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    textFileStorage = field.GetValue(GameManager.Instance) as TextFileGameDataStorage;
                    Debug.Log("Found TextFileGameDataStorage through GameManager");
                }
            }
            
            // If not found in GameManager, try to find it in the scene
            if (textFileStorage == null)
            {
                textFileStorage = FindObjectOfType<TextFileGameDataStorage>();
                if (textFileStorage != null)
                {
                    Debug.Log("Found TextFileGameDataStorage through FindObjectOfType");
                }
            }
            
            if (textFileStorage != null)
            {
                Debug.Log($"TextFileGameDataStorage found and initialized: {textFileStorage.enabled}");
                Debug.Log($"Current Level: {textFileStorage.CurrentLevel}");
                Debug.Log($"Completed Levels Count: {textFileStorage.CompletedLevelsCount}");
                Debug.Log($"Total Stars: {textFileStorage.TotalStars}");
                
                // Test first 5 levels
                for (int i = 1; i <= 5; i++)
                {
                    bool isCompleted = textFileStorage.IsLevelCompleted(i);
                    int stars = textFileStorage.GetLevelStars(i);
                    float bestTime = textFileStorage.GetLevelBestTime(i);
                    int attempts = textFileStorage.GetLevelAttempts(i);
                    
                    Debug.Log($"Level {i}: Completed={isCompleted}, Stars={stars}, BestTime={bestTime}s, Attempts={attempts}");
                }
            }
            else
            {
                Debug.LogError("TextFileGameDataStorage NOT FOUND!");
            }
            
            Debug.Log("=== End Test ===");
        }
        
        
    }
}