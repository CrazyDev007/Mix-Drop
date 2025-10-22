using UnityEngine;
using System.Collections.Generic;

namespace MixDrop.Data
{
    /// <summary>
    /// ScriptableObject containing configuration data for the Level Select Screen.
    /// Defines the layout, behavior, and visual presentation of the level selection interface.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLevelSelectScreenData", menuName = "MixDrop/Level Select Screen Data")]
    public class LevelSelectScreenData : ScriptableObject
    {
        [Header("Screen Configuration")]
        [Tooltip("Title displayed at the top of the level select screen")]
        [SerializeField] private string screenTitle;
        
        [Tooltip("Subtitle displayed below the title")]
        [SerializeField] private string screenSubtitle;
        
        [Tooltip("Background image for the level select screen")]
        [SerializeField] private Sprite backgroundImage;
        
        [Tooltip("Background music to play on the level select screen")]
        [SerializeField] private AudioClip backgroundMusic;
        
        [Tooltip("Background color for the level select screen")]
        [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.2f, 1.0f);
        
        [Header("Grid Layout")]
        [Tooltip("Number of columns in the level grid")]
        [Range(1, 10)]
        [SerializeField] private int gridColumns = 3;
        
        [Tooltip("Number of rows in the level grid")]
        [Range(1, 10)]
        [SerializeField] private int gridRows = 3;
        
        [Tooltip("Spacing between level buttons in pixels")]
        [SerializeField] private float gridSpacing = 20f;
        
        [Tooltip("Padding around the grid in pixels")]
        [SerializeField] private float gridPadding = 50f;
        
        [Header("Level Button Configuration")]
        [Tooltip("Default icon for locked levels")]
        [SerializeField] private Sprite lockedLevelIcon;
        
        [Tooltip("Default icon for unlocked but uncompleted levels")]
        [SerializeField] private Sprite unlockedLevelIcon;
        
        [Tooltip("Default icon for completed levels")]
        [SerializeField] private Sprite completedLevelIcon;
        
        [Tooltip("Star icons for rating display")]
        [SerializeField] private Sprite[] starIcons = new Sprite[2]; // [0] = empty star, [1] = filled star
        
        [Tooltip("Colors for different level states")]
        [SerializeField] private LevelButtonColors levelButtonColors;
        
        [Header("Navigation Configuration")]
        [Tooltip("Whether to show pagination controls")]
        [SerializeField] private bool showPagination = true;
        
        [Tooltip("Maximum number of levels per page")]
        [Range(1, 50)]
        [SerializeField] private int levelsPerPage = 9;
        
        [Tooltip("Number of pages in the level select screen (0 = calculate automatically)")]
        [Range(0, 20)]
        [SerializeField] private int numberOfPages = 0;
        
        [Tooltip("Whether to allow keyboard navigation")]
        [SerializeField] private bool allowKeyboardNavigation = true;
        
        [Tooltip("Whether to allow gamepad navigation")]
        [SerializeField] private bool allowGamepadNavigation = true;
        
        [Header("Animation Configuration")]
        [Tooltip("Duration of button hover animations in seconds")]
        [SerializeField] private float hoverAnimationDuration = 0.2f;
        
        [Tooltip("Duration of button press animations in seconds")]
        [SerializeField] private float pressAnimationDuration = 0.1f;
        
        [Tooltip("Duration of screen transition animations in seconds")]
        [SerializeField] private float transitionAnimationDuration = 0.5f;
        
        [Tooltip("Easing curve for UI animations")]
        [SerializeField] private AnimationCurve animationEasingCurve;
        
        [Tooltip("Scale factor for button hover animation")]
        [SerializeField] private float hoverScaleFactor = 1.1f;
        
        [Tooltip("Scale factor for button press animation")]
        [SerializeField] private float pressScaleFactor = 0.95f;
        
        [Header("Audio Configuration")]
        [Tooltip("Sound played when hovering over a level button")]
        [SerializeField] private AudioClip hoverSound;
        
        [Tooltip("Sound played when clicking a level button")]
        [SerializeField] private AudioClip clickSound;
        
        [Tooltip("Sound played when unlocking a level")]
        [SerializeField] private AudioClip unlockSound;
        
        [Tooltip("Sound played when completing a level")]
        [SerializeField] private AudioClip completeSound;
        
        [Tooltip("Sound played when navigating between pages")]
        [SerializeField] private AudioClip pageNavigationSound;
        
        [Header("Accessibility Configuration")]
        [Tooltip("Whether to enable screen reader support")]
        [SerializeField] private bool enableScreenReader = true;
        
        [Tooltip("Whether to enable high contrast mode")]
        [SerializeField] private bool enableHighContrastMode = false;
        
        [Tooltip("Whether to enable large text mode")]
        [SerializeField] private bool enableLargeTextMode = false;
        
        [Tooltip("Colors for high contrast mode")]
        [SerializeField] private HighContrastColors highContrastColors;
        
        [Header("Text Configuration")]
        [Tooltip("Font style for level names")]
        [SerializeField] private FontStyle levelNameFontStyle = FontStyle.Bold;
        
        [Tooltip("Font size for level numbers")]
        [SerializeField] private int levelNumberFontSize = 24;
        
        [Tooltip("Font size for level names")]
        [SerializeField] private int levelNameFontSize = 16;
        
        [Tooltip("Font size for star count")]
        [SerializeField] private int starCountFontSize = 20;
        
        [Tooltip("Font size for locked text")]
        [SerializeField] private int lockedTextFontSize = 14;
        
        [Tooltip("Font size for new text")]
        [SerializeField] private int newTextFontSize = 14;
        
        [Header("Localization Configuration")]
        [Tooltip("Localization key for screen title")]
        [SerializeField] private string screenTitleLocalizationKey = "level_select.title";
        
        [Tooltip("Localization key for locked level text")]
        [SerializeField] private string lockedLevelLocalizationKey = "level_select.locked";
        
        [Tooltip("Localization key for new level text")]
        [SerializeField] private string newLevelLocalizationKey = "level_select.new";
        
        [Tooltip("Localization key for completed level text")]
        [SerializeField] private string completedLevelLocalizationKey = "level_select.completed";
        
        [Tooltip("Localization key for page number")]
        [SerializeField] private string pageNumberLocalizationKey = "level_select.page";
        
        [Tooltip("Localization key for previous page button")]
        [SerializeField] private string previousPageLocalizationKey = "level_select.previous";
        
        [Tooltip("Localization key for next page button")]
        [SerializeField] private string nextPageLocalizationKey = "level_select.next";
        
        [Header("Level Collection")]
        [Tooltip("List of all available levels")]
        [SerializeField] private List<LevelData> levels = new List<LevelData>();
        
        #region Nested Classes
        
        /// <summary>
        /// Color configuration for level buttons in different states
        /// </summary>
        [System.Serializable]
        public class LevelButtonColors
        {
            [Tooltip("Background color for locked levels")]
            [SerializeField] private Color lockedBackgroundColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            
            [Tooltip("Background color for unlocked levels")]
            [SerializeField] private Color unlockedBackgroundColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            
            [Tooltip("Background color for completed levels")]
            [SerializeField] private Color completedBackgroundColor = new Color(0.2f, 0.8f, 0.2f, 1f);
            
            [Tooltip("Text color for locked levels")]
            [SerializeField] private Color lockedTextColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            
            [Tooltip("Text color for unlocked levels")]
            [SerializeField] private Color unlockedTextColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            [Tooltip("Text color for completed levels")]
            [SerializeField] private Color completedTextColor = new Color(1f, 1f, 1f, 1f);
            
            [Tooltip("Border color for locked levels")]
            [SerializeField] private Color lockedBorderColor = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            [Tooltip("Border color for unlocked levels")]
            [SerializeField] private Color unlockedBorderColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            
            [Tooltip("Border color for completed levels")]
            [SerializeField] private Color completedBorderColor = new Color(0.1f, 0.5f, 0.1f, 1f);
            
            [Tooltip("Normal background color")]
            [SerializeField] private Color normalColor = new Color(0.2f, 0.2f, 0.3f, 1.0f);
            
            [Tooltip("Hover background color")]
            [SerializeField] private Color hoverColor = new Color(0.3f, 0.3f, 0.4f, 1.0f);
            
            [Tooltip("Pressed background color")]
            [SerializeField] private Color pressedColor = new Color(0.15f, 0.15f, 0.25f, 1.0f);
            
            [Tooltip("Disabled background color")]
            [SerializeField] private Color disabledColor = new Color(0.1f, 0.1f, 0.15f, 1.0f);
            
            [Tooltip("Selected background color")]
            [SerializeField] private Color selectedColor = new Color(0.25f, 0.35f, 0.5f, 1.0f);
            
            [Tooltip("Locked background color")]
            [SerializeField] private Color lockedColor = new Color(0.05f, 0.05f, 0.1f, 1.0f);
            
            [Tooltip("New level background color")]
            [SerializeField] private Color newLevelColor = new Color(0.4f, 0.3f, 0.1f, 1.0f);
            
            [Tooltip("Completed background color")]
            [SerializeField] private Color completedColor = new Color(0.1f, 0.4f, 0.2f, 1.0f);
            
            [Tooltip("Border color")]
            [SerializeField] private Color borderColor = new Color(0.4f, 0.4f, 0.5f, 1.0f);
            
            [Tooltip("Normal text color")]
            [SerializeField] private Color normalTextColor = Color.white;
            
            [Tooltip("Disabled text color")]
            [SerializeField] private Color disabledTextColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            
            [Tooltip("New level text color")]
            [SerializeField] private Color newLevelTextColor = new Color(1.0f, 0.9f, 0.6f, 1.0f);
            
            [Tooltip("Empty star color")]
            [SerializeField] private Color emptyStarColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            
            [Tooltip("Filled star color")]
            [SerializeField] private Color filledStarColor = new Color(1.0f, 0.8f, 0.2f, 1.0f);
            
            #region Properties
            
            public Color LockedBackgroundColor => lockedBackgroundColor;
            public Color UnlockedBackgroundColor => unlockedBackgroundColor;
            public Color CompletedBackgroundColor => completedBackgroundColor;
            public Color LockedTextColor
            {
                get => lockedTextColor;
                set => lockedTextColor = value;
            }
            
            public Color UnlockedTextColor => unlockedTextColor;
            
            public Color CompletedTextColor
            {
                get => completedTextColor;
                set => completedTextColor = value;
            }
            public Color LockedBorderColor => lockedBorderColor;
            public Color UnlockedBorderColor => unlockedBorderColor;
            public Color CompletedBorderColor => completedBorderColor;
            
            public Color NormalColor
            {
                get => normalColor;
                set => normalColor = value;
            }
            
            public Color HoverColor
            {
                get => hoverColor;
                set => hoverColor = value;
            }
            
            public Color PressedColor
            {
                get => pressedColor;
                set => pressedColor = value;
            }
            
            public Color DisabledColor
            {
                get => disabledColor;
                set => disabledColor = value;
            }
            
            public Color SelectedColor
            {
                get => selectedColor;
                set => selectedColor = value;
            }
            
            public Color LockedColor
            {
                get => lockedColor;
                set => lockedColor = value;
            }
            
            public Color NewLevelColor
            {
                get => newLevelColor;
                set => newLevelColor = value;
            }
            
            public Color CompletedColor
            {
                get => completedColor;
                set => completedColor = value;
            }
            
            public Color BorderColor
            {
                get => borderColor;
                set => borderColor = value;
            }
            
            public Color NormalTextColor
            {
                get => normalTextColor;
                set => normalTextColor = value;
            }
            
            public Color DisabledTextColor
            {
                get => disabledTextColor;
                set => disabledTextColor = value;
            }
            
            public Color NewLevelTextColor
            {
                get => newLevelTextColor;
                set => newLevelTextColor = value;
            }
            
            public Color EmptyStarColor
            {
                get => emptyStarColor;
                set => emptyStarColor = value;
            }
            
            public Color FilledStarColor
            {
                get => filledStarColor;
                set => filledStarColor = value;
            }
            
            #endregion
        }
        
        /// <summary>
        /// Color configuration for high contrast mode
        /// </summary>
        [System.Serializable]
        public class HighContrastColors
        {
            [Tooltip("Primary background color")]
            [SerializeField] private Color primaryBackgroundColor = Color.black;
            
            [Tooltip("Secondary background color")]
            [SerializeField] private Color secondaryBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
            
            [Tooltip("Primary text color")]
            [SerializeField] private Color primaryTextColor = Color.white;
            
            [Tooltip("Secondary text color")]
            [SerializeField] private Color secondaryTextColor = new Color(0.9f, 0.9f, 0.9f, 1f);
            
            [Tooltip("Accent color for interactive elements")]
            [SerializeField] private Color accentColor = new Color(1f, 1f, 0f, 1f);
            
            [Tooltip("Color for locked elements")]
            [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
            
            [Tooltip("Color for completed elements")]
            [SerializeField] private Color completedColor = new Color(0f, 1f, 0f, 1f);
            
            [Tooltip("Background color")]
            [SerializeField] private Color backgroundColor = Color.black;
            
            [Tooltip("Level button color")]
            [SerializeField] private Color levelButtonColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
            
            [Tooltip("Text color")]
            [SerializeField] private Color textColor = Color.white;
            
            [Tooltip("Border color")]
            [SerializeField] private Color borderColor = Color.white;
            
            [Tooltip("Star color")]
            [SerializeField] private Color starColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
            
            [Tooltip("New level color")]
            [SerializeField] private Color newLevelColor = new Color(1.0f, 0.6f, 0.0f, 1.0f);
            
            #region Properties
            
            public Color PrimaryBackgroundColor => primaryBackgroundColor;
            public Color SecondaryBackgroundColor => secondaryBackgroundColor;
            public Color PrimaryTextColor => primaryTextColor;
            public Color SecondaryTextColor => secondaryTextColor;
            public Color AccentColor => accentColor;
            
            public Color LockedColor
            {
                get => lockedColor;
                set => lockedColor = value;
            }
            
            public Color CompletedColor
            {
                get => completedColor;
                set => completedColor = value;
            }
            
            public Color BackgroundColor
            {
                get => backgroundColor;
                set => backgroundColor = value;
            }
            
            public Color LevelButtonColor
            {
                get => levelButtonColor;
                set => levelButtonColor = value;
            }
            
            public Color TextColor
            {
                get => textColor;
                set => textColor = value;
            }
            
            public Color BorderColor
            {
                get => borderColor;
                set => borderColor = value;
            }
            
            public Color StarColor
            {
                get => starColor;
                set => starColor = value;
            }
            
            public Color NewLevelColor
            {
                get => newLevelColor;
                set => newLevelColor = value;
            }
            
            #endregion
        }
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Title displayed at the top of the level select screen
        /// </summary>
        public string ScreenTitle
        {
            get => screenTitle;
            set => screenTitle = value;
        }
        
        /// <summary>
        /// Subtitle displayed below the title
        /// </summary>
        public string ScreenSubtitle
        {
            get => screenSubtitle;
            set => screenSubtitle = value;
        }
        
        /// <summary>
        /// Background image for the level select screen
        /// </summary>
        public Sprite BackgroundImage
        {
            get => backgroundImage;
            set => backgroundImage = value;
        }
        
        /// <summary>
        /// Background music to play on the level select screen
        /// </summary>
        public AudioClip BackgroundMusic
        {
            get => backgroundMusic;
            set => backgroundMusic = value;
        }
        
        /// <summary>
        /// Background color for the level select screen
        /// </summary>
        public Color BackgroundColor
        {
            get => backgroundColor;
            set => backgroundColor = value;
        }
        
        /// <summary>
        /// Number of columns in the level grid
        /// </summary>
        public int GridColumns
        {
            get => gridColumns;
            set => gridColumns = value;
        }
        
        /// <summary>
        /// Number of rows in the level grid
        /// </summary>
        public int GridRows
        {
            get => gridRows;
            set => gridRows = value;
        }
        
        /// <summary>
        /// Spacing between level buttons in pixels
        /// </summary>
        public float GridSpacing
        {
            get => gridSpacing;
            set => gridSpacing = value;
        }
        
        /// <summary>
        /// Padding around the grid in pixels
        /// </summary>
        public float GridPadding
        {
            get => gridPadding;
            set => gridPadding = value;
        }
        
        /// <summary>
        /// Default icon for locked levels
        /// </summary>
        public Sprite LockedLevelIcon
        {
            get => lockedLevelIcon;
            set => lockedLevelIcon = value;
        }
        
        /// <summary>
        /// Default icon for unlocked but uncompleted levels
        /// </summary>
        public Sprite UnlockedLevelIcon
        {
            get => unlockedLevelIcon;
            set => unlockedLevelIcon = value;
        }
        
        /// <summary>
        /// Default icon for completed levels
        /// </summary>
        public Sprite CompletedLevelIcon
        {
            get => completedLevelIcon;
            set => completedLevelIcon = value;
        }
        
        /// <summary>
        /// Star icons for rating display
        /// </summary>
        public Sprite[] StarIcons
        {
            get => starIcons;
            set => starIcons = value;
        }
        
        /// <summary>
        /// Colors for different level states
        /// </summary>
        public LevelButtonColors ButtonColors
        {
            get => levelButtonColors;
            set => levelButtonColors = value;
        }
        
        /// <summary>
        /// Whether to show pagination controls
        /// </summary>
        public bool ShowPagination
        {
            get => showPagination;
            set => showPagination = value;
        }
        
        /// <summary>
        /// Maximum number of levels per page
        /// </summary>
        public int LevelsPerPage
        {
            get => levelsPerPage;
            set => levelsPerPage = value;
        }
        
        /// <summary>
        /// Number of pages in the level select screen (0 = calculate automatically)
        /// </summary>
        public int NumberOfPages
        {
            get => numberOfPages;
            set => numberOfPages = value;
        }
        
        /// <summary>
        /// Whether to allow keyboard navigation
        /// </summary>
        public bool AllowKeyboardNavigation
        {
            get => allowKeyboardNavigation;
            set => allowKeyboardNavigation = value;
        }
        
        /// <summary>
        /// Whether to allow gamepad navigation
        /// </summary>
        public bool AllowGamepadNavigation
        {
            get => allowGamepadNavigation;
            set => allowGamepadNavigation = value;
        }
        
        /// <summary>
        /// Duration of button hover animations in seconds
        /// </summary>
        public float HoverAnimationDuration
        {
            get => hoverAnimationDuration;
            set => hoverAnimationDuration = value;
        }
        
        /// <summary>
        /// Duration of button press animations in seconds
        /// </summary>
        public float PressAnimationDuration
        {
            get => pressAnimationDuration;
            set => pressAnimationDuration = value;
        }
        
        /// <summary>
        /// Duration of screen transition animations in seconds
        /// </summary>
        public float TransitionAnimationDuration
        {
            get => transitionAnimationDuration;
            set => transitionAnimationDuration = value;
        }
        
        /// <summary>
        /// Easing curve for UI animations
        /// </summary>
        public AnimationCurve AnimationEasingCurve
        {
            get => animationEasingCurve;
            set => animationEasingCurve = value;
        }
        
        /// <summary>
        /// Scale factor for button hover animation
        /// </summary>
        public float HoverScaleFactor
        {
            get => hoverScaleFactor;
            set => hoverScaleFactor = value;
        }
        
        /// <summary>
        /// Scale factor for button press animation
        /// </summary>
        public float PressScaleFactor
        {
            get => pressScaleFactor;
            set => pressScaleFactor = value;
        }
        
        /// <summary>
        /// Sound played when hovering over a level button
        /// </summary>
        public AudioClip HoverSound
        {
            get => hoverSound;
            set => hoverSound = value;
        }
        
        /// <summary>
        /// Sound played when clicking a level button
        /// </summary>
        public AudioClip ClickSound
        {
            get => clickSound;
            set => clickSound = value;
        }
        
        /// <summary>
        /// Sound played when unlocking a level
        /// </summary>
        public AudioClip UnlockSound
        {
            get => unlockSound;
            set => unlockSound = value;
        }
        
        /// <summary>
        /// Sound played when completing a level
        /// </summary>
        public AudioClip CompleteSound
        {
            get => completeSound;
            set => completeSound = value;
        }
        
        /// <summary>
        /// Sound played when navigating between pages
        /// </summary>
        public AudioClip PageNavigationSound
        {
            get => pageNavigationSound;
            set => pageNavigationSound = value;
        }
        
        /// <summary>
        /// Whether to enable screen reader support
        /// </summary>
        public bool EnableScreenReader
        {
            get => enableScreenReader;
            set => enableScreenReader = value;
        }
        
        /// <summary>
        /// Whether to enable high contrast mode
        /// </summary>
        public bool EnableHighContrastMode
        {
            get => enableHighContrastMode;
            set => enableHighContrastMode = value;
        }
        
        /// <summary>
        /// Whether to enable large text mode
        /// </summary>
        public bool EnableLargeTextMode
        {
            get => enableLargeTextMode;
            set => enableLargeTextMode = value;
        }
        
        /// <summary>
        /// Colors for high contrast mode
        /// </summary>
        public HighContrastColors ContrastColors
        {
            get => highContrastColors;
            set => highContrastColors = value;
        }
        
        /// <summary>
        /// Font style for level names
        /// </summary>
        public FontStyle LevelNameFontStyle
        {
            get => levelNameFontStyle;
            set => levelNameFontStyle = value;
        }
        
        /// <summary>
        /// Font size for level numbers
        /// </summary>
        public int LevelNumberFontSize
        {
            get => levelNumberFontSize;
            set => levelNumberFontSize = value;
        }
        
        /// <summary>
        /// Font size for level names
        /// </summary>
        public int LevelNameFontSize
        {
            get => levelNameFontSize;
            set => levelNameFontSize = value;
        }
        
        /// <summary>
        /// Font size for star count
        /// </summary>
        public int StarCountFontSize
        {
            get => starCountFontSize;
            set => starCountFontSize = value;
        }
        
        /// <summary>
        /// Font size for locked text
        /// </summary>
        public int LockedTextFontSize
        {
            get => lockedTextFontSize;
            set => lockedTextFontSize = value;
        }
        
        /// <summary>
        /// Font size for new text
        /// </summary>
        public int NewTextFontSize
        {
            get => newTextFontSize;
            set => newTextFontSize = value;
        }
        
        /// <summary>
        /// List of all available levels
        /// </summary>
        public List<LevelData> Levels
        {
            get => levels;
            set => levels = value;
        }
        
        /// <summary>
        /// Localization key for screen title
        /// </summary>
        public string ScreenTitleLocalizationKey
        {
            get => screenTitleLocalizationKey;
            set => screenTitleLocalizationKey = value;
        }
        
        /// <summary>
        /// Localization key for locked level text
        /// </summary>
        public string LockedLevelLocalizationKey
        {
            get => lockedLevelLocalizationKey;
            set => lockedLevelLocalizationKey = value;
        }
        
        /// <summary>
        /// Localization key for new level text
        /// </summary>
        public string NewLevelLocalizationKey
        {
            get => newLevelLocalizationKey;
            set => newLevelLocalizationKey = value;
        }
        
        /// <summary>
        /// Localization key for completed level text
        /// </summary>
        public string CompletedLevelLocalizationKey
        {
            get => completedLevelLocalizationKey;
            set => completedLevelLocalizationKey = value;
        }
        
        /// <summary>
        /// Localization key for page number
        /// </summary>
        public string PageNumberLocalizationKey
        {
            get => pageNumberLocalizationKey;
            set => pageNumberLocalizationKey = value;
        }
        
        /// <summary>
        /// Localization key for previous page button
        /// </summary>
        public string PreviousPageLocalizationKey
        {
            get => previousPageLocalizationKey;
            set => previousPageLocalizationKey = value;
        }
        
        /// <summary>
        /// Localization key for next page button
        /// </summary>
        public string NextPageLocalizationKey
        {
            get => nextPageLocalizationKey;
            set => nextPageLocalizationKey = value;
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Gets the total number of levels
        /// </summary>
        /// <returns>Total number of levels</returns>
        public int GetTotalLevels()
        {
            return levels.Count;
        }
        
        /// <summary>
        /// Gets the number of pages needed to display all levels
        /// </summary>
        /// <returns>Number of pages</returns>
        public int GetPageCount()
        {
            // If numberOfPages is explicitly set, use that value
            if (numberOfPages > 0)
                return numberOfPages;
                
            // Otherwise calculate based on levels count
            if (levels.Count == 0)
                return 0;
                
            return Mathf.CeilToInt((float)levels.Count / levelsPerPage);
        }
        
        /// <summary>
        /// Gets the levels for a specific page
        /// </summary>
        /// <param name="pageIndex">Index of the page (0-based)</param>
        /// <returns>List of levels for the specified page</returns>
        public List<LevelData> GetLevelsForPage(int pageIndex)
        {
            List<LevelData> pageLevels = new List<LevelData>();
            
            int startIndex = pageIndex * levelsPerPage;
            int endIndex = Mathf.Min(startIndex + levelsPerPage, levels.Count);
            
            for (int i = startIndex; i < endIndex; i++)
            {
                pageLevels.Add(levels[i]);
            }
            
            return pageLevels;
        }
        
        /// <summary>
        /// Gets a level by its ID
        /// </summary>
        /// <param name="levelId">ID of the level to find</param>
        /// <returns>LevelData if found, null otherwise</returns>
        public LevelData GetLevelById(string levelId)
        {
            return levels.Find(level => level.LevelId == levelId);
        }
        
        /// <summary>
        /// Gets a level by its index in the list
        /// </summary>
        /// <param name="index">Index of the level</param>
        /// <returns>LevelData at the specified index</returns>
        public LevelData GetLevelByIndex(int index)
        {
            if (index < 0 || index >= levels.Count)
                return null;
                
            return levels[index];
        }
        
        /// <summary>
        /// Adds a new level to the collection
        /// </summary>
        /// <param name="level">LevelData to add</param>
        public void AddLevel(LevelData level)
        {
            if (level != null && !levels.Contains(level))
            {
                levels.Add(level);
                
                // Mark as dirty to save changes
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
                #endif
            }
        }
        
        /// <summary>
        /// Removes a level from the collection
        /// </summary>
        /// <param name="level">LevelData to remove</param>
        public void RemoveLevel(LevelData level)
        {
            if (level != null && levels.Contains(level))
            {
                levels.Remove(level);
                
                // Mark as dirty to save changes
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
                #endif
            }
        }
        
        /// <summary>
        /// Reorders levels based on their IDs
        /// </summary>
        public void SortLevelsById()
        {
            levels.Sort((a, b) => string.Compare(a.LevelId, b.LevelId, System.StringComparison.Ordinal));
            
            // Mark as dirty to save changes
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        /// <summary>
        /// Reorders levels based on their difficulty
        /// </summary>
        public void SortLevelsByDifficulty()
        {
            levels.Sort((a, b) => a.Difficulty.CompareTo(b.Difficulty));
            
            // Mark as dirty to save changes
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        /// <summary>
        /// Validates the level select screen data to ensure all required fields are set
        /// </summary>
        /// <returns>True if the data is valid</returns>
        public bool Validate()
        {
            // Check required fields
            if (string.IsNullOrEmpty(screenTitle))
            {
                Debug.LogError($"Level select screen validation failed: Screen title is not set for {name}");
                return false;
            }
            
            // Check grid configuration
            if (gridColumns < 1 || gridColumns > 10)
            {
                Debug.LogError($"Level select screen validation failed: Grid columns must be between 1 and 10 for {name}");
                return false;
            }
            
            if (gridRows < 1 || gridRows > 10)
            {
                Debug.LogError($"Level select screen validation failed: Grid rows must be between 1 and 10 for {name}");
                return false;
            }
            
            if (gridSpacing < 0)
            {
                Debug.LogError($"Level select screen validation failed: Grid spacing must be non-negative for {name}");
                return false;
            }
            
            if (gridPadding < 0)
            {
                Debug.LogError($"Level select screen validation failed: Grid padding must be non-negative for {name}");
                return false;
            }
            
            // Check pagination configuration
            if (levelsPerPage < 1 || levelsPerPage > 50)
            {
                Debug.LogError($"Level select screen validation failed: Levels per page must be between 1 and 50 for {name}");
                return false;
            }
            
            if (numberOfPages < 0)
            {
                Debug.LogError($"Level select screen validation failed: Number of pages must be non-negative for {name}");
                return false;
            }
            
            // Check animation durations
            if (hoverAnimationDuration < 0)
            {
                Debug.LogError($"Level select screen validation failed: Hover animation duration must be non-negative for {name}");
                return false;
            }
            
            if (pressAnimationDuration < 0)
            {
                Debug.LogError($"Level select screen validation failed: Press animation duration must be non-negative for {name}");
                return false;
            }
            
            if (transitionAnimationDuration < 0)
            {
                Debug.LogError($"Level select screen validation failed: Transition animation duration must be non-negative for {name}");
                return false;
            }
            
            // Check star icons
            if (starIcons == null || starIcons.Length != 2)
            {
                Debug.LogError($"Level select screen validation failed: Star icons array must have exactly 2 elements for {name}");
                return false;
            }
            
            if (starIcons[0] == null || starIcons[1] == null)
            {
                Debug.LogError($"Level select screen validation failed: Star icons must not be null for {name}");
                return false;
            }
            
            // Validate all levels
            foreach (var level in levels)
            {
                if (!level.Validate())
                {
                    return false;
                }
            }
            
            return true;
        }
        
        #endregion
        
        #region Editor Methods
        
#if UNITY_EDITOR
        /// <summary>
        /// Called when the script is loaded or a value is changed in the inspector
        /// </summary>
        private void OnValidate()
        {
            // Initialize animation curve if not set
            if (animationEasingCurve == null)
            {
                animationEasingCurve = new AnimationCurve();
                animationEasingCurve.AddKey(0f, 0f);
                animationEasingCurve.AddKey(1f, 1f);
            }
            
            // Clamp values to valid ranges
            gridColumns = Mathf.Clamp(gridColumns, 1, 10);
            gridRows = Mathf.Clamp(gridRows, 1, 10);
            gridSpacing = Mathf.Max(0, gridSpacing);
            gridPadding = Mathf.Max(0, gridPadding);
            levelsPerPage = Mathf.Clamp(levelsPerPage, 1, 50);
            numberOfPages = Mathf.Max(0, numberOfPages);
            hoverAnimationDuration = Mathf.Max(0, hoverAnimationDuration);
            pressAnimationDuration = Mathf.Max(0, pressAnimationDuration);
            transitionAnimationDuration = Mathf.Max(0, transitionAnimationDuration);
            
            // Ensure star icons array has correct size
            if (starIcons == null)
            {
                starIcons = new Sprite[2];
            }
            else if (starIcons.Length != 2)
            {
                System.Array.Resize(ref starIcons, 2);
            }
            
            // Validate data
            Validate();
        }
        
        /// <summary>
        /// Creates a menu item to validate all level select screen data assets
        /// </summary>
        [UnityEditor.MenuItem("MixDrop/Validate All Level Select Screen Data")]
        private static void ValidateAllLevelSelectScreenData()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:LevelSelectScreenData");
            int validCount = 0;
            int invalidCount = 0;
            
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                LevelSelectScreenData screenData = UnityEditor.AssetDatabase.LoadAssetAtPath<LevelSelectScreenData>(path);
                
                if (screenData.Validate())
                {
                    validCount++;
                }
                else
                {
                    invalidCount++;
                }
            }
            
            UnityEditor.EditorUtility.DisplayDialog(
                "Validation Results",
                $"Validation complete.\n\nValid screen data: {validCount}\nInvalid screen data: {invalidCount}",
                "OK"
            );
        }
#endif
        
        #endregion
    }
}