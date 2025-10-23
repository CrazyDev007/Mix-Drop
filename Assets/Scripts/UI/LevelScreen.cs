using ScreenFlow;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
        private Button settingsButton;
        
        // Data
        private int currentPage = 0;
        private int levelsPerPage = 15; // 3x5 grid
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
            settingsButton = screen.Q<Button>("settings-button");

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
            
            if (settingsButton != null)
            {
                settingsButton.clicked += OnClickSettingsButton;
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
                Debug.LogError("LevelButton template not found in Resources!");
                return;
            }
            
            // Calculate total pages
            totalPages = Mathf.CeilToInt((float)totalLevels / levelsPerPage);
            
            // Add diagnostic logging for initial setup
            Debug.Log("[LevelScreen] SetupScreen - Starting setup");
            Debug.Log($"[LevelScreen] Total Levels: {totalLevels}, Levels Per Page: {levelsPerPage}, Total Pages: {totalPages}");
            
            // Initialize screen
            UpdateHeaderStats();
            SetupLevelGrid();
            UpdatePagination();
            
            // Log initial layout after a delay to ensure UI is fully initialized
            StartCoroutine(LogInitialLayoutInfo());
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
            int completedLevels = PlayerPrefs.GetInt("CompletedLevels", 0);
            int totalStars = PlayerPrefs.GetInt("TotalStars", 0);
            
            // Update progress
            if (progressBarFill != null)

            for (int row = 0; row < 44; row++) // 200 rows for 1000 levels with 5 per row
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
            int completedLevels = PlayerPrefs.GetInt("CompletedLevels", 0);
            int startLevel = currentPage * levelsPerPage + 1;
            int endLevel = Mathf.Min(startLevel + levelsPerPage - 1, totalLevels);
            
            // Add diagnostic logging
            Debug.Log($"[LevelScreen] SetupLevelGrid - Page: {currentPage}, StartLevel: {startLevel}, EndLevel: {endLevel}");
            
            // Create level buttons for each level in this page
            for (int levelNumber = startLevel; levelNumber <= endLevel; levelNumber++)
            {
                var levelButtonElement = levelRowTemplate.CloneTree();
                
                // Get the container element
                var buttonContainer = levelButtonElement.Q<VisualElement>("level-button-container");
                if (buttonContainer != null)
                {
                    // Set up click handler for the container
                    buttonContainer.AddManipulator(new Clickable(() => OnLevelSelected(levelNumber)));
                    
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
                    
                    // Set locked state
                    bool isLocked = levelNumber > completedLevels + 1;
                    var lockIcon = buttonContainer.Q<VisualElement>("lock-icon");
                    if (lockIcon != null)
                    {
                        lockIcon.style.display = isLocked ? DisplayStyle.Flex : DisplayStyle.None;
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
                        completedIcon.style.display = (levelNumber <= completedLevels) ? DisplayStyle.Flex : DisplayStyle.None;
                    }
                    
                    // Update stars (for completed levels)
                    if (levelNumber <= completedLevels)
                    {
                        int starsEarned = PlayerPrefs.GetInt($"Level{levelNumber}Stars", Random.Range(1, 4));
                        for (int i = 1; i <= 3; i++)
                        {
                            var star = buttonContainer.Q<VisualElement>($"star-{i}");
                            if (star != null)
                            {
                                star.RemoveFromClassList("empty-star");
                                star.AddToClassList(i <= starsEarned ? "filled-star" : "empty-star");
                            }
                        }
                    }
                }
                
                levelGridContainer.Add(levelButtonElement);
            }
            
            // Add layout diagnostic logging
            StartCoroutine(LogLayoutInfoAfterFrame());
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
            }
        }

        private void OnClickNextPage()
        {
            if (currentPage < totalPages - 1)
            {
                currentPage++;
                SetupLevelGrid();
                UpdatePagination();
            }
        }

        private void OnLevelSelected(int levelNumber)
        {
            PlayerPrefs.SetInt("Hardness", 0);
            PlayerPrefs.SetInt("ActiveLevel", levelNumber);
            SceneManager.LoadSceneAsync("GamePlay", LoadSceneMode.Single);
        }

        public void OnClickBackButton()
        {
            ScreenManager.Instance.ShowScreen("LobbyScreen");
        }
        
        private void OnClickSettingsButton()
        {
            ScreenManager.Instance.ShowScreen("SettingsScreen");
        }
    }
}