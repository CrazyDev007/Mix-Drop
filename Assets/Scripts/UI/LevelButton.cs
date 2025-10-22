using System;
using UnityEngine;
using UnityEngine.UIElements;
using CrazyDev007.LevelEditor.Abstraction;

namespace CrazyDev007.LevelEditor.UI
{
    /// <summary>
    /// UI component representing a level button in the level select screen
    /// </summary>
    [UxmlElement]
    public partial class LevelButton : VisualElement
    {
        private const string USS_CLASS_NAME = "level-button";
        private const string USS_LOCKED_CLASS = "level-button--locked";
        private const string USS_COMPLETED_CLASS = "level-button--completed";
        private const string USS_HOVER_CLASS = "level-button--hover";
        private const string USS_PRESSED_CLASS = "level-button--pressed";
        private const string USS_LOADING_CLASS = "loading";

        // Child elements
        private Label _levelNumberLabel;
        private VisualElement _starsContainer;
        private VisualElement[] _starElements;
        private VisualElement _loadingOverlay;
        private VisualElement _loadingSpinner;

        // Data
        private int _levelNumber;
        private int _starsEarned;
        private bool _isLocked;
        private bool _isCompleted;
        private bool _isLoading;

        // Services
        private ILocalizationService _localizationService;

        /// <summary>
        /// Event fired when the level button is clicked
        /// </summary>
        public event Action<int> OnLevelSelected;

        /// <summary>
        /// The level number this button represents
        /// </summary>
        [UxmlAttribute]
        public int LevelNumber
        {
            get => _levelNumber;
            set
            {
                _levelNumber = value;
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Number of stars earned for this level (0-3)
        /// </summary>
        [UxmlAttribute]
        public int StarsEarned
        {
            get => _starsEarned;
            set
            {
                _starsEarned = Mathf.Clamp(value, 0, 3);
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Whether this level is locked
        /// </summary>
        [UxmlAttribute]
        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                _isLocked = value;
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Whether this level is completed
        /// </summary>
        [UxmlAttribute]
        public bool IsCompleted
        {
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                UpdateDisplay();
            }
        }

        /// <summary>
        /// Whether this level is in a loading state
        /// </summary>
        [UxmlAttribute]
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                UpdateDisplay();
            }
        }

        public LevelButton()
        {
            // Get services
            _localizationService = ServiceLocator.Container.Resolve<ILocalizationService>();

            // Initialize data
            _levelNumber = 1;
            _starsEarned = 0;
            _isLocked = false;
            _isCompleted = false;
            _isLoading = false;

            // Create UI structure
            CreateUI();

            // Register callbacks
            RegisterCallbacks();

            // Initial display update
            UpdateDisplay();
        }

        private void CreateUI()
        {
            // Add USS class
            AddToClassList(USS_CLASS_NAME);

            // Create level number label
            _levelNumberLabel = new Label();
            _levelNumberLabel.AddToClassList("level-button__number");
            Add(_levelNumberLabel);

            // Create stars container
            _starsContainer = new VisualElement();
            _starsContainer.AddToClassList("level-button__stars");
            Add(_starsContainer);

            // Create star elements
            _starElements = new VisualElement[3];
            for (int i = 0; i < 3; i++)
            {
                _starElements[i] = new VisualElement();
                _starElements[i].AddToClassList("level-button__star");
                _starsContainer.Add(_starElements[i]);
            }

            // Create loading state elements
            _loadingOverlay = new VisualElement();
            _loadingOverlay.AddToClassList("loading-overlay");
            Add(_loadingOverlay);

            _loadingSpinner = new VisualElement();
            _loadingSpinner.AddToClassList("loading-spinner");
            Add(_loadingSpinner);
        }

        private void RegisterCallbacks()
        {
            // Mouse events
            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
            RegisterCallback<ClickEvent>(OnClick);

            // Keyboard events
            RegisterCallback<KeyDownEvent>(OnKeyDown);

            // Focus events for accessibility
            RegisterCallback<FocusInEvent>(OnFocusIn);
            RegisterCallback<FocusOutEvent>(OnFocusOut);

            // Make focusable
            focusable = true;
            tabIndex = 0;
        }

        private void OnMouseEnter(MouseEnterEvent evt)
        {
            if (!_isLocked && !_isLoading)
            {
                AddToClassList(USS_HOVER_CLASS);
            }
        }

        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            RemoveFromClassList(USS_HOVER_CLASS);
            RemoveFromClassList(USS_PRESSED_CLASS);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (!_isLocked && !_isLoading)
            {
                AddToClassList(USS_PRESSED_CLASS);
            }
        }

        private void OnMouseUp(MouseUpEvent evt)
        {
            RemoveFromClassList(USS_PRESSED_CLASS);
        }

        private void OnClick(ClickEvent evt)
        {
            if (!_isLocked && !_isLoading)
            {
                OnLevelSelected?.Invoke(_levelNumber);
            }
            else if (_isLocked)
            {
                // Show tooltip for locked level
                ShowLockedTooltip();
            }
        }

        private void OnKeyDown(KeyDownEvent evt)
        {
            if ((evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.Space) && !_isLocked && !_isLoading)
            {
                OnLevelSelected?.Invoke(_levelNumber);
                evt.StopPropagation();
            }
        }

        private void OnFocusIn(FocusInEvent evt)
        {
            if (!_isLocked && !_isLoading)
            {
                AddToClassList(USS_HOVER_CLASS);
            }
        }

        private void OnFocusOut(FocusOutEvent evt)
        {
            RemoveFromClassList(USS_HOVER_CLASS);
        }

        private void UpdateDisplay()
        {
            // Update level number
            _levelNumberLabel.text = _levelNumber.ToString();

            // Update classes
            EnableInClassList(USS_LOCKED_CLASS, _isLocked);
            EnableInClassList(USS_COMPLETED_CLASS, _isCompleted);
            EnableInClassList(USS_LOADING_CLASS, _isLoading);

            // Update stars
            for (int i = 0; i < 3; i++)
            {
                bool isEarned = i < _starsEarned;
                _starElements[i].EnableInClassList("level-button__star--earned", isEarned);
                _starElements[i].EnableInClassList("level-button__star--empty", !isEarned);
            }

            // Update loading state visibility
            if (_loadingOverlay != null)
            {
                _loadingOverlay.style.display = _isLoading ? DisplayStyle.Flex : DisplayStyle.None;
            }

            if (_loadingSpinner != null)
            {
                _loadingSpinner.style.display = _isLoading ? DisplayStyle.Flex : DisplayStyle.None;
            }

            // Update accessibility
            UpdateAccessibility();
        }

        private void UpdateAccessibility()
        {
            string label;
            if (_isLocked)
            {
                label = _localizationService.GetText("level_locked_tooltip", _levelNumber - 1);
            }
            else
            {
                label = _localizationService.GetText("level_number", _levelNumber);
                if (_isCompleted)
                {
                    label += " - " + _localizationService.GetText("level_completed");
                }
                if (_starsEarned > 0)
                {
                    label += " - " + _localizationService.GetText("stars_earned", _starsEarned);
                }
            }

            // Set accessibility attributes (Unity UI Toolkit supports aria-label)
            this.tooltip = label;
        }

        private void ShowLockedTooltip()
        {
            // Create tooltip (simplified implementation)
            // In a full implementation, this would use a proper tooltip system
            Debug.Log(_localizationService.GetText("level_locked_tooltip", _levelNumber - 1));
        }

        /// <summary>
        /// Sets all level data at once
        /// </summary>
        public void SetLevelData(int levelNumber, int starsEarned, bool isLocked, bool isCompleted)
        {
            _levelNumber = levelNumber;
            _starsEarned = Mathf.Clamp(starsEarned, 0, 3);
            _isLocked = isLocked;
            _isCompleted = isCompleted;
            UpdateDisplay();
        }

        /// <summary>
        /// Sets all level data at once including loading state
        /// </summary>
        public void SetLevelData(int levelNumber, int starsEarned, bool isLocked, bool isCompleted, bool isLoading)
        {
            _levelNumber = levelNumber;
            _starsEarned = Mathf.Clamp(starsEarned, 0, 3);
            _isLocked = isLocked;
            _isCompleted = isCompleted;
            _isLoading = isLoading;
            UpdateDisplay();
        }
    }
}