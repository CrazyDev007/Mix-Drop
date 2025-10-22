using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CrazyDev007.LevelEditor.Abstraction;

namespace CrazyDev007.LevelEditor.UI
{
    /// <summary>
    /// Main UI component for the level select screen
    /// </summary>
    [UxmlElement]
    public partial class LevelSelectScreen : VisualElement
    {
        private const string USS_CLASS_NAME = "level-select-screen";
        private const int LEVELS_PER_ROW = 5;

        // Child elements
        private LocalizedText _titleLabel;
        private VisualElement _levelsGrid;
        private Button _backButton;

        // Data
        private List<LevelData> _levels;
        private IProgressManager _progressManager;
        private ILocalizationService _localizationService;

        /// <summary>
        /// Event fired when a level is selected
        /// </summary>
        public event System.Action<int> OnLevelSelected;

        /// <summary>
        /// Event fired when back button is clicked
        /// </summary>
        public event System.Action OnBackClicked;

        public LevelSelectScreen()
        {
            // Get services
            _progressManager = ServiceLocator.Container.Resolve<IProgressManager>();
            _localizationService = ServiceLocator.Container.Resolve<ILocalizationService>();

            // Initialize data
            _levels = new List<LevelData>();

            // Create UI structure
            CreateUI();

            // Load level data
            LoadLevelData();
        }

        private void CreateUI()
        {
            // Add USS class
            AddToClassList(USS_CLASS_NAME);

            // Create title
            _titleLabel = new LocalizedText();
            _titleLabel.LocalizationKey = "level_select_title";
            _titleLabel.AddToClassList("level-select-screen__title");
            Add(_titleLabel);

            // Create levels grid
            _levelsGrid = new VisualElement();
            _levelsGrid.AddToClassList("level-select-screen__grid");
            Add(_levelsGrid);

            // Create back button
            _backButton = new Button();
            _backButton.clicked += () => OnBackClicked?.Invoke();
            _backButton.AddToClassList("level-select-screen__back-button");

            var backButtonText = new LocalizedText();
            backButtonText.LocalizationKey = "back_button";
            _backButton.Add(backButtonText);

            Add(_backButton);

            // Set up keyboard navigation
            SetupKeyboardNavigation();
        }

        private void SetupKeyboardNavigation()
        {
            // Make the screen focusable for keyboard navigation
            focusable = true;
            tabIndex = 0;

            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            // Handle global keyboard shortcuts
            if (evt.keyCode == KeyCode.Escape)
            {
                OnBackClicked?.Invoke();
                evt.StopPropagation();
            }
        }

        private void LoadLevelData()
        {
            // Clear existing levels
            _levelsGrid.Clear();
            _levels.Clear();

            // In a real implementation, this would load from a data service
            // For now, create sample level data
            for (int i = 1; i <= 25; i++)
            {
                var levelData = new LevelData
                {
                    LevelNumber = i,
                    IsLocked = i > 5, // First 5 levels unlocked
                    StarsEarned = i <= 3 ? Random.Range(1, 4) : 0, // Some levels have stars
                    IsCompleted = i <= 3
                };
                _levels.Add(levelData);
            }

            // Create level buttons
            CreateLevelButtons();
        }

        private void CreateLevelButtons()
        {
            foreach (var levelData in _levels)
            {
                var levelButton = new LevelButton();
                levelButton.SetLevelData(
                    levelData.LevelNumber,
                    levelData.StarsEarned,
                    levelData.IsLocked,
                    levelData.IsCompleted
                );

                levelButton.OnLevelSelected += HandleLevelSelected;
                levelButton.AddToClassList("level-select-screen__level-button");

                _levelsGrid.Add(levelButton);
            }
        }

        private void HandleLevelSelected(int levelNumber)
        {
            OnLevelSelected?.Invoke(levelNumber);
        }

        /// <summary>
        /// Refreshes the level data display
        /// </summary>
        public void RefreshLevels()
        {
            LoadLevelData();
        }

        /// <summary>
        /// Sets the progress data for all levels
        /// </summary>
        public void SetProgressData(Dictionary<int, LevelProgress> progressData)
        {
            foreach (var level in _levels)
            {
                if (progressData.TryGetValue(level.LevelNumber, out var progress))
                {
                    level.StarsEarned = progress.StarsEarned;
                    level.IsCompleted = progress.IsCompleted;
                    level.IsLocked = progress.IsLocked;
                }
            }

            // Update UI
            UpdateLevelButtons();
        }

        private void UpdateLevelButtons()
        {
            for (int i = 0; i < _levelsGrid.childCount; i++)
            {
                if (_levelsGrid[i] is LevelButton levelButton && i < _levels.Count)
                {
                    var levelData = _levels[i];
                    levelButton.SetLevelData(
                        levelData.LevelNumber,
                        levelData.StarsEarned,
                        levelData.IsLocked,
                        levelData.IsCompleted
                    );
                }
            }
        }

        /// <summary>
        /// Data structure for level information
        /// </summary>
        private class LevelData
        {
            public int LevelNumber;
            public int StarsEarned;
            public bool IsLocked;
            public bool IsCompleted;
        }

        /// <summary>
        /// Data structure for level progress
        /// </summary>
        public class LevelProgress
        {
            public int StarsEarned;
            public bool IsCompleted;
            public bool IsLocked;
        }
    }
}