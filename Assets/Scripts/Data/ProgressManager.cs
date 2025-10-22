using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace MixDrop.Data
{
    /// <summary>
    /// Manages player progress across all levels, including completion status,
    /// unlock states, and statistics tracking.
    /// </summary>
    public class ProgressManager : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Reference to the level select screen data")]
        [SerializeField] private LevelSelectScreenData levelSelectScreenData;
        
        [Tooltip("Whether to automatically save progress when it changes")]
        [SerializeField] private bool autoSave = true;
        
        [Tooltip("Delay in seconds before auto-saving after a progress change")]
        [SerializeField] private float autoSaveDelay = 2f;
        
        [Header("Events")]
        [Tooltip("Event triggered when a level is completed")]
        [SerializeField] private UnityEngine.Events.UnityEvent<string> onLevelCompleted;
        
        [Tooltip("Event triggered when a level is unlocked")]
        [SerializeField] private UnityEngine.Events.UnityEvent<string> onLevelUnlocked;
        
        [Tooltip("Event triggered when progress is loaded")]
        [SerializeField] private UnityEngine.Events.UnityEvent onProgressLoaded;
        
        [Tooltip("Event triggered when progress is saved")]
        [SerializeField] private UnityEngine.Events.UnityEvent onProgressSaved;
        
        // Private fields
        private Dictionary<string, LevelProgress> levelProgressMap = new Dictionary<string, LevelProgress>();
        private List<string> completedLevels = new List<string>();
        private int totalStarsEarned = 0;
        private float autoSaveTimer = 0f;
        private bool isDirty = false;
        private bool isInitialized = false;
        
        #region Nested Classes
        
        /// <summary>
        /// Represents the progress data for a single level
        /// </summary>
        [System.Serializable]
        private class LevelProgress
        {
            public string levelId;
            public bool isCompleted;
            public int starsAchieved;
            public float bestTimeSeconds;
            public int attemptsCount;
            public int collectiblesFound;
            public int highScore;
            public DateTime lastPlayed;
            
            public LevelProgress(string levelId)
            {
                this.levelId = levelId;
                this.isCompleted = false;
                this.starsAchieved = 0;
                this.bestTimeSeconds = 0;
                this.attemptsCount = 0;
                this.collectiblesFound = 0;
                this.highScore = 0;
                this.lastPlayed = DateTime.MinValue;
            }
        }
        
        /// <summary>
        /// Represents the complete save data for all levels
        /// </summary>
        [System.Serializable]
        private class SaveData
        {
            public List<LevelProgress> levelProgressList = new List<LevelProgress>();
            public DateTime lastSaveTime;
            public string version;
            
            public SaveData()
            {
                this.lastSaveTime = DateTime.Now;
                this.version = Application.version;
            }
        }
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Gets the total number of stars earned across all levels
        /// </summary>
        public int TotalStarsEarned => totalStarsEarned;
        
        /// <summary>
        /// Gets the number of completed levels
        /// </summary>
        public int CompletedLevelsCount => completedLevels.Count;
        
        /// <summary>
        /// Gets the total number of levels
        /// </summary>
        public int TotalLevelsCount => levelSelectScreenData != null ? levelSelectScreenData.GetTotalLevels() : 0;
        
        /// <summary>
        /// Gets the completion percentage across all levels
        /// </summary>
        public float OverallCompletionPercentage
        {
            get
            {
                if (TotalLevelsCount == 0)
                    return 0f;
                    
                return (float)CompletedLevelsCount / TotalLevelsCount * 100f;
            }
        }
        
        /// <summary>
        /// Gets the list of completed level IDs
        /// </summary>
        public IReadOnlyList<string> CompletedLevels => completedLevels.AsReadOnly();
        
        #endregion
        
        #region Unity Lifecycle Methods
        
        private void Awake()
        {
            // Ensure this object persists between scenes
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            Initialize();
        }
        
        private void Update()
        {
            // Handle auto-save timer
            if (isDirty && autoSave)
            {
                autoSaveTimer += Time.deltaTime;
                
                if (autoSaveTimer >= autoSaveDelay)
                {
                    SaveProgress();
                    autoSaveTimer = 0f;
                    isDirty = false;
                }
            }
        }
        
        private void OnApplicationPause(bool pauseStatus)
        {
            // Save progress when the application is paused
            if (pauseStatus && isDirty)
            {
                SaveProgress();
            }
        }
        
        private void OnApplicationQuit()
        {
            // Save progress when the application is quitting
            if (isDirty)
            {
                SaveProgress();
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Initializes the progress manager and loads saved progress
        /// </summary>
        public void Initialize()
        {
            if (isInitialized)
                return;
                
            if (levelSelectScreenData == null)
            {
                Debug.LogError("ProgressManager: LevelSelectScreenData reference is not set!");
                return;
            }
            
            // Load saved progress
            LoadProgress();
            
            // Initialize progress for any new levels
            InitializeNewLevels();
            
            // Calculate total stars earned
            CalculateTotalStars();
            
            isInitialized = true;
            
            // Trigger progress loaded event
            onProgressLoaded?.Invoke();
        }
        
        /// <summary>
        /// Updates the progress for a specific level
        /// </summary>
        /// <param name="levelId">ID of the level to update</param>
        /// <param name="completed">Whether the level was completed</param>
        /// <param name="stars">Star rating achieved (0-3)</param>
        /// <param name="timeSeconds">Completion time in seconds</param>
        /// <param name="collectibles">Number of collectibles found</param>
        /// <param name="score">Score achieved</param>
        public void UpdateLevelProgress(string levelId, bool completed, int stars, float timeSeconds, int collectibles, int score)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("ProgressManager: Not initialized. Call Initialize() first.");
                return;
            }
            
            if (!levelProgressMap.ContainsKey(levelId))
            {
                Debug.LogWarning($"ProgressManager: Level with ID '{levelId}' not found.");
                return;
            }
            
            LevelData levelData = levelSelectScreenData.GetLevelById(levelId);
            if (levelData == null)
            {
                Debug.LogWarning($"ProgressManager: LevelData with ID '{levelId}' not found.");
                return;
            }
            
            LevelProgress progress = levelProgressMap[levelId];
            bool wasCompleted = progress.isCompleted;
            
            // Update progress
            progress.isCompleted = completed;
            progress.attemptsCount++;
            progress.lastPlayed = DateTime.Now;
            
            if (completed)
            {
                // Update stars if better than previous
                if (stars > progress.starsAchieved)
                {
                    progress.starsAchieved = stars;
                }
                
                // Update best time if faster than previous
                if (progress.bestTimeSeconds == 0 || timeSeconds < progress.bestTimeSeconds)
                {
                    progress.bestTimeSeconds = timeSeconds;
                }
                
                // Update collectibles found
                progress.collectiblesFound = Mathf.Max(progress.collectiblesFound, collectibles);
                
                // Update high score
                if (score > progress.highScore)
                {
                    progress.highScore = score;
                }
                
                // Add to completed levels if not already there
                if (!wasCompleted)
                {
                    completedLevels.Add(levelId);
                    onLevelCompleted?.Invoke(levelId);
                }
            }
            
            // Update the LevelData ScriptableObject
            levelData.UpdateProgress(completed, stars, timeSeconds, collectibles, score);
            
            // Check for newly unlocked levels
            CheckForUnlockedLevels();
            
            // Mark as dirty for auto-save
            isDirty = true;
            autoSaveTimer = 0f;
            
            // Recalculate total stars
            CalculateTotalStars();
        }
        
        /// <summary>
        /// Gets the progress for a specific level
        /// </summary>
        /// <param name="levelId">ID of the level</param>
        /// <returns>LevelData with progress information, or null if not found</returns>
        public LevelData GetLevelProgress(string levelId)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("ProgressManager: Not initialized. Call Initialize() first.");
                return null;
            }
            
            LevelData levelData = levelSelectScreenData.GetLevelById(levelId);
            if (levelData == null)
                return null;
                
            // Update the LevelData with progress information
            if (levelProgressMap.TryGetValue(levelId, out LevelProgress progress))
            {
                // Note: In a real implementation, we would update the LevelData with progress info
                // For now, we'll just return the LevelData as is
            }
            
            return levelData;
        }
        
        /// <summary>
        /// Checks if a level is unlocked
        /// </summary>
        /// <param name="levelId">ID of the level to check</param>
        /// <returns>True if the level is unlocked</returns>
        public bool IsLevelUnlocked(string levelId)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("ProgressManager: Not initialized. Call Initialize() first.");
                return false;
            }
            
            LevelData levelData = levelSelectScreenData.GetLevelById(levelId);
            if (levelData == null)
                return false;
                
            return levelData.ShouldUnlock(completedLevels.ToArray(), totalStarsEarned);
        }
        
        /// <summary>
        /// Gets the star rating for a specific level
        /// </summary>
        /// <param name="levelId">ID of the level</param>
        /// <returns>Star rating (0-3)</returns>
        public int GetLevelStars(string levelId)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("ProgressManager: Not initialized. Call Initialize() first.");
                return 0;
            }
            
            if (levelProgressMap.TryGetValue(levelId, out LevelProgress progress))
            {
                return progress.starsAchieved;
            }
            
            return 0;
        }
        
        /// <summary>
        /// Gets the best completion time for a specific level
        /// </summary>
        /// <param name="levelId">ID of the level</param>
        /// <returns>Best time in seconds, or 0 if not completed</returns>
        public float GetLevelBestTime(string levelId)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("ProgressManager: Not initialized. Call Initialize() first.");
                return 0;
            }
            
            if (levelProgressMap.TryGetValue(levelId, out LevelProgress progress))
            {
                return progress.bestTimeSeconds;
            }
            
            return 0;
        }
        
        /// <summary>
        /// Gets the number of attempts for a specific level
        /// </summary>
        /// <param name="levelId">ID of the level</param>
        /// <returns>Number of attempts</returns>
        public int GetLevelAttempts(string levelId)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("ProgressManager: Not initialized. Call Initialize() first.");
                return 0;
            }
            
            if (levelProgressMap.TryGetValue(levelId, out LevelProgress progress))
            {
                return progress.attemptsCount;
            }
            
            return 0;
        }
        
        /// <summary>
        /// Resets the progress for a specific level
        /// </summary>
        /// <param name="levelId">ID of the level to reset</param>
        public void ResetLevelProgress(string levelId)
        {
            if (!isInitialized)
            {
                Debug.LogWarning("ProgressManager: Not initialized. Call Initialize() first.");
                return;
            }
            
            if (!levelProgressMap.ContainsKey(levelId))
            {
                Debug.LogWarning($"ProgressManager: Level with ID '{levelId}' not found.");
                return;
            }
            
            LevelData levelData = levelSelectScreenData.GetLevelById(levelId);
            if (levelData != null)
            {
                levelData.ResetProgress();
            }
            
            // Reset progress
            levelProgressMap[levelId] = new LevelProgress(levelId);
            
            // Remove from completed levels if it was there
            if (completedLevels.Contains(levelId))
            {
                completedLevels.Remove(levelId);
            }
            
            // Mark as dirty for auto-save
            isDirty = true;
            autoSaveTimer = 0f;
            
            // Recalculate total stars
            CalculateTotalStars();
        }
        
        /// <summary>
        /// Resets all progress
        /// </summary>
        public void ResetAllProgress()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("ProgressManager: Not initialized. Call Initialize() first.");
                return;
            }
            
            // Reset all LevelData objects
            foreach (LevelData levelData in levelSelectScreenData.Levels)
            {
                levelData.ResetProgress();
            }
            
            // Clear progress map
            levelProgressMap.Clear();
            
            // Clear completed levels
            completedLevels.Clear();
            
            // Reset total stars
            totalStarsEarned = 0;
            
            // Reinitialize with default progress
            InitializeNewLevels();
            
            // Mark as dirty for auto-save
            isDirty = true;
            autoSaveTimer = 0f;
        }
        
        /// <summary>
        /// Manually saves the current progress
        /// </summary>
        public void SaveProgress()
        {
            if (!isInitialized)
            {
                Debug.LogWarning("ProgressManager: Not initialized. Call Initialize() first.");
                return;
            }
            
            // Create save data
            SaveData saveData = new SaveData();
            saveData.levelProgressList = levelProgressMap.Values.ToList();
            
            // Convert to JSON
            string json = JsonUtility.ToJson(saveData, true);
            
            // Save to PlayerPrefs
            PlayerPrefs.SetString("MixDrop_LevelProgress", json);
            PlayerPrefs.Save();
            
            // Reset dirty flag
            isDirty = false;
            autoSaveTimer = 0f;
            
            // Trigger progress saved event
            onProgressSaved?.Invoke();
            
            Debug.Log("ProgressManager: Progress saved successfully.");
        }
        
        /// <summary>
        /// Manually loads the saved progress
        /// </summary>
        public void LoadProgress()
        {
            // Load from PlayerPrefs
            string json = PlayerPrefs.GetString("MixDrop_LevelProgress", "");
            
            if (string.IsNullOrEmpty(json))
            {
                Debug.Log("ProgressManager: No saved progress found.");
                return;
            }
            
            try
            {
                // Parse JSON
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);
                
                // Clear existing progress
                levelProgressMap.Clear();
                completedLevels.Clear();
                
                // Load progress for each level
                foreach (LevelProgress progress in saveData.levelProgressList)
                {
                    levelProgressMap[progress.levelId] = progress;
                    
                    if (progress.isCompleted)
                    {
                        completedLevels.Add(progress.levelId);
                    }
                }
                
                Debug.Log($"ProgressManager: Progress loaded successfully. {completedLevels.Count} levels completed.");
            }
            catch (Exception e)
            {
                Debug.LogError($"ProgressManager: Failed to load progress: {e.Message}");
            }
        }
        
        /// <summary>
        /// Gets statistics for all levels
        /// </summary>
        /// <returns>Dictionary containing level statistics</returns>
        public Dictionary<string, Dictionary<string, object>> GetAllLevelStatistics()
        {
            Dictionary<string, Dictionary<string, object>> stats = new Dictionary<string, Dictionary<string, object>>();
            
            if (!isInitialized)
            {
                Debug.LogWarning("ProgressManager: Not initialized. Call Initialize() first.");
                return stats;
            }
            
            foreach (LevelData levelData in levelSelectScreenData.Levels)
            {
                Dictionary<string, object> levelStats = new Dictionary<string, object>();
                
                if (levelProgressMap.TryGetValue(levelData.LevelId, out LevelProgress progress))
                {
                    levelStats["isCompleted"] = progress.isCompleted;
                    levelStats["starsAchieved"] = progress.starsAchieved;
                    levelStats["bestTimeSeconds"] = progress.bestTimeSeconds;
                    levelStats["attemptsCount"] = progress.attemptsCount;
                    levelStats["collectiblesFound"] = progress.collectiblesFound;
                    levelStats["highScore"] = progress.highScore;
                    levelStats["lastPlayed"] = progress.lastPlayed;
                    levelStats["isUnlocked"] = IsLevelUnlocked(levelData.LevelId);
                    levelStats["completionPercentage"] = levelData.GetCompletionPercentage();
                }
                else
                {
                    levelStats["isCompleted"] = false;
                    levelStats["starsAchieved"] = 0;
                    levelStats["bestTimeSeconds"] = 0;
                    levelStats["attemptsCount"] = 0;
                    levelStats["collectiblesFound"] = 0;
                    levelStats["highScore"] = 0;
                    levelStats["lastPlayed"] = DateTime.MinValue;
                    levelStats["isUnlocked"] = IsLevelUnlocked(levelData.LevelId);
                    levelStats["completionPercentage"] = 0f;
                }
                
                stats[levelData.LevelId] = levelStats;
            }
            
            return stats;
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Initializes progress for any new levels that don't have progress yet
        /// </summary>
        private void InitializeNewLevels()
        {
            if (levelSelectScreenData == null)
                return;
                
            foreach (LevelData levelData in levelSelectScreenData.Levels)
            {
                if (!levelProgressMap.ContainsKey(levelData.LevelId))
                {
                    levelProgressMap[levelData.LevelId] = new LevelProgress(levelData.LevelId);
                }
            }
        }
        
        /// <summary>
        /// Calculates the total number of stars earned across all levels
        /// </summary>
        private void CalculateTotalStars()
        {
            totalStarsEarned = 0;
            
            foreach (LevelProgress progress in levelProgressMap.Values)
            {
                totalStarsEarned += progress.starsAchieved;
            }
        }
        
        /// <summary>
        /// Checks for any newly unlocked levels and triggers events
        /// </summary>
        private void CheckForUnlockedLevels()
        {
            if (levelSelectScreenData == null)
                return;
                
            foreach (LevelData levelData in levelSelectScreenData.Levels)
            {
                string levelId = levelData.LevelId;
                
                // Skip if already in progress map (already unlocked or has progress)
                if (levelProgressMap.ContainsKey(levelId))
                    continue;
                    
                // Check if level should be unlocked
                if (levelData.ShouldUnlock(completedLevels.ToArray(), totalStarsEarned))
                {
                    // Add to progress map
                    levelProgressMap[levelId] = new LevelProgress(levelId);
                    
                    // Trigger unlock event
                    onLevelUnlocked?.Invoke(levelId);
                    
                    Debug.Log($"ProgressManager: Level '{levelId}' unlocked!");
                }
            }
        }
        
        #endregion
        
        #region Editor Methods
        
#if UNITY_EDITOR
        /// <summary>
        /// Called when the script is loaded or a value is changed in the inspector
        /// </summary>
        private void OnValidate()
        {
            // Validate auto save delay
            autoSaveDelay = Mathf.Max(0.1f, autoSaveDelay);
        }
        
        /// <summary>
        /// Creates a menu item to reset all progress in the editor
        /// </summary>
        [UnityEditor.MenuItem("MixDrop/Debug/Reset All Progress")]
        private static void ResetAllProgressMenuItem()
        {
            ProgressManager[] progressManagers = FindObjectsOfType<ProgressManager>();
            
            if (progressManagers.Length == 0)
            {
                UnityEditor.EditorUtility.DisplayDialog(
                    "Reset Progress",
                    "No ProgressManager found in the scene.",
                    "OK"
                );
                return;
            }
            
            if (UnityEditor.EditorUtility.DisplayDialog(
                "Reset All Progress",
                "Are you sure you want to reset all progress? This action cannot be undone.",
                "Reset",
                "Cancel"
            ))
            {
                foreach (ProgressManager progressManager in progressManagers)
                {
                    progressManager.ResetAllProgress();
                    progressManager.SaveProgress();
                }
                
                UnityEditor.EditorUtility.DisplayDialog(
                    "Reset Progress",
                    "All progress has been reset.",
                    "OK"
                );
            }
        }
        
        /// <summary>
        /// Creates a menu item to print progress statistics in the editor
        /// </summary>
        [UnityEditor.MenuItem("MixDrop/Debug/Print Progress Statistics")]
        private static void PrintProgressStatisticsMenuItem()
        {
            ProgressManager[] progressManagers = FindObjectsOfType<ProgressManager>();
            
            if (progressManagers.Length == 0)
            {
                UnityEditor.EditorUtility.DisplayDialog(
                    "Progress Statistics",
                    "No ProgressManager found in the scene.",
                    "OK"
                );
                return;
            }
            
            foreach (ProgressManager progressManager in progressManagers)
            {
                if (!progressManager.isInitialized)
                {
                    progressManager.Initialize();
                }
                
                string stats = $"Progress Statistics for {progressManager.name}:\n\n";
                stats += $"Total Levels: {progressManager.TotalLevelsCount}\n";
                stats += $"Completed Levels: {progressManager.CompletedLevelsCount}\n";
                stats += $"Total Stars Earned: {progressManager.TotalStarsEarned}\n";
                stats += $"Overall Completion: {progressManager.OverallCompletionPercentage:F1}%\n\n";
                
                stats += "Level Details:\n";
                var allStats = progressManager.GetAllLevelStatistics();
                
                foreach (var kvp in allStats)
                {
                    string levelId = kvp.Key;
                    var levelStats = kvp.Value;
                    
                    stats += $"  {levelId}: ";
                    stats += $"Completed: {levelStats["isCompleted"]}, ";
                    stats += $"Stars: {levelStats["starsAchieved"]}, ";
                    stats += $"Unlocked: {levelStats["isUnlocked"]}\n";
                }
                
                Debug.Log(stats);
            }
            
            UnityEditor.EditorUtility.DisplayDialog(
                "Progress Statistics",
                "Progress statistics have been printed to the console.",
                "OK"
            );
        }
#endif
        
        #endregion
    }
}