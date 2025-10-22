using ScreenFlow;
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
        private int totalLevels = 1000;
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
            levelRowTemplate = Resources.Load<VisualTreeAsset>("LevelRow");
            if (levelRowTemplate == null)
            {
                Debug.LogError("LevelRow template not found in Resources!");
                return;
            }
            
            // Calculate total pages
            totalPages = Mathf.CeilToInt((float)totalLevels / levelsPerPage);
            
            // Initialize screen
            UpdateHeaderStats();
            SetupLevelGrid();
            UpdatePagination();
        }

        private void UpdateHeaderStats()
        {
            int completedLevels = PlayerPrefs.GetInt("CompletedLevels", 0);
            int totalStars = PlayerPrefs.GetInt("TotalStars", 0);
            
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
            int completedLevels = PlayerPrefs.GetInt("CompletedLevels", 0);
            int startLevel = currentPage * levelsPerPage + 1;
            int endLevel = Mathf.Min(startLevel + levelsPerPage - 1, totalLevels);
            
            // Calculate how many rows we need (5 levels per row)
            int levelsInThisPage = endLevel - startLevel + 1;
            int rowsNeeded = Mathf.CeilToInt((float)levelsInThisPage / 5);
            
            for (int row = 0; row < rowsNeeded; row++)
            {
                var rowElement = levelRowTemplate.CloneTree();
                
                // Setup each button in the row
                for (int col = 0; col < 5; col++)
                {
                    int levelNumber = startLevel + row * 5 + col;
                    if (levelNumber > endLevel)
                    {
                        // Hide extra buttons in the last row
                        var extraButton = rowElement.Q<Button>($"level-{col + 1}");
                        if (extraButton != null)
                        {
                            extraButton.style.display = DisplayStyle.None;
                        }
                        continue;
                    }
                    
                    var button = rowElement.Q<Button>($"level-{col + 1}");
                    if (button != null)
                    {
                        bool isLocked = levelNumber > completedLevels + 1;
                        button.text = $"{levelNumber}";
                        button.SetEnabled(!isLocked);
                        
                        // Remove existing click handlers to avoid duplicates
                        button.clicked -= () => OnLevelSelected(levelNumber);
                        button.clicked += () => OnLevelSelected(levelNumber);
                    }
                }
                
                levelGridContainer.Add(rowElement);
            }
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