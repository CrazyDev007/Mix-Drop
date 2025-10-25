using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace MixDrop.Data
{
    /// <summary>
    /// Creates sample level data assets for testing and demonstration purposes.
    /// This is an editor-only script that generates sample LevelData and LevelSelectScreenData assets.
    /// </summary>
    public class SampleLevelDataCreator : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Path where the sample assets will be created")]
        [SerializeField] private string assetsPath = "Assets/Resources/Data/Samples";
        
        [Tooltip("Number of sample levels to create")]
        [Range(1, 50)]
        [SerializeField] private int numberOfLevels = 15;
        
        [Tooltip("Number of pages in the level select screen")]
        [Range(1, 5)]
        [SerializeField] private int numberOfPages = 3;
        
        [Tooltip("Whether to overwrite existing assets")]
        [SerializeField] private bool overwriteExistingAssets = false;
        
        [Header("Level Configuration")]
        [Tooltip("Base difficulty for levels")]
        [Range(1, 10)]
        [SerializeField] private int baseDifficulty = 3;
        
        [Tooltip("Difficulty increase per level")]
        [Range(0, 2)]
        [SerializeField] private float difficultyIncrease = 0.2f;
        
        [Tooltip("Base time limit for levels in seconds")]
        [SerializeField] private int baseTimeLimit = 120;
        
        [Tooltip("Time limit increase per level in seconds")]
        [SerializeField] private int timeLimitIncrease = 10;
        
        [Header("Unlock Configuration")]
        [Tooltip("Whether to create sequential unlock requirements")]
        [SerializeField] private bool sequentialUnlocks = true;
        
        [Tooltip("Number of stars required to unlock special levels")]
        [Range(0, 30)]
        [SerializeField] private int starsRequiredForSpecialLevels = 10;
        
        #region Constants
        
        private const string LEVEL_SELECT_SCREEN_DATA_NAME = "SampleLevelSelectScreenData";
        private const string LEVEL_DATA_PREFIX = "SampleLevel_";
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Creates all sample level data assets
        /// </summary>
        public void CreateAllSampleAssets()
        {
            // Create directory if it doesn't exist
            if (!Directory.Exists(assetsPath))
            {
                Directory.CreateDirectory(assetsPath);
            }
            
            // Create level select screen data
            CreateLevelSelectScreenData();
            
            // Create level data assets
            CreateLevelDataAssets();
            
            // Refresh asset database
            AssetDatabase.Refresh();
            
            Debug.Log("SampleLevelDataCreator: All sample assets created successfully.");
        }
        
        /// <summary>
        /// Creates the level select screen data asset
        /// </summary>
        public void CreateLevelSelectScreenData()
        {
            string assetPath = Path.Combine(assetsPath, $"{LEVEL_SELECT_SCREEN_DATA_NAME}.asset");
            
            // Check if asset already exists
            if (File.Exists(assetPath) && !overwriteExistingAssets)
            {
                Debug.Log($"SampleLevelDataCreator: Level select screen data asset already exists at {assetPath}");
                return;
            }
            
            // Create new asset
            LevelSelectScreenData levelSelectScreenData = ScriptableObject.CreateInstance<LevelSelectScreenData>();
            
            // Configure level select screen data
            ConfigureLevelSelectScreenData(levelSelectScreenData);
            
            // Save asset
            AssetDatabase.CreateAsset(levelSelectScreenData, assetPath);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"SampleLevelDataCreator: Level select screen data asset created at {assetPath}");
        }
        
        /// <summary>
        /// Creates all level data assets
        /// </summary>
        public void CreateLevelDataAssets()
        {
            List<LevelData> createdLevels = new List<LevelData>();
            
            // Create regular levels
            for (int i = 1; i <= numberOfLevels; i++)
            {
                LevelData levelData = CreateLevelDataAsset(i, false);
                if (levelData != null)
                {
                    createdLevels.Add(levelData);
                }
            }
            
            // Create bonus levels (every 5th level)
            for (int i = 1; i <= numberOfLevels / 5; i++)
            {
                LevelData levelData = CreateLevelDataAsset(i, true);
                if (levelData != null)
                {
                    createdLevels.Add(levelData);
                }
            }
            
            // Add created levels to level select screen data
            string levelSelectScreenDataPath = Path.Combine(assetsPath, $"{LEVEL_SELECT_SCREEN_DATA_NAME}.asset");
            LevelSelectScreenData levelSelectScreenData = AssetDatabase.LoadAssetAtPath<LevelSelectScreenData>(levelSelectScreenDataPath);
            
            if (levelSelectScreenData != null)
            {
                // Clear existing levels
                SerializedObject serializedObject = new SerializedObject(levelSelectScreenData);
                SerializedProperty levelsProperty = serializedObject.FindProperty("levels");
                levelsProperty.ClearArray();
                
                // Add created levels
                for (int i = 0; i < createdLevels.Count; i++)
                {
                    levelsProperty.InsertArrayElementAtIndex(i);
                    levelsProperty.GetArrayElementAtIndex(i).objectReferenceValue = createdLevels[i];
                }
                
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(levelSelectScreenData);
                AssetDatabase.SaveAssets();
            }
        }
        
        /// <summary>
        /// Creates a single level data asset
        /// </summary>
        /// <param name="levelNumber">Number of the level</param>
        /// <param name="isBonusLevel">Whether this is a bonus level</param>
        /// <returns>Created level data asset</returns>
        public LevelData CreateLevelDataAsset(int levelNumber, bool isBonusLevel)
        {
            string levelName = isBonusLevel ? $"Bonus_{levelNumber}" : levelNumber.ToString();
            string assetPath = Path.Combine(assetsPath, $"{LEVEL_DATA_PREFIX}{levelName}.asset");
            
            // Check if asset already exists
            if (File.Exists(assetPath) && !overwriteExistingAssets)
            {
                Debug.Log($"SampleLevelDataCreator: Level data asset already exists at {assetPath}");
                return AssetDatabase.LoadAssetAtPath<LevelData>(assetPath);
            }
            
            // Create new asset
            LevelData levelData = ScriptableObject.CreateInstance<LevelData>();
            
            // Configure level data
            ConfigureLevelData(levelData, levelNumber, isBonusLevel);
            
            // Save asset
            AssetDatabase.CreateAsset(levelData, assetPath);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"SampleLevelDataCreator: Level data asset created at {assetPath}");
            return levelData;
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Configures the level select screen data with sample values
        /// </summary>
        /// <param name="levelSelectScreenData">Level select screen data to configure</param>
        private void ConfigureLevelSelectScreenData(LevelSelectScreenData levelSelectScreenData)
        {
            // Set basic properties
            levelSelectScreenData.ScreenTitle = "Level Select";
            levelSelectScreenData.GridColumns = 3;
            levelSelectScreenData.GridRows = 2;
            levelSelectScreenData.GridSpacing = 20;
            levelSelectScreenData.GridPadding = 30;
            levelSelectScreenData.LevelsPerPage = 6;
            levelSelectScreenData.NumberOfPages = numberOfPages;
            
            // Set animation properties
            levelSelectScreenData.HoverAnimationDuration = 0.2f;
            levelSelectScreenData.PressAnimationDuration = 0.1f;
            levelSelectScreenData.TransitionAnimationDuration = 0.3f;
            levelSelectScreenData.HoverScaleFactor = 1.1f;
            levelSelectScreenData.PressScaleFactor = 0.95f;
            
            // Set colors
            ConfigureColors(levelSelectScreenData);
            
            // Set text properties
            levelSelectScreenData.LevelNameFontStyle = FontStyle.Bold;
            levelSelectScreenData.LevelNumberFontSize = 24;
            levelSelectScreenData.LevelNameFontSize = 16;
            levelSelectScreenData.StarCountFontSize = 20;
            levelSelectScreenData.LockedTextFontSize = 14;
            levelSelectScreenData.NewTextFontSize = 14;
            
            // Set localization keys
            levelSelectScreenData.ScreenTitleLocalizationKey = "level_select.title";
            levelSelectScreenData.LockedLevelLocalizationKey = "level_select.locked";
            levelSelectScreenData.NewLevelLocalizationKey = "level_select.new";
            levelSelectScreenData.CompletedLevelLocalizationKey = "level_select.completed";
            levelSelectScreenData.PageNumberLocalizationKey = "level_select.page";
            levelSelectScreenData.PreviousPageLocalizationKey = "level_select.previous";
            levelSelectScreenData.NextPageLocalizationKey = "level_select.next";
        }
        
        /// <summary>
        /// Configures colors for the level select screen data
        /// </summary>
        /// <param name="levelSelectScreenData">Level select screen data to configure</param>
        private void ConfigureColors(LevelSelectScreenData levelSelectScreenData)
        {
            // Set background color
            levelSelectScreenData.BackgroundColor = new Color(0.1f, 0.1f, 0.2f, 1.0f);
            
            // Set level button colors
            var levelButtonColors = new LevelSelectScreenData.LevelButtonColors();
            levelButtonColors.NormalColor = new Color(0.2f, 0.2f, 0.3f, 1.0f);
            levelButtonColors.HoverColor = new Color(0.3f, 0.3f, 0.4f, 1.0f);
            levelButtonColors.PressedColor = new Color(0.15f, 0.15f, 0.25f, 1.0f);
            levelButtonColors.DisabledColor = new Color(0.1f, 0.1f, 0.15f, 1.0f);
            levelButtonColors.SelectedColor = new Color(0.25f, 0.35f, 0.5f, 1.0f);
            levelButtonColors.LockedColor = new Color(0.05f, 0.05f, 0.1f, 1.0f);
            levelButtonColors.NewLevelColor = new Color(0.4f, 0.3f, 0.1f, 1.0f);
            levelButtonColors.CompletedColor = new Color(0.1f, 0.4f, 0.2f, 1.0f);
            levelButtonColors.BorderColor = new Color(0.4f, 0.4f, 0.5f, 1.0f);
            
            // Set text colors
            levelButtonColors.NormalTextColor = Color.white;
            levelButtonColors.DisabledTextColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);
            levelButtonColors.LockedTextColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            levelButtonColors.NewLevelTextColor = new Color(1.0f, 0.9f, 0.6f, 1.0f);
            levelButtonColors.CompletedTextColor = new Color(0.6f, 1.0f, 0.7f, 1.0f);
            
            // Set star colors
            levelButtonColors.EmptyStarColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            levelButtonColors.FilledStarColor = new Color(1.0f, 0.8f, 0.2f, 1.0f);
            
            // Set high contrast colors
            var highContrastColors = new LevelSelectScreenData.HighContrastColors();
            highContrastColors.BackgroundColor = Color.black;
            highContrastColors.LevelButtonColor = new Color(0.8f, 0.8f, 0.8f, 1.0f);
            highContrastColors.TextColor = Color.white;
            highContrastColors.BorderColor = Color.white;
            highContrastColors.StarColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
            highContrastColors.LockedColor = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            highContrastColors.NewLevelColor = new Color(1.0f, 0.6f, 0.0f, 1.0f);
            highContrastColors.CompletedColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
            
            // Apply colors
            levelSelectScreenData.ButtonColors = levelButtonColors;
            levelSelectScreenData.ContrastColors = highContrastColors;
        }
        
        /// <summary>
        /// Configures a level data asset with sample values
        /// </summary>
        /// <param name="levelData">Level data to configure</param>
        /// <param name="levelNumber">Number of the level</param>
        /// <param name="isBonusLevel">Whether this is a bonus level</param>
        private void ConfigureLevelData(LevelData levelData, int levelNumber, bool isBonusLevel)
        {
            // Set basic properties
            string levelName = isBonusLevel ? $"Bonus Level {levelNumber}" : $"Level {levelNumber}";
            string levelId = isBonusLevel ? $"bonus_{levelNumber}" : levelNumber.ToString();
            string sceneName = isBonusLevel ? $"BonusLevel_{levelNumber}" : $"Level_{levelNumber}";
            
            levelData.SetBasicInfo(levelId, levelName, sceneName, levelNumber, isBonusLevel, levelNumber == 1);
            
            // Set difficulty
            float difficulty = baseDifficulty + (levelNumber - 1) * difficultyIncrease;
            difficulty = Mathf.Clamp(difficulty, 1, 10);
            
            // Set time limit
            int timeLimit = baseTimeLimit + (levelNumber - 1) * timeLimitIncrease;
            timeLimit = isBonusLevel ? timeLimit / 2 : timeLimit;
            
            levelData.SetDifficultyAndTime(difficulty, timeLimit);
            
            // Set unlock requirements
            ConfigureUnlockRequirements(levelData, levelNumber, isBonusLevel);
            
            // Set star requirements
            ConfigureStarRequirements(levelData, levelNumber, isBonusLevel);
            
            // Set preview image
            // Note: In a real implementation, you would assign actual preview images
            // levelData.PreviewImage = someSprite;
            
            // Set localization keys
            string nameKey = isBonusLevel ?
                $"levels.bonus_{levelNumber}.name" :
                $"levels.{levelNumber}.name";
                
            string descriptionKey = isBonusLevel ?
                $"levels.bonus_{levelNumber}.description" :
                $"levels.{levelNumber}.description";
                
            levelData.SetLocalizationKeys(nameKey, descriptionKey);
        }
        
        /// <summary>
        /// Configures unlock requirements for a level
        /// </summary>
        /// <param name="levelData">Level data to configure</param>
        /// <param name="levelNumber">Number of the level</param>
        /// <param name="isBonusLevel">Whether this is a bonus level</param>
        private void ConfigureUnlockRequirements(LevelData levelData, int levelNumber, bool isBonusLevel)
        {
            if (levelNumber == 1)
            {
                // First level is unlocked by default
                levelData.SetUnlockRequirements(LevelData.UnlockRequirementType.None, "", 0, "");
                return;
            }
            
            if (isBonusLevel)
            {
                // Bonus levels require stars to unlock
                levelData.SetUnlockRequirements(LevelData.UnlockRequirementType.Stars, "", starsRequiredForSpecialLevels, "");
            }
            else if (sequentialUnlocks)
            {
                // Regular levels require previous level to be completed
                levelData.SetUnlockRequirements(LevelData.UnlockRequirementType.LevelCompleted, (levelNumber - 1).ToString(), 0, "");
            }
            else
            {
                // Non-sequential unlock based on stars
                levelData.SetUnlockRequirements(LevelData.UnlockRequirementType.Stars, "", (levelNumber - 1) * 2, "");
            }
        }
        
        /// <summary>
        /// Configures star requirements for a level
        /// </summary>
        /// <param name="levelData">Level data to configure</param>
        /// <param name="levelNumber">Number of the level</param>
        /// <param name="isBonusLevel">Whether this is a bonus level</param>
        private void ConfigureStarRequirements(LevelData levelData, int levelNumber, bool isBonusLevel)
        {
            // Set star thresholds based on difficulty
            float difficulty = levelData.Difficulty;
            
            float oneStarThreshold, twoStarThreshold, threeStarThreshold;
            
            if (isBonusLevel)
            {
                // Bonus levels have stricter requirements
                oneStarThreshold = 0.8f;
                twoStarThreshold = 0.9f;
                threeStarThreshold = 1.0f;
            }
            else
            {
                // Regular levels have requirements based on difficulty
                oneStarThreshold = Mathf.Lerp(0.5f, 0.7f, difficulty / 10f);
                twoStarThreshold = Mathf.Lerp(0.7f, 0.85f, difficulty / 10f);
                threeStarThreshold = Mathf.Lerp(0.85f, 0.95f, difficulty / 10f);
            }
            
            levelData.SetStarThresholds(oneStarThreshold, twoStarThreshold, threeStarThreshold);
            
            // Set collectible requirements
            int collectiblesForOneStar = 1;
            int collectiblesForTwoStars = Mathf.CeilToInt(oneStarThreshold * 3);
            int collectiblesForThreeStars = 3;
            
            levelData.SetCollectibleRequirements(collectiblesForOneStar, collectiblesForTwoStars, collectiblesForThreeStars);
            
            // Set time requirements
            int timeLimit = levelData.TimeLimitSeconds;
            int timeForOneStar = Mathf.CeilToInt(timeLimit * 0.8f);
            int timeForTwoStars = Mathf.CeilToInt(timeLimit * 0.6f);
            int timeForThreeStars = Mathf.CeilToInt(timeLimit * 0.4f);
            
            levelData.SetTimeRequirements(timeForOneStar, timeForTwoStars, timeForThreeStars);
            
            // Set score requirements
            levelData.SetScoreRequirements(1000, 2500, 5000);
        }
        
        #endregion
        
        #region Editor Methods
        
#if UNITY_EDITOR
        /// <summary>
        /// Called when the script is loaded or a value is changed in the inspector
        /// </summary>
        private void OnValidate()
        {
            // Validate number of levels
            numberOfLevels = Mathf.Clamp(numberOfLevels, 1, 50);
            
            // Validate number of pages
            numberOfPages = Mathf.Clamp(numberOfPages, 1, 5);
            
            // Validate difficulty values
            baseDifficulty = Mathf.Clamp(baseDifficulty, 1, 10);
            difficultyIncrease = Mathf.Clamp(difficultyIncrease, 0, 2);
            
            // Validate time limit values
            baseTimeLimit = Mathf.Max(10, baseTimeLimit);
            timeLimitIncrease = Mathf.Max(0, timeLimitIncrease);
            
            // Validate stars required
            starsRequiredForSpecialLevels = Mathf.Clamp(starsRequiredForSpecialLevels, 0, 30);
        }
        
        /// <summary>
        /// Creates a menu item to create all sample assets
        /// </summary>
        [MenuItem("MixDrop/Tools/Create Sample Level Data")]
        private static void CreateSampleLevelDataMenuItem()
        {
            SampleLevelDataCreator[] creators = FindObjectsOfType<SampleLevelDataCreator>();
            
            if (creators.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "Create Sample Level Data",
                    "No SampleLevelDataCreator found in the scene.",
                    "OK"
                );
                return;
            }
            
            foreach (SampleLevelDataCreator creator in creators)
            {
                creator.CreateAllSampleAssets();
            }
            
            EditorUtility.DisplayDialog(
                "Create Sample Level Data",
                "Sample level data assets created successfully.",
                "OK"
            );
        }
        
        /// <summary>
        /// Creates a menu item to create only the level select screen data
        /// </summary>
        [MenuItem("MixDrop/Tools/Create Sample Level Select Screen Data")]
        private static void CreateSampleLevelSelectScreenDataMenuItem()
        {
            SampleLevelDataCreator[] creators = FindObjectsOfType<SampleLevelDataCreator>();
            
            if (creators.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "Create Sample Level Select Screen Data",
                    "No SampleLevelDataCreator found in the scene.",
                    "OK"
                );
                return;
            }
            
            foreach (SampleLevelDataCreator creator in creators)
            {
                creator.CreateLevelSelectScreenData();
            }
            
            EditorUtility.DisplayDialog(
                "Create Sample Level Select Screen Data",
                "Sample level select screen data asset created successfully.",
                "OK"
            );
        }
        
        /// <summary>
        /// Creates a menu item to create only level data assets
        /// </summary>
        [MenuItem("MixDrop/Tools/Create Sample Level Data Assets")]
        private static void CreateSampleLevelDataAssetsMenuItem()
        {
            SampleLevelDataCreator[] creators = FindObjectsOfType<SampleLevelDataCreator>();
            
            if (creators.Length == 0)
            {
                EditorUtility.DisplayDialog(
                    "Create Sample Level Data Assets",
                    "No SampleLevelDataCreator found in the scene.",
                    "OK"
                );
                return;
            }
            
            foreach (SampleLevelDataCreator creator in creators)
            {
                creator.CreateLevelDataAssets();
            }
            
            EditorUtility.DisplayDialog(
                "Create Sample Level Data Assets",
                "Sample level data assets created successfully.",
                "OK"
            );
        }
        
        /// <summary>
        /// Creates a menu item to add a SampleLevelDataCreator to the scene
        /// </summary>
        [MenuItem("MixDrop/Tools/Add Sample Level Data Creator to Scene")]
        private static void AddSampleLevelDataCreatorToScene()
        {
            GameObject go = new GameObject("SampleLevelDataCreator");
            go.AddComponent<SampleLevelDataCreator>();
            
            EditorUtility.DisplayDialog(
                "Add Sample Level Data Creator to Scene",
                "SampleLevelDataCreator added to the scene.",
                "OK"
            );
        }
#endif
        
        #endregion
    }
}