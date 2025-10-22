using UnityEngine;

namespace MixDrop.Data
{
    /// <summary>
    /// ScriptableObject containing data for a single level.
    /// Implements the data model for level information including metadata,
    /// completion status, and unlock requirements.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLevelData", menuName = "MixDrop/Level Data")]
    public class LevelData : ScriptableObject
    {
        /// <summary>
        /// Types of unlock requirements for levels
        /// </summary>
        public enum UnlockRequirementType
        {
            None,
            LevelCompleted,
            Stars
        }
        [Header("Level Information")]
        [Tooltip("Unique identifier for the level")]
        [SerializeField] private string levelId;
        
        [Tooltip("Display name of the level")]
        [SerializeField] private string levelName;
        
        [Tooltip("Description of the level")]
        [TextArea(3, 5)]
        [SerializeField] private string description;
        
        [Tooltip("Scene name to load when this level is selected")]
        [SerializeField] private string sceneName;
        
        [Header("Level Configuration")]
        [Tooltip("Difficulty rating from 1 to 10")]
        [Range(1, 10)]
        [SerializeField] private float difficulty;
        
        [Tooltip("Level number in the sequence")]
        [SerializeField] private int levelNumber;
        
        [Tooltip("Whether this is a bonus level")]
        [SerializeField] private bool isBonusLevel = false;
        
        [Tooltip("Time limit for completing the level in seconds")]
        [SerializeField] private int timeLimitSeconds = 120;
        
        [Tooltip("Estimated time to complete in minutes")]
        [SerializeField] private int estimatedTimeMinutes;
        
        [Tooltip("Icon representing the level")]
        [SerializeField] private Sprite levelIcon;
        
        [Tooltip("Background image for the level card")]
        [SerializeField] private Sprite backgroundImage;
        
        [Header("Unlock Requirements")]
        [Tooltip("Whether this level is unlocked by default")]
        [SerializeField] private bool isUnlockedByDefault = false;
        
        [Tooltip("Type of unlock requirement")]
        [SerializeField] private UnlockRequirementType unlockType = UnlockRequirementType.None;
        
        [Tooltip("Minimum level ID required to unlock this level")]
        [SerializeField] private string requiredLevelId;
        
        [Tooltip("Minimum stars required across all levels to unlock this level")]
        [SerializeField] private int requiredStars = 0;
        
        [Tooltip("Specific level from which stars are required")]
        [SerializeField] private string requiredStarsFromLevel = "";
        
        [Header("Star Requirements")]
        [Tooltip("Score threshold for 1 star (0-1)")]
        [Range(0, 1)]
        [SerializeField] private float oneStarThreshold = 0.5f;
        
        [Tooltip("Score threshold for 2 stars (0-1)")]
        [Range(0, 1)]
        [SerializeField] private float twoStarThreshold = 0.7f;
        
        [Tooltip("Score threshold for 3 stars (0-1)")]
        [Range(0, 1)]
        [SerializeField] private float threeStarThreshold = 0.9f;
        
        [Header("Collectible Requirements")]
        [Tooltip("Number of collectibles needed for 1 star")]
        [SerializeField] private int collectiblesForOneStar = 1;
        
        [Tooltip("Number of collectibles needed for 2 stars")]
        [SerializeField] private int collectiblesForTwoStars = 2;
        
        [Tooltip("Number of collectibles needed for 3 stars")]
        [SerializeField] private int collectiblesForThreeStars = 3;
        
        [Header("Time Requirements")]
        [Tooltip("Time needed in seconds for 1 star")]
        [SerializeField] private int timeForOneStar = 90;
        
        [Tooltip("Time needed in seconds for 2 stars")]
        [SerializeField] private int timeForTwoStars = 60;
        
        [Tooltip("Time needed in seconds for 3 stars")]
        [SerializeField] private int timeForThreeStars = 30;
        
        [Header("Score Requirements")]
        [Tooltip("Score needed for 1 star")]
        [SerializeField] private int scoreForOneStar = 1000;
        
        [Tooltip("Score needed for 2 stars")]
        [SerializeField] private int scoreForTwoStars = 2500;
        
        [Tooltip("Score needed for 3 stars")]
        [SerializeField] private int scoreForThreeStars = 5000;
        
        [Header("Localization")]
        [Tooltip("Localization key for level name")]
        [SerializeField] private string levelNameLocalizationKey = "";
        
        [Tooltip("Localization key for level description")]
        [SerializeField] private string levelDescriptionLocalizationKey = "";
        
        [Header("Progress Tracking")]
        [Tooltip("Whether this level has been completed")]
        [SerializeField] private bool isCompleted;
        
        [Tooltip("Star rating achieved (0-3)")]
        [Range(0, 3)]
        [SerializeField] private int starsAchieved;
        
        [Tooltip("Best completion time in seconds")]
        [SerializeField] private float bestTimeSeconds;
        
        [Tooltip("Number of times this level has been attempted")]
        [SerializeField] private int attemptsCount;
        
        [Header("Level Statistics")]
        [Tooltip("Total collectibles in the level")]
        [SerializeField] private int totalCollectibles;
        
        [Tooltip("Collectibles found by the player")]
        [SerializeField] private int collectiblesFound;
        
        [Tooltip("High score achieved in this level")]
        [SerializeField] private int highScore;
        
        #region Properties
        
        /// <summary>
        /// Unique identifier for the level
        /// </summary>
        public string LevelId => levelId;
        
        /// <summary>
        /// Display name of the level
        /// </summary>
        public string LevelName => levelName;
        
        /// <summary>
        /// Description of the level
        /// </summary>
        public string Description => description;
        
        /// <summary>
        /// Scene name to load when this level is selected
        /// </summary>
        public string SceneName => sceneName;
        
        /// <summary>
        /// Difficulty rating from 1 to 10
        /// </summary>
        public float Difficulty => difficulty;
        
        /// <summary>
        /// Level number in the sequence
        /// </summary>
        public int LevelNumber => levelNumber;
        
        /// <summary>
        /// Whether this is a bonus level
        /// </summary>
        public bool IsBonusLevel => isBonusLevel;
        
        /// <summary>
        /// Time limit for completing the level in seconds
        /// </summary>
        public int TimeLimitSeconds => timeLimitSeconds;
        
        /// <summary>
        /// Estimated time to complete in minutes
        /// </summary>
        public int EstimatedTimeMinutes => estimatedTimeMinutes;
        
        /// <summary>
        /// Icon representing the level
        /// </summary>
        public Sprite LevelIcon => levelIcon;
        
        /// <summary>
        /// Background image for the level card
        /// </summary>
        public Sprite BackgroundImage => backgroundImage;
        
        /// <summary>
        /// Whether this level is unlocked by default
        /// </summary>
        public bool IsUnlockedByDefault => isUnlockedByDefault;
        
        /// <summary>
        /// Whether this level is locked by default
        /// </summary>
        public bool IsLockedByDefault => !isUnlockedByDefault;
        
        /// <summary>
        /// Type of unlock requirement
        /// </summary>
        public UnlockRequirementType UnlockType => unlockType;
        
        /// <summary>
        /// Minimum level ID required to unlock this level
        /// </summary>
        public string RequiredLevelId => requiredLevelId;
        
        /// <summary>
        /// Minimum stars required across all levels to unlock this level
        /// </summary>
        public int RequiredStars => requiredStars;
        
        /// <summary>
        /// Specific level from which stars are required
        /// </summary>
        public string RequiredStarsFromLevel => requiredStarsFromLevel;
        
        /// <summary>
        /// Score threshold for 1 star (0-1)
        /// </summary>
        public float OneStarThreshold => oneStarThreshold;
        
        /// <summary>
        /// Score threshold for 2 stars (0-1)
        /// </summary>
        public float TwoStarThreshold => twoStarThreshold;
        
        /// <summary>
        /// Score threshold for 3 stars (0-1)
        /// </summary>
        public float ThreeStarThreshold => threeStarThreshold;
        
        /// <summary>
        /// Number of collectibles needed for 1 star
        /// </summary>
        public int CollectiblesForOneStar => collectiblesForOneStar;
        
        /// <summary>
        /// Number of collectibles needed for 2 stars
        /// </summary>
        public int CollectiblesForTwoStars => collectiblesForTwoStars;
        
        /// <summary>
        /// Number of collectibles needed for 3 stars
        /// </summary>
        public int CollectiblesForThreeStars => collectiblesForThreeStars;
        
        /// <summary>
        /// Time needed in seconds for 1 star
        /// </summary>
        public int TimeForOneStar => timeForOneStar;
        
        /// <summary>
        /// Time needed in seconds for 2 stars
        /// </summary>
        public int TimeForTwoStars => timeForTwoStars;
        
        /// <summary>
        /// Time needed in seconds for 3 stars
        /// </summary>
        public int TimeForThreeStars => timeForThreeStars;
        
        /// <summary>
        /// Score needed for 1 star
        /// </summary>
        public int ScoreForOneStar => scoreForOneStar;
        
        /// <summary>
        /// Score needed for 2 stars
        /// </summary>
        public int ScoreForTwoStars => scoreForTwoStars;
        
        /// <summary>
        /// Score needed for 3 stars
        /// </summary>
        public int ScoreForThreeStars => scoreForThreeStars;
        
        /// <summary>
        /// Localization key for level name
        /// </summary>
        public string LevelNameLocalizationKey => levelNameLocalizationKey;
        
        /// <summary>
        /// Localization key for level description
        /// </summary>
        public string LevelDescriptionLocalizationKey => levelDescriptionLocalizationKey;
        
        /// <summary>
        /// Whether this level has been completed
        /// </summary>
        public bool IsCompleted => isCompleted;
        
        /// <summary>
        /// Star rating achieved (0-3)
        /// </summary>
        public int StarsAchieved => starsAchieved;
        
        /// <summary>
        /// Best completion time in seconds
        /// </summary>
        public float BestTimeSeconds => bestTimeSeconds;
        
        /// <summary>
        /// Number of times this level has been attempted
        /// </summary>
        public int AttemptsCount => attemptsCount;
        
        /// <summary>
        /// Total collectibles in the level
        /// </summary>
        public int TotalCollectibles => totalCollectibles;
        
        /// <summary>
        /// Collectibles found by the player
        /// </summary>
        public int CollectiblesFound => collectiblesFound;
        
        /// <summary>
        /// High score achieved in this level
        /// </summary>
        public int HighScore => highScore;
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Sets the basic level information
        /// </summary>
        /// <param name="id">Level ID</param>
        /// <param name="name">Level name</param>
        /// <param name="scene">Scene name</param>
        /// <param name="number">Level number</param>
        /// <param name="isBonus">Whether this is a bonus level</param>
        /// <param name="isUnlocked">Whether this level is unlocked by default</param>
        public void SetBasicInfo(string id, string name, string scene, int number, bool isBonus, bool isUnlocked)
        {
            levelId = id;
            levelName = name;
            sceneName = scene;
            levelNumber = number;
            isBonusLevel = isBonus;
            isUnlockedByDefault = isUnlocked;
            
            // Mark as dirty to save changes
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        /// <summary>
        /// Sets the difficulty and time limit
        /// </summary>
        /// <param name="difficulty">Difficulty rating</param>
        /// <param name="timeLimit">Time limit in seconds</param>
        public void SetDifficultyAndTime(float difficulty, int timeLimit)
        {
            this.difficulty = difficulty;
            timeLimitSeconds = timeLimit;
            
            // Mark as dirty to save changes
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        /// <summary>
        /// Sets the unlock requirements
        /// </summary>
        /// <param name="unlockType">Type of unlock requirement</param>
        /// <param name="requiredLevelId">Required level ID</param>
        /// <param name="requiredStars">Required stars</param>
        /// <param name="requiredStarsFromLevel">Required stars from specific level</param>
        public void SetUnlockRequirements(UnlockRequirementType unlockType, string requiredLevelId, int requiredStars, string requiredStarsFromLevel)
        {
            this.unlockType = unlockType;
            this.requiredLevelId = requiredLevelId;
            this.requiredStars = requiredStars;
            this.requiredStarsFromLevel = requiredStarsFromLevel;
            
            // Mark as dirty to save changes
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        /// <summary>
        /// Sets the star thresholds
        /// </summary>
        /// <param name="oneStar">Threshold for 1 star</param>
        /// <param name="twoStars">Threshold for 2 stars</param>
        /// <param name="threeStars">Threshold for 3 stars</param>
        public void SetStarThresholds(float oneStar, float twoStars, float threeStars)
        {
            oneStarThreshold = oneStar;
            twoStarThreshold = twoStars;
            threeStarThreshold = threeStars;
            
            // Mark as dirty to save changes
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        /// <summary>
        /// Sets the collectible requirements
        /// </summary>
        /// <param name="oneStar">Collectibles for 1 star</param>
        /// <param name="twoStars">Collectibles for 2 stars</param>
        /// <param name="threeStars">Collectibles for 3 stars</param>
        public void SetCollectibleRequirements(int oneStar, int twoStars, int threeStars)
        {
            collectiblesForOneStar = oneStar;
            collectiblesForTwoStars = twoStars;
            collectiblesForThreeStars = threeStars;
            
            // Mark as dirty to save changes
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        /// <summary>
        /// Sets the time requirements
        /// </summary>
        /// <param name="oneStar">Time for 1 star</param>
        /// <param name="twoStars">Time for 2 stars</param>
        /// <param name="threeStars">Time for 3 stars</param>
        public void SetTimeRequirements(int oneStar, int twoStars, int threeStars)
        {
            timeForOneStar = oneStar;
            timeForTwoStars = twoStars;
            timeForThreeStars = threeStars;
            
            // Mark as dirty to save changes
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        /// <summary>
        /// Sets the score requirements
        /// </summary>
        /// <param name="oneStar">Score for 1 star</param>
        /// <param name="twoStars">Score for 2 stars</param>
        /// <param name="threeStars">Score for 3 stars</param>
        public void SetScoreRequirements(int oneStar, int twoStars, int threeStars)
        {
            scoreForOneStar = oneStar;
            scoreForTwoStars = twoStars;
            scoreForThreeStars = threeStars;
            
            // Mark as dirty to save changes
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        /// <summary>
        /// Sets the localization keys
        /// </summary>
        /// <param name="nameKey">Localization key for level name</param>
        /// <param name="descriptionKey">Localization key for level description</param>
        public void SetLocalizationKeys(string nameKey, string descriptionKey)
        {
            levelNameLocalizationKey = nameKey;
            levelDescriptionLocalizationKey = descriptionKey;
            
            // Mark as dirty to save changes
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        /// <summary>
        /// Updates the level completion status and statistics
        /// </summary>
        /// <param name="completed">Whether the level was completed</param>
        /// <param name="stars">Star rating achieved (0-3)</param>
        /// <param name="timeSeconds">Completion time in seconds</param>
        /// <param name="collectibles">Number of collectibles found</param>
        /// <param name="score">Score achieved</param>
        public void UpdateProgress(bool completed, int stars, float timeSeconds, int collectibles, int score)
        {
            isCompleted = completed;
            attemptsCount++;
            
            if (completed)
            {
                // Update stars if better than previous
                if (stars > starsAchieved)
                {
                    starsAchieved = stars;
                }
                
                // Update best time if faster than previous
                if (bestTimeSeconds == 0 || timeSeconds < bestTimeSeconds)
                {
                    bestTimeSeconds = timeSeconds;
                }
                
                // Update collectibles found
                collectiblesFound = Mathf.Max(collectiblesFound, collectibles);
                
                // Update high score
                if (score > highScore)
                {
                    highScore = score;
                }
            }
            
            // Mark as dirty to save changes
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        /// <summary>
        /// Resets the level progress to default values
        /// </summary>
        public void ResetProgress()
        {
            isCompleted = false;
            starsAchieved = 0;
            bestTimeSeconds = 0;
            attemptsCount = 0;
            collectiblesFound = 0;
            highScore = 0;
            
            // Mark as dirty to save changes
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
        
        /// <summary>
        /// Checks if the level should be unlocked based on completion requirements
        /// </summary>
        /// <param name="completedLevels">List of completed level IDs</param>
        /// <param name="totalStars">Total stars earned across all levels</param>
        /// <returns>True if the level should be unlocked</returns>
        public bool ShouldUnlock(string[] completedLevels, int totalStars)
        {
            if (isUnlockedByDefault)
                return true;
                
            // Check if required level is completed
            if (!string.IsNullOrEmpty(requiredLevelId))
            {
                bool requiredLevelCompleted = System.Array.Exists(completedLevels, id => id == requiredLevelId);
                if (!requiredLevelCompleted)
                    return false;
            }
            
            // Check if required stars are earned
            if (totalStars < requiredStars)
                return false;
                
            return true;
        }
        
        /// <summary>
        /// Gets the completion percentage for this level
        /// </summary>
        /// <returns>Completion percentage (0-100)</returns>
        public float GetCompletionPercentage()
        {
            if (!isCompleted)
                return 0f;
                
            // Calculate based on stars and collectibles
            float starPercentage = (starsAchieved / 3f) * 100f;
            float collectiblePercentage = totalCollectibles > 0 ? (collectiblesFound / (float)totalCollectibles) * 100f : 100f;
            
            return (starPercentage + collectiblePercentage) / 2f;
        }
        
        /// <summary>
        /// Validates the level data to ensure all required fields are set
        /// </summary>
        /// <returns>True if the data is valid</returns>
        public bool Validate()
        {
            // Check required fields
            if (string.IsNullOrEmpty(levelId))
            {
                Debug.LogError($"Level validation failed: Level ID is not set for {name}");
                return false;
            }
            
            if (string.IsNullOrEmpty(levelName))
            {
                Debug.LogError($"Level validation failed: Level name is not set for {levelId}");
                return false;
            }
            
            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogError($"Level validation failed: Scene name is not set for {levelId}");
                return false;
            }
            
            // Check value ranges
            if (difficulty < 1 || difficulty > 10)
            {
                Debug.LogError($"Level validation failed: Difficulty must be between 1 and 10 for {levelId}");
                return false;
            }
            
            if (estimatedTimeMinutes <= 0)
            {
                Debug.LogError($"Level validation failed: Estimated time must be greater than 0 for {levelId}");
                return false;
            }
            
            if (timeLimitSeconds <= 0)
            {
                Debug.LogError($"Level validation failed: Time limit must be greater than 0 for {levelId}");
                return false;
            }
            
            if (starsAchieved < 0 || starsAchieved > 3)
            {
                Debug.LogError($"Level validation failed: Stars achieved must be between 0 and 3 for {levelId}");
                return false;
            }
            
            if (collectiblesFound < 0 || collectiblesFound > totalCollectibles)
            {
                Debug.LogError($"Level validation failed: Collectibles found must be between 0 and total collectibles for {levelId}");
                return false;
            }
            
            // Check star thresholds
            if (oneStarThreshold < 0 || oneStarThreshold > 1)
            {
                Debug.LogError($"Level validation failed: One star threshold must be between 0 and 1 for {levelId}");
                return false;
            }
            
            if (twoStarThreshold < 0 || twoStarThreshold > 1)
            {
                Debug.LogError($"Level validation failed: Two star threshold must be between 0 and 1 for {levelId}");
                return false;
            }
            
            if (threeStarThreshold < 0 || threeStarThreshold > 1)
            {
                Debug.LogError($"Level validation failed: Three star threshold must be between 0 and 1 for {levelId}");
                return false;
            }
            
            if (oneStarThreshold > twoStarThreshold || twoStarThreshold > threeStarThreshold)
            {
                Debug.LogError($"Level validation failed: Star thresholds must be in ascending order for {levelId}");
                return false;
            }
            
            // Check collectible requirements
            if (collectiblesForOneStar < 0 || collectiblesForOneStar > totalCollectibles)
            {
                Debug.LogError($"Level validation failed: Collectibles for one star must be between 0 and total collectibles for {levelId}");
                return false;
            }
            
            if (collectiblesForTwoStars < collectiblesForOneStar || collectiblesForTwoStars > totalCollectibles)
            {
                Debug.LogError($"Level validation failed: Collectibles for two stars must be between one star requirement and total collectibles for {levelId}");
                return false;
            }
            
            if (collectiblesForThreeStars < collectiblesForTwoStars || collectiblesForThreeStars > totalCollectibles)
            {
                Debug.LogError($"Level validation failed: Collectibles for three stars must be between two stars requirement and total collectibles for {levelId}");
                return false;
            }
            
            // Check time requirements
            if (timeForOneStar < 0)
            {
                Debug.LogError($"Level validation failed: Time for one star must be non-negative for {levelId}");
                return false;
            }
            
            if (timeForTwoStars < 0)
            {
                Debug.LogError($"Level validation failed: Time for two stars must be non-negative for {levelId}");
                return false;
            }
            
            if (timeForThreeStars < 0)
            {
                Debug.LogError($"Level validation failed: Time for three stars must be non-negative for {levelId}");
                return false;
            }
            
            if (timeForOneStar < timeForTwoStars || timeForTwoStars < timeForThreeStars)
            {
                Debug.LogError($"Level validation failed: Time requirements must be in descending order for {levelId}");
                return false;
            }
            
            // Check score requirements
            if (scoreForOneStar < 0)
            {
                Debug.LogError($"Level validation failed: Score for one star must be non-negative for {levelId}");
                return false;
            }
            
            if (scoreForTwoStars < scoreForOneStar)
            {
                Debug.LogError($"Level validation failed: Score for two stars must be at least score for one star for {levelId}");
                return false;
            }
            
            if (scoreForThreeStars < scoreForTwoStars)
            {
                Debug.LogError($"Level validation failed: Score for three stars must be at least score for two stars for {levelId}");
                return false;
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
            // Ensure level ID is set if not already
            if (string.IsNullOrEmpty(levelId))
            {
                levelId = System.Guid.NewGuid().ToString("N");
            }
            
            // Clamp values to valid ranges
            difficulty = Mathf.Clamp(difficulty, 1, 10);
            starsAchieved = Mathf.Clamp(starsAchieved, 0, 3);
            requiredStars = Mathf.Clamp(requiredStars, 0, 30);
            collectiblesFound = Mathf.Clamp(collectiblesFound, 0, totalCollectibles);
            
            // Clamp star thresholds
            oneStarThreshold = Mathf.Clamp(oneStarThreshold, 0, 1);
            twoStarThreshold = Mathf.Clamp(twoStarThreshold, 0, 1);
            threeStarThreshold = Mathf.Clamp(threeStarThreshold, 0, 1);
            
            // Ensure thresholds are in ascending order
            if (oneStarThreshold > twoStarThreshold)
                twoStarThreshold = oneStarThreshold;
            if (twoStarThreshold > threeStarThreshold)
                threeStarThreshold = twoStarThreshold;
            
            // Clamp collectible requirements
            collectiblesForOneStar = Mathf.Clamp(collectiblesForOneStar, 0, totalCollectibles);
            collectiblesForTwoStars = Mathf.Clamp(collectiblesForTwoStars, collectiblesForOneStar, totalCollectibles);
            collectiblesForThreeStars = Mathf.Clamp(collectiblesForThreeStars, collectiblesForTwoStars, totalCollectibles);
            
            // Clamp time requirements
            timeForOneStar = Mathf.Max(0, timeForOneStar);
            timeForTwoStars = Mathf.Max(0, timeForTwoStars);
            timeForThreeStars = Mathf.Max(0, timeForThreeStars);
            
            // Ensure time requirements are in descending order
            if (timeForOneStar < timeForTwoStars)
                timeForTwoStars = timeForOneStar;
            if (timeForTwoStars < timeForThreeStars)
                timeForThreeStars = timeForTwoStars;
            
            // Clamp score requirements
            scoreForOneStar = Mathf.Max(0, scoreForOneStar);
            scoreForTwoStars = Mathf.Max(scoreForOneStar, scoreForTwoStars);
            scoreForThreeStars = Mathf.Max(scoreForTwoStars, scoreForThreeStars);
            
            // Validate data
            Validate();
        }
        
        /// <summary>
        /// Creates a menu item to validate all level data assets
        /// </summary>
        [UnityEditor.MenuItem("MixDrop/Validate All Level Data")]
        private static void ValidateAllLevelData()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:LevelData");
            int validCount = 0;
            int invalidCount = 0;
            
            foreach (string guid in guids)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                LevelData levelData = UnityEditor.AssetDatabase.LoadAssetAtPath<LevelData>(path);
                
                if (levelData.Validate())
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
                $"Validation complete.\n\nValid levels: {validCount}\nInvalid levels: {invalidCount}",
                "OK"
            );
        }
#endif
        
        #endregion
    }
}