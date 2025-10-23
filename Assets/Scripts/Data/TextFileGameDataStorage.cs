using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

/// <summary>
/// Simple text file-based storage system for game data
/// Stores current level, completed levels, and star scores in a readable text format
/// </summary>
public class TextFileGameDataStorage : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Name of the save file")]
    [SerializeField] private string saveFileName = "MixDrop_GameData.txt";
    
    [Tooltip("Whether to create a backup when saving")]
    [SerializeField] private bool createBackupOnSave = true;
    
    [Tooltip("Whether to auto-save when data changes")]
    [SerializeField] private bool autoSave = true;
    
    [Tooltip("Delay in seconds before auto-saving after a data change")]
    [SerializeField] private float autoSaveDelay = 2f;
    
    // Private fields
    private string saveFilePath;
    private string backupFilePath;
    private float autoSaveTimer = 0f;
    private bool isDirty = false;
    private bool isInitialized = false;
    
    // Game data
    private int currentLevel = 1;
    private HashSet<int> completedLevels = new HashSet<int>();
    private Dictionary<int, int> levelStars = new Dictionary<int, int>();
    private Dictionary<int, float> levelBestTimes = new Dictionary<int, float>();
    private Dictionary<int, int> levelAttempts = new Dictionary<int, int>();
    
    #region Properties
    
    /// <summary>
    /// Gets the current level
    /// </summary>
    public int CurrentLevel => currentLevel;
    
    /// <summary>
    /// Gets the number of completed levels
    /// </summary>
    public int CompletedLevelsCount => completedLevels.Count;
    
    /// <summary>
    /// Gets the total stars earned
    /// </summary>
    public int TotalStars
    {
        get
        {
            int total = 0;
            foreach (var stars in levelStars.Values)
            {
                total += stars;
            }
            return total;
        }
    }
    
    #endregion
    
    #region Unity Lifecycle Methods
    
    private void Awake()
    {
        // Ensure this object persists between scenes
        //DontDestroyOnLoad(gameObject);
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
                SaveGameData();
                autoSaveTimer = 0f;
                isDirty = false;
            }
        }
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        // Save data when the application is paused
        if (pauseStatus && isDirty)
        {
            SaveGameData();
        }
    }
    
    private void OnApplicationQuit()
    {
        // Save data when the application is quitting
        if (isDirty)
        {
            SaveGameData();
        }
    }
    
    private void OnDestroy()
    {
        // Save data when the object is destroyed
        if (isDirty)
        {
            Debug.Log("TextFileGameDataStorage: OnDestroy called, saving data...");
            SaveGameData();
        }
    }
    
    private void OnDisable()
    {
        // Save data when the object is disabled
        if (isDirty)
        {
            Debug.Log("TextFileGameDataStorage: OnDisable called, saving data...");
            SaveGameData();
        }
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        // Save data when the application loses focus
        if (!hasFocus && isDirty)
        {
            Debug.Log("TextFileGameDataStorage: Application lost focus, saving data...");
            SaveGameData();
        }
    }
    
    #endregion
    
    #region Public Methods
    
    /// <summary>
    /// Initializes the text file storage system
    /// </summary>
    public void Initialize()
    {
        if (isInitialized)
            return;
            
        // Set up file paths
        saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
        backupFilePath = Path.Combine(Application.persistentDataPath, $"{Path.GetFileNameWithoutExtension(saveFileName)}_backup{Path.GetExtension(saveFileName)}");
        
        // Load existing game data
        LoadGameData();
        
        isInitialized = true;
        
        Debug.Log($"TextFileGameDataStorage: Initialized. Save file path: {saveFilePath}");
    }
    
    /// <summary>
    /// Sets the current level
    /// </summary>
    /// <param name="level">Level number</param>
    public void SetCurrentLevel(int level)
    {
        if (level < 1)
            level = 1;
            
        currentLevel = level;
        MarkAsDirty();
    }
    
    /// <summary>
    /// Marks a level as completed
    /// </summary>
    /// <param name="level">Level number</param>
    /// <param name="stars">Stars earned (0-3)</param>
    /// <param name="timeSeconds">Completion time in seconds</param>
    /// <param name="attempts">Number of attempts</param>
    public void CompleteLevel(int level, int stars, float timeSeconds, int attempts)
    {
        if (level < 1)
            return;
            
        Debug.Log($"[TextFileGameDataStorage] CompleteLevel() called for level {level}. Current currentLevel before: {currentLevel}");
            
        // Add to completed levels
        completedLevels.Add(level);
        
        // Update stars if better
        if (!levelStars.ContainsKey(level) || stars > levelStars[level])
        {
            levelStars[level] = stars;
        }
        
        // Update best time if better
        if (!levelBestTimes.ContainsKey(level) || timeSeconds < levelBestTimes[level] || levelBestTimes[level] == 0)
        {
            levelBestTimes[level] = timeSeconds;
        }
        
        // Update attempts
        if (!levelAttempts.ContainsKey(level))
        {
            levelAttempts[level] = 0;
        }
        levelAttempts[level] += attempts;
        
        // Unlock next level
        if (level >= currentLevel)
        {
            currentLevel = level + 1;
            Debug.Log($"[TextFileGameDataStorage] Auto-incrementing currentLevel from {level} to {currentLevel}");
        }
        else
        {
            Debug.Log($"[TextFileGameDataStorage] NOT auto-incrementing currentLevel. level ({level}) < currentLevel ({currentLevel})");
        }
        
        MarkAsDirty();
        
        Debug.Log($"TextFileGameDataStorage: Level {level} completed with {stars} stars in {timeSeconds} seconds. Final currentLevel: {currentLevel}");
    }
    
    /// <summary>
    /// Gets the stars earned for a specific level
    /// </summary>
    /// <param name="level">Level number</param>
    /// <returns>Stars earned (0-3)</returns>
    public int GetLevelStars(int level)
    {
        if (levelStars.TryGetValue(level, out int stars))
        {
            return stars;
        }
        return 0;
    }
    
    /// <summary>
    /// Gets the best completion time for a specific level
    /// </summary>
    /// <param name="level">Level number</param>
    /// <returns>Best time in seconds, or 0 if not completed</returns>
    public float GetLevelBestTime(int level)
    {
        if (levelBestTimes.TryGetValue(level, out float time))
        {
            return time;
        }
        return 0f;
    }
    
    /// <summary>
    /// Gets the number of attempts for a specific level
    /// </summary>
    /// <param name="level">Level number</param>
    /// <returns>Number of attempts</returns>
    public int GetLevelAttempts(int level)
    {
        if (levelAttempts.TryGetValue(level, out int attempts))
        {
            return attempts;
        }
        return 0;
    }
    
    /// <summary>
    /// Checks if a level is completed
    /// </summary>
    /// <param name="level">Level number</param>
    /// <returns>True if the level is completed</returns>
    public bool IsLevelCompleted(int level)
    {
        return completedLevels.Contains(level);
    }
    
    /// <summary>
    /// Resets all game data
    /// </summary>
    public void ResetAllData()
    {
        currentLevel = 1;
        completedLevels.Clear();
        levelStars.Clear();
        levelBestTimes.Clear();
        levelAttempts.Clear();
        
        MarkAsDirty();
        
        Debug.Log("TextFileGameDataStorage: All game data has been reset.");
    }
    
    /// <summary>
    /// Manually saves the game data
    /// </summary>
    public void SaveGameData()
    {
        if (!isInitialized)
        {
            Debug.LogError("TextFileGameDataStorage: Not initialized. Call Initialize() first.");
            return;
        }
        
        try
        {
            Debug.Log($"TextFileGameDataStorage: Starting save process to {saveFilePath}");
            
            // Ensure directory exists
            string directory = Path.GetDirectoryName(saveFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Debug.Log($"TextFileGameDataStorage: Created directory {directory}");
            }
            
            // Create backup if requested and file exists
            if (createBackupOnSave && File.Exists(saveFilePath))
            {
                File.Copy(saveFilePath, backupFilePath, true);
                Debug.Log($"TextFileGameDataStorage: Created backup at {backupFilePath}");
            }
            
            // Create the save data string
            string saveData = CreateSaveDataString();
            Debug.Log($"TextFileGameDataStorage: Generated save data ({saveData.Length} characters)");
            
            // Write to file
            File.WriteAllText(saveFilePath, saveData);
            
            // Verify file was written
            if (File.Exists(saveFilePath))
            {
                FileInfo fileInfo = new FileInfo(saveFilePath);
                Debug.Log($"TextFileGameDataStorage: File written successfully ({fileInfo.Length} bytes)");
            }
            else
            {
                Debug.LogError("TextFileGameDataStorage: File was not created after write operation!");
                return;
            }
            
            // Reset dirty flag
            isDirty = false;
            autoSaveTimer = 0f;
            
            Debug.Log($"TextFileGameDataStorage: Game data saved successfully to {saveFilePath}");
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.LogError($"TextFileGameDataStorage: Access denied when saving. Check file permissions. {e.Message}");
        }
        catch (DirectoryNotFoundException e)
        {
            Debug.LogError($"TextFileGameDataStorage: Directory not found when saving. {e.Message}");
        }
        catch (IOException e)
        {
            Debug.LogError($"TextFileGameDataStorage: IO error when saving. {e.Message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"TextFileGameDataStorage: Failed to save game data: {e.Message}\n{e.StackTrace}");
        }
    }
    
    /// <summary>
    /// Forces an immediate save of game data, even if not marked as dirty
    /// </summary>
    public void ForceSaveGameData()
    {
        Debug.Log("TextFileGameDataStorage: Force save requested");
        isDirty = true;
        SaveGameData();
    }
    
    /// <summary>
    /// Manually loads the game data
    /// </summary>
    public void LoadGameData()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log($"TextFileGameDataStorage: Save file not found at {saveFilePath}. Using default values.");
            return;
        }
        
        try
        {
            string saveData = File.ReadAllText(saveFilePath);
            ParseSaveDataString(saveData);
            
            Debug.Log($"TextFileGameDataStorage: Game data loaded successfully from {saveFilePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"TextFileGameDataStorage: Failed to load game data: {e.Message}");
            
            // Try to load from backup
            if (File.Exists(backupFilePath))
            {
                try
                {
                    string backupData = File.ReadAllText(backupFilePath);
                    ParseSaveDataString(backupData);
                    
                    Debug.Log($"TextFileGameDataStorage: Game data loaded from backup file {backupFilePath}");
                }
                catch (Exception backupException)
                {
                    Debug.LogError($"TextFileGameDataStorage: Failed to load from backup: {backupException.Message}");
                }
            }
        }
    }
    
    /// <summary>
    /// Gets a formatted string with all game data statistics
    /// </summary>
    /// <returns>Formatted statistics string</returns>
    public string GetStatisticsString()
    {
        string stats = "=== Mix-Drop Game Statistics ===\n\n";
        stats += $"Current Level: {currentLevel}\n";
        stats += $"Completed Levels: {completedLevels.Count}\n";
        stats += $"Total Stars: {TotalStars}\n\n";
        
        stats += "Level Details:\n";
        for (int i = 1; i <= currentLevel; i++)
        {
            bool isCompleted = IsLevelCompleted(i);
            int stars = GetLevelStars(i);
            float bestTime = GetLevelBestTime(i);
            int attempts = GetLevelAttempts(i);
            
            stats += $"  Level {i}: ";
            stats += isCompleted ? "Completed" : "Not Completed";
            stats += $", Stars: {stars}/3";
            
            if (bestTime > 0)
            {
                stats += $", Best Time: {bestTime:F1}s";
            }
            
            if (attempts > 0)
            {
                stats += $", Attempts: {attempts}";
            }
            
            stats += "\n";
        }
        
        return stats;
    }
    
    #endregion
    
    #region Private Methods
    
    /// <summary>
    /// Marks the data as dirty for auto-saving
    /// </summary>
    private void MarkAsDirty()
    {
        isDirty = true;
        autoSaveTimer = 0f;
    }
    
    /// <summary>
    /// Creates a formatted string with all game data
    /// </summary>
    /// <returns>Formatted save data string</returns>
    private string CreateSaveDataString()
    {
        string data = $"# Mix-Drop Game Data\n";
        data += $"# Generated on {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n\n";
        
        // Basic game data
        data += $"[GameData]\n";
        data += $"CurrentLevel={currentLevel}\n";
        data += $"CompletedLevelsCount={completedLevels.Count}\n";
        data += $"TotalStars={TotalStars}\n\n";
        
        // Completed levels
        data += $"[CompletedLevels]\n";
        foreach (int level in completedLevels)
        {
            data += $"{level}\n";
        }
        data += "\n";
        
        // Level stars
        data += $"[LevelStars]\n";
        foreach (var kvp in levelStars)
        {
            data += $"{kvp.Key}={kvp.Value}\n";
        }
        data += "\n";
        
        // Level best times
        data += $"[LevelBestTimes]\n";
        foreach (var kvp in levelBestTimes)
        {
            data += $"{kvp.Key}={kvp.Value:F2}\n";
        }
        data += "\n";
        
        // Level attempts
        data += $"[LevelAttempts]\n";
        foreach (var kvp in levelAttempts)
        {
            data += $"{kvp.Key}={kvp.Value}\n";
        }
        
        return data;
    }
    
    /// <summary>
    /// Parses a save data string and populates the game data
    /// </summary>
    /// <param name="saveData">Save data string to parse</param>
    private void ParseSaveDataString(string saveData)
    {
        if (string.IsNullOrEmpty(saveData))
            return;
            
        StringReader reader = new StringReader(saveData);
        string line;
        string currentSection = "";
        
        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();
            
            // Skip empty lines and comments
            if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                continue;
            
            // Check for section headers
            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                currentSection = line.Substring(1, line.Length - 2);
                continue;
            }
            
            // Parse data based on current section
            switch (currentSection)
            {
                case "GameData":
                    ParseGameDataLine(line);
                    break;
                    
                case "CompletedLevels":
                    if (int.TryParse(line, out int level))
                    {
                        completedLevels.Add(level);
                    }
                    break;
                    
                case "LevelStars":
                    ParseKeyValuePair(line, levelStars);
                    break;
                    
                case "LevelBestTimes":
                    ParseFloatKeyValuePair(line, levelBestTimes);
                    break;
                    
                case "LevelAttempts":
                    ParseKeyValuePair(line, levelAttempts);
                    break;
            }
        }
    }
    
    /// <summary>
    /// Parses a game data line
    /// </summary>
    /// <param name="line">Line to parse</param>
    private void ParseGameDataLine(string line)
    {
        string[] parts = line.Split('=');
        if (parts.Length != 2)
            return;
            
        string key = parts[0].Trim();
        string value = parts[1].Trim();
        
        switch (key)
        {
            case "CurrentLevel":
                if (int.TryParse(value, out int currentLevelValue))
                {
                    currentLevel = currentLevelValue;
                }
                break;
                
            case "CompletedLevelsCount":
                // This is calculated from the completed levels list, so we don't need to parse it
                break;
                
            case "TotalStars":
                // This is calculated from the level stars dictionary, so we don't need to parse it
                break;
        }
    }
    
    /// <summary>
    /// Parses a key-value pair with integer values
    /// </summary>
    /// <param name="line">Line to parse</param>
    /// <param name="dictionary">Dictionary to populate</param>
    private void ParseKeyValuePair(string line, Dictionary<int, int> dictionary)
    {
        string[] parts = line.Split('=');
        if (parts.Length != 2)
            return;
            
        if (int.TryParse(parts[0].Trim(), out int key) && int.TryParse(parts[1].Trim(), out int value))
        {
            dictionary[key] = value;
        }
    }
    
    /// <summary>
    /// Parses a key-value pair with float values
    /// </summary>
    /// <param name="line">Line to parse</param>
    /// <param name="dictionary">Dictionary to populate</param>
    private void ParseFloatKeyValuePair(string line, Dictionary<int, float> dictionary)
    {
        string[] parts = line.Split('=');
        if (parts.Length != 2)
            return;
            
        if (int.TryParse(parts[0].Trim(), out int key) && float.TryParse(parts[1].Trim(), out float value))
        {
            dictionary[key] = value;
        }
    }
    
    #endregion
    
    #region Editor Methods
    
#if UNITY_EDITOR
    /// <summary>
    /// Creates a menu item to open the save data file
    /// </summary>
    [UnityEditor.MenuItem("MixDrop/Text File Storage/Open Save Data File")]
    private static void OpenSaveDataFile()
    {
        TextFileGameDataStorage[] storageSystems = FindObjectsOfType<TextFileGameDataStorage>();
        
        if (storageSystems.Length == 0)
        {
            UnityEditor.EditorUtility.DisplayDialog(
                "Open Save Data File",
                "No TextFileGameDataStorage found in the scene.",
                "OK"
            );
            return;
        }
        
        foreach (TextFileGameDataStorage storage in storageSystems)
        {
            if (!storage.isInitialized)
            {
                storage.Initialize();
            }
            
            if (File.Exists(storage.saveFilePath))
            {
                System.Diagnostics.Process.Start(storage.saveFilePath);
            }
            else
            {
                UnityEditor.EditorUtility.DisplayDialog(
                    "Open Save Data File",
                    $"Save file not found at {storage.saveFilePath}",
                    "OK"
                );
            }
        }
    }
    
    /// <summary>
    /// Creates a menu item to print game statistics
    /// </summary>
    [UnityEditor.MenuItem("MixDrop/Text File Storage/Print Game Statistics")]
    private static void PrintGameStatistics()
    {
        TextFileGameDataStorage[] storageSystems = FindObjectsOfType<TextFileGameDataStorage>();
        
        if (storageSystems.Length == 0)
        {
            UnityEditor.EditorUtility.DisplayDialog(
                "Print Game Statistics",
                "No TextFileGameDataStorage found in the scene.",
                "OK"
            );
            return;
        }
        
        foreach (TextFileGameDataStorage storage in storageSystems)
        {
            if (!storage.isInitialized)
            {
                storage.Initialize();
            }
            
            Debug.Log(storage.GetStatisticsString());
        }
        
        UnityEditor.EditorUtility.DisplayDialog(
            "Print Game Statistics",
            "Game statistics have been printed to the console.",
            "OK"
        );
    }
    
    /// <summary>
    /// Creates a menu item to reset all game data
    /// </summary>
    [UnityEditor.MenuItem("MixDrop/Text File Storage/Reset All Game Data")]
    private static void ResetAllGameData()
    {
        TextFileGameDataStorage[] storageSystems = FindObjectsOfType<TextFileGameDataStorage>();
        
        if (storageSystems.Length == 0)
        {
            UnityEditor.EditorUtility.DisplayDialog(
                "Reset Game Data",
                "No TextFileGameDataStorage found in the scene.",
                "OK"
            );
            return;
        }
        
        if (UnityEditor.EditorUtility.DisplayDialog(
            "Reset All Game Data",
            "Are you sure you want to reset all game data? This action cannot be undone.",
            "Reset",
            "Cancel"
        ))
        {
            foreach (TextFileGameDataStorage storage in storageSystems)
            {
                if (!storage.isInitialized)
                {
                    storage.Initialize();
                }
                
                storage.ResetAllData();
                storage.SaveGameData();
            }
            
            UnityEditor.EditorUtility.DisplayDialog(
                "Reset Game Data",
                "All game data has been reset.",
                "OK"
            );
        }
    }
#endif
    
    #endregion
}