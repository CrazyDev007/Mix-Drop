using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// Simple JSON container class for parsing JSON data
[System.Serializable]
public class SimpleJSON
{
    public Dictionary<string, string> data = new Dictionary<string, string>();
    public List<SimpleJSON> array = new List<SimpleJSON>();
    public bool isObject = false;
    public bool isArray = false;
    
    public SimpleJSON AsObject()
    {
        isObject = true;
        return this;
    }
    
    public SimpleJSON AsArray()
    {
        isArray = true;
        return this;
    }
    
    public bool ContainsKey(string key)
    {
        return data.ContainsKey(key);
    }
    
    public SimpleJSON this[string key]
    {
        get
        {
            if (data.ContainsKey(key))
            {
                var json = new SimpleJSON();
                json.data = new Dictionary<string, string>();
                json.data["value"] = data[key];
                return json;
            }
            return new SimpleJSON();
        }
        set
        {
            if (value != null)
            {
                if (value.data.ContainsKey("value"))
                {
                    data[key] = value.data["value"];
                }
                else
                {
                    // Handle direct string assignment
                    data[key] = value.ToString();
                }
            }
        }
    }
    
    public SimpleJSON this[int index]
    {
        get
        {
            if (index >= 0 && index < array.Count)
            {
                return array[index];
            }
            return new SimpleJSON();
        }
    }
    
    public string Value
    {
        get
        {
            if (data.ContainsKey("value"))
            {
                return data["value"];
            }
            return "";
        }
    }
    
    public int Count
    {
        get
        {
            return array.Count;
        }
    }
    
    public void Remove(string key)
    {
        if (data.ContainsKey(key))
        {
            data.Remove(key);
        }
    }
    
    public override string ToString()
    {
        if (isObject)
        {
            string result = "{";
            bool first = true;
            foreach (var kvp in data)
            {
                if (!first) result += ",";
                result += "\"" + kvp.Key + "\":\"" + kvp.Value + "\"";
                first = false;
            }
            result += "}";
            return result;
        }
        else if (isArray)
        {
            string result = "[";
            for (int i = 0; i < array.Count; i++)
            {
                if (i > 0) result += ",";
                result += array[i].ToString();
            }
            result += "]";
            return result;
        }
        return "";
    }
    
    // Implicit conversion from string to SimpleJSON
    public static implicit operator SimpleJSON(string value)
    {
        var json = new SimpleJSON();
        json.data = new Dictionary<string, string>();
        json.data["value"] = value;
        return json;
    }
}

namespace MixDrop.Data
{
    /// <summary>
    /// Handles data validation and migration for save game data,
    /// ensuring compatibility between different versions of the game.
    /// </summary>
    public class DataValidationAndMigration : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Current version of the data format")]
        [SerializeField] private string currentDataVersion = "1.0.0";
        
        [Tooltip("Whether to automatically migrate data when loading")]
        [SerializeField] private bool autoMigrateData = true;
        
        [Tooltip("Whether to create a backup before migrating data")]
        [SerializeField] private bool backupBeforeMigration = true;
        
        [Tooltip("Whether to validate data integrity when loading")]
        [SerializeField] private bool validateDataIntegrity = true;
        
        [Header("Events")]
        [Tooltip("Event triggered when data validation succeeds")]
        [SerializeField] private UnityEngine.Events.UnityEvent onDataValidationSuccess;
        
        [Tooltip("Event triggered when data validation fails")]
        [SerializeField] private UnityEngine.Events.UnityEvent<string> onDataValidationFailure;
        
        [Tooltip("Event triggered when data migration succeeds")]
        [SerializeField] private UnityEngine.Events.UnityEvent<string> onDataMigrationSuccess;
        
        [Tooltip("Event triggered when data migration fails")]
        [SerializeField] private UnityEngine.Events.UnityEvent<string> onDataMigrationFailure;
        
        // Private fields
        private Dictionary<string, DataMigrationRule> migrationRules = new Dictionary<string, DataMigrationRule>();
        private Dictionary<string, DataValidationRule> validationRules = new Dictionary<string, DataValidationRule>();
        private bool isInitialized = false;
        
        #region Nested Classes
        
        /// <summary>
        /// Represents a rule for migrating data from one version to another
        /// </summary>
        [System.Serializable]
        public class DataMigrationRule
        {
            public string fromVersion;
            public string toVersion;
            public string description;
            public bool isRequired;
            
            // Delegate for the migration function
            public Func<string, string> migrationFunction;
        }
        
        /// <summary>
        /// Represents a rule for validating data integrity
        /// </summary>
        [System.Serializable]
        public class DataValidationRule
        {
            public string name;
            public string description;
            public bool isRequired;
            public string appliesToVersion;
            
            // Delegate for the validation function
            public Func<string, bool> validationFunction;
        }
        
        /// <summary>
        /// Represents the result of a data validation operation
        /// </summary>
        [System.Serializable]
        public class ValidationResult
        {
            public bool isValid;
            public List<string> errorMessages = new List<string>();
            public List<string> warningMessages = new List<string>();
            
            public void AddError(string message)
            {
                errorMessages.Add(message);
                isValid = false;
            }
            
            public void AddWarning(string message)
            {
                warningMessages.Add(message);
            }
        }
        
        /// <summary>
        /// Represents the result of a data migration operation
        /// </summary>
        [System.Serializable]
        public class MigrationResult
        {
            public bool success;
            public string fromVersion;
            public string toVersion;
            public List<string> messages = new List<string>();
            
            public void AddMessage(string message)
            {
                messages.Add(message);
            }
        }
        
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
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Initializes the data validation and migration system
        /// </summary>
        public void Initialize()
        {
            if (isInitialized)
                return;
                
            // Register migration rules
            RegisterMigrationRules();
            
            // Register validation rules
            RegisterValidationRules();
            
            isInitialized = true;
            
            Debug.Log("DataValidationAndMigration: Initialized successfully.");
        }
        
        /// <summary>
        /// Validates the integrity of save data
        /// </summary>
        /// <param name="jsonData">JSON data to validate</param>
        /// <param name="dataVersion">Version of the data</param>
        /// <returns>Validation result</returns>
        public ValidationResult ValidateData(string jsonData, string dataVersion)
        {
            if (!isInitialized)
            {
                Debug.LogError("DataValidationAndMigration: Not initialized. Call Initialize() first.");
                return new ValidationResult { isValid = false };
            }
            
            ValidationResult result = new ValidationResult { isValid = true };
            
            try
            {
                // Check if JSON is valid
                if (!IsValidJson(jsonData))
                {
                    result.AddError("Invalid JSON format");
                    return result;
                }
                
                // Apply validation rules
                foreach (var rule in validationRules.Values)
                {
                    // Skip rules that don't apply to this version
                    if (!string.IsNullOrEmpty(rule.appliesToVersion) && 
                        rule.appliesToVersion != dataVersion)
                    {
                        continue;
                    }
                    
                    try
                    {
                        bool isValid = rule.validationFunction(jsonData);
                        
                        if (!isValid)
                        {
                            string message = $"Validation failed: {rule.description}";
                            
                            if (rule.isRequired)
                            {
                                result.AddError(message);
                            }
                            else
                            {
                                result.AddWarning(message);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        string message = $"Validation error for rule '{rule.name}': {e.Message}";
                        
                        if (rule.isRequired)
                        {
                            result.AddError(message);
                        }
                        else
                        {
                            result.AddWarning(message);
                        }
                    }
                }
                
                // Trigger appropriate event
                if (result.isValid)
                {
                    onDataValidationSuccess?.Invoke();
                }
                else
                {
                    string errors = string.Join("\n", result.errorMessages);
                    onDataValidationFailure?.Invoke(errors);
                }
                
                return result;
            }
            catch (Exception e)
            {
                result.AddError($"Unexpected validation error: {e.Message}");
                onDataValidationFailure?.Invoke(result.errorMessages[0]);
                return result;
            }
        }
        
        /// <summary>
        /// Migrates data from one version to another
        /// </summary>
        /// <param name="jsonData">JSON data to migrate</param>
        /// <param name="fromVersion">Version to migrate from</param>
        /// <param name="toVersion">Version to migrate to</param>
        /// <returns>Migration result</returns>
        public MigrationResult MigrateData(string jsonData, string fromVersion, string toVersion)
        {
            if (!isInitialized)
            {
                Debug.LogError("DataValidationAndMigration: Not initialized. Call Initialize() first.");
                return new MigrationResult { success = false };
            }
            
            MigrationResult result = new MigrationResult
            {
                success = true,
                fromVersion = fromVersion,
                toVersion = toVersion
            };
            
            try
            {
                // If versions are the same, no migration needed
                if (fromVersion == toVersion)
                {
                    result.AddMessage("Data is already at the target version. No migration needed.");
                    return result;
                }
                
                // Get migration path
                List<string> migrationPath = GetMigrationPath(fromVersion, toVersion);
                
                if (migrationPath.Count == 0)
                {
                    result.AddMessage("No migration path found. Data may already be at the target version.");
                    return result;
                }
                
                // Apply migrations in sequence
                string currentData = jsonData;
                string currentVersion = fromVersion;
                
                for (int i = 0; i < migrationPath.Count - 1; i++)
                {
                    string sourceVersion = migrationPath[i];
                    string targetVersion = migrationPath[i + 1];
                    
                    string ruleKey = $"{sourceVersion}_to_{targetVersion}";
                    
                    if (migrationRules.TryGetValue(ruleKey, out DataMigrationRule rule))
                    {
                        try
                        {
                            result.AddMessage($"Migrating from {sourceVersion} to {targetVersion}: {rule.description}");
                            
                            // Apply migration
                            currentData = rule.migrationFunction(currentData);
                            currentVersion = targetVersion;
                        }
                        catch (Exception e)
                        {
                            string message = $"Migration from {sourceVersion} to {targetVersion} failed: {e.Message}";
                            
                            if (rule.isRequired)
                            {
                                result.success = false;
                                result.AddMessage(message);
                                break;
                            }
                            else
                            {
                                result.AddMessage($"Warning: {message}");
                            }
                        }
                    }
                    else
                    {
                        result.AddMessage($"No migration rule found for {sourceVersion} to {targetVersion}");
                    }
                }
                
                // Update version in the migrated data
                if (result.success)
                {
                    currentData = UpdateDataVersion(currentData, toVersion);
                    result.AddMessage($"Migration completed successfully. Data updated to version {toVersion}.");
                }
                
                // Trigger appropriate event
                if (result.success)
                {
                    onDataMigrationSuccess?.Invoke(toVersion);
                }
                else
                {
                    string errors = string.Join("\n", result.messages);
                    onDataMigrationFailure?.Invoke(errors);
                }
                
                return result;
            }
            catch (Exception e)
            {
                result.success = false;
                result.AddMessage($"Unexpected migration error: {e.Message}");
                onDataMigrationFailure?.Invoke(result.messages[0]);
                return result;
            }
        }
        
        /// <summary>
        /// Validates and migrates data in one operation
        /// </summary>
        /// <param name="jsonData">JSON data to validate and migrate</param>
        /// <param name="dataVersion">Version of the data</param>
        /// <returns>Result containing validation result, migration result, and final data</returns>
        public async Task<(ValidationResult validationResult, MigrationResult migrationResult, string finalData)> ValidateAndMigrateDataAsync(string jsonData, string dataVersion)
        {
            if (!isInitialized)
            {
                Debug.LogError("DataValidationAndMigration: Not initialized. Call Initialize() first.");
                return (new ValidationResult { isValid = false }, new MigrationResult { success = false }, jsonData);
            }
            
            // Validate data
            ValidationResult validationResult = ValidateData(jsonData, dataVersion);
            
            // If validation failed and there are critical errors, return early
            if (!validationResult.isValid && validationResult.errorMessages.Count > 0)
            {
                return (validationResult, new MigrationResult { success = false }, jsonData);
            }
            
            // Migrate data if needed
            MigrationResult migrationResult = new MigrationResult { success = true };
            string finalData = jsonData;
            
            if (autoMigrateData && dataVersion != currentDataVersion)
            {
                // Create backup if requested
                if (backupBeforeMigration)
                {
                    // Note: In a real implementation, you would call the DataPersistenceLayer to create a backup
                    Debug.Log("DataValidationAndMigration: Creating backup before migration...");
                }
                
                // Perform migration
                migrationResult = MigrateData(jsonData, dataVersion, currentDataVersion);
                
                if (migrationResult.success)
                {
                    finalData = migrationResult.success ? UpdateDataVersion(jsonData, currentDataVersion) : jsonData;
                    
                    // Validate migrated data
                    ValidationResult migratedValidationResult = ValidateData(finalData, currentDataVersion);
                    
                    if (!migratedValidationResult.isValid)
                    {
                        // Merge validation results
                        validationResult.isValid = false;
                        validationResult.errorMessages.AddRange(migratedValidationResult.errorMessages);
                        validationResult.warningMessages.AddRange(migratedValidationResult.warningMessages);
                    }
                }
            }
            
            return (validationResult, migrationResult, finalData);
        }
        
        /// <summary>
        /// Gets the version from JSON data
        /// </summary>
        /// <param name="jsonData">JSON data to extract version from</param>
        /// <returns>Version string, or null if not found</returns>
        public string GetDataVersion(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                return null;
                
            try
            {
                // Parse JSON to extract version
                var jsonNode = ParseJson(jsonData);
                
                // Try to get version from different possible locations
                if (jsonNode.AsObject().ContainsKey("version"))
                {
                    return jsonNode.AsObject()["version"].Value;
                }
                else if (jsonNode.AsObject().ContainsKey("dataVersion"))
                {
                    return jsonNode.AsObject()["dataVersion"].Value;
                }
                else if (jsonNode.AsObject().ContainsKey("saveVersion"))
                {
                    return jsonNode.AsObject()["saveVersion"].Value;
                }
                
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"DataValidationAndMigration: Failed to get data version: {e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Updates the version in JSON data
        /// </summary>
        /// <param name="jsonData">JSON data to update</param>
        /// <param name="newVersion">New version string</param>
        /// <returns>Updated JSON data</returns>
        public string UpdateDataVersion(string jsonData, string newVersion)
        {
            if (string.IsNullOrEmpty(jsonData))
                return jsonData;
                
            try
            {
                // Parse JSON
                var jsonNode = ParseJson(jsonData);
                
                // Update version if it exists
                if (jsonNode.AsObject().ContainsKey("version"))
                {
                    jsonNode.AsObject()["version"] = newVersion;
                }
                else if (jsonNode.AsObject().ContainsKey("dataVersion"))
                {
                    jsonNode.AsObject()["dataVersion"] = newVersion;
                }
                else if (jsonNode.AsObject().ContainsKey("saveVersion"))
                {
                    jsonNode.AsObject()["saveVersion"] = newVersion;
                }
                else
                {
                    // Add version field if it doesn't exist
                    jsonNode.AsObject()["version"] = newVersion;
                }
                
                // Convert back to JSON
                return jsonNode.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError($"DataValidationAndMigration: Failed to update data version: {e.Message}");
                return jsonData;
            }
        }
        
        /// <summary>
        /// Gets all registered migration rules
        /// </summary>
        /// <returns>Dictionary of migration rules</returns>
        public Dictionary<string, DataMigrationRule> GetMigrationRules()
        {
            return new Dictionary<string, DataMigrationRule>(migrationRules);
        }
        
        /// <summary>
        /// Gets all registered validation rules
        /// </summary>
        /// <returns>Dictionary of validation rules</returns>
        public Dictionary<string, DataValidationRule> GetValidationRules()
        {
            return new Dictionary<string, DataValidationRule>(validationRules);
        }
        
        /// <summary>
        /// Adds a custom migration rule
        /// </summary>
        /// <param name="fromVersion">Version to migrate from</param>
        /// <param name="toVersion">Version to migrate to</param>
        /// <param name="description">Description of the migration</param>
        /// <param name="isRequired">Whether the migration is required</param>
        /// <param name="migrationFunction">Function to perform the migration</param>
        public void AddMigrationRule(string fromVersion, string toVersion, string description, bool isRequired, Func<string, string> migrationFunction)
        {
            string ruleKey = $"{fromVersion}_to_{toVersion}";
            
            migrationRules[ruleKey] = new DataMigrationRule
            {
                fromVersion = fromVersion,
                toVersion = toVersion,
                description = description,
                isRequired = isRequired,
                migrationFunction = migrationFunction
            };
        }
        
        /// <summary>
        /// Adds a custom validation rule
        /// </summary>
        /// <param name="name">Name of the validation rule</param>
        /// <param name="description">Description of the validation</param>
        /// <param name="isRequired">Whether the validation is required</param>
        /// <param name="appliesToVersion">Version the rule applies to (null for all versions)</param>
        /// <param name="validationFunction">Function to perform the validation</param>
        public void AddValidationRule(string name, string description, bool isRequired, string appliesToVersion, Func<string, bool> validationFunction)
        {
            validationRules[name] = new DataValidationRule
            {
                name = name,
                description = description,
                isRequired = isRequired,
                appliesToVersion = appliesToVersion,
                validationFunction = validationFunction
            };
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Registers all built-in migration rules
        /// </summary>
        private void RegisterMigrationRules()
        {
            // Example migration rules for different versions
            // In a real implementation, you would add rules for each version change
            
            // Migration from 0.9.0 to 1.0.0
            AddMigrationRule("0.9.0", "1.0.0", "Add version field and restructure level data", true, (json) =>
            {
                // Parse JSON
                var jsonNode = ParseJson(json);
                
                // Add version field
                jsonNode.AsObject()["version"] = "1.0.0";
                
                // Migrate level data structure if it exists
                if (jsonNode.AsObject().ContainsKey("levels"))
                {
                    var levelsArray = jsonNode.AsObject()["levels"].AsArray();
                    
                    for (int i = 0; i < levelsArray.Count; i++)
                    {
                        var levelNode = levelsArray[i].AsObject();
                        
                        // Add new fields
                        if (!levelNode.ContainsKey("levelId"))
                        {
                            levelNode["levelId"] = levelNode.ContainsKey("id") ? levelNode["id"].Value : System.Guid.NewGuid().ToString();
                        }
                        
                        if (!levelNode.ContainsKey("starsAchieved"))
                        {
                            levelNode["starsAchieved"] = levelNode.ContainsKey("stars") ? levelNode["stars"].Value : "0";
                        }
                        
                        if (!levelNode.ContainsKey("bestTimeSeconds"))
                        {
                            levelNode["bestTimeSeconds"] = levelNode.ContainsKey("bestTime") ? levelNode["bestTime"].Value : "0";
                        }
                        
                        // Remove old fields if they exist
                        if (levelNode.ContainsKey("id"))
                        {
                            levelNode.Remove("id");
                        }
                        
                        if (levelNode.ContainsKey("stars"))
                        {
                            levelNode.Remove("stars");
                        }
                        
                        if (levelNode.ContainsKey("bestTime"))
                        {
                            levelNode.Remove("bestTime");
                        }
                    }
                }
                
                // Convert back to JSON
                return jsonNode.ToString();
            });
            
            // Add more migration rules as needed for future versions
        }
        
        /// <summary>
        /// Registers all built-in validation rules
        /// </summary>
        private void RegisterValidationRules()
        {
            // Example validation rules
            // In a real implementation, you would add rules for each data structure
            
            // Validate that required fields exist
            AddValidationRule("RequiredFields", "Check that required fields exist", true, null, (json) =>
            {
                var jsonNode = ParseJson(json);
                var obj = jsonNode.AsObject();
                
                // Check for version field
                if (!obj.ContainsKey("version"))
                {
                    return false;
                }
                
                // Check for levels array if it should exist
                if (obj.ContainsKey("levels"))
                {
                    var levelsArray = obj["levels"].AsArray();
                    
                    for (int i = 0; i < levelsArray.Count; i++)
                    {
                        var levelNode = levelsArray[i].AsObject();
                        
                        // Check for required level fields
                        if (!levelNode.ContainsKey("levelId"))
                        {
                            return false;
                        }
                    }
                }
                
                return true;
            });
            
            // Validate that level IDs are unique
            AddValidationRule("UniqueLevelIds", "Check that all level IDs are unique", true, null, (json) =>
            {
                var jsonNode = ParseJson(json);
                var obj = jsonNode.AsObject();
                
                if (!obj.ContainsKey("levels"))
                {
                    return true; // No levels to validate
                }
                
                var levelsArray = obj["levels"].AsArray();
                var levelIds = new HashSet<string>();
                
                for (int i = 0; i < levelsArray.Count; i++)
                {
                    var levelNode = levelsArray[i].AsObject();
                    string levelId = levelNode["levelId"].Value;
                    
                    if (levelIds.Contains(levelId))
                    {
                        return false; // Duplicate ID found
                    }
                    
                    levelIds.Add(levelId);
                }
                
                return true;
            });
            
            // Validate that numeric values are within expected ranges
            AddValidationRule("NumericRanges", "Check that numeric values are within expected ranges", false, null, (json) =>
            {
                var jsonNode = ParseJson(json);
                var obj = jsonNode.AsObject();
                
                if (!obj.ContainsKey("levels"))
                {
                    return true; // No levels to validate
                }
                
                var levelsArray = obj["levels"].AsArray();
                
                for (int i = 0; i < levelsArray.Count; i++)
                {
                    var levelNode = levelsArray[i].AsObject();
                    
                    // Validate stars achieved (0-3)
                    if (levelNode.ContainsKey("starsAchieved"))
                    {
                        if (!int.TryParse(levelNode["starsAchieved"].Value, out int stars) || stars < 0 || stars > 3)
                        {
                            return false;
                        }
                    }
                    
                    // Validate best time (non-negative)
                    if (levelNode.ContainsKey("bestTimeSeconds"))
                    {
                        if (!float.TryParse(levelNode["bestTimeSeconds"].Value, out float time) || time < 0)
                        {
                            return false;
                        }
                    }
                }
                
                return true;
            });
            
            // Add more validation rules as needed
        }
        
        /// <summary>
        /// Gets the migration path from one version to another
        /// </summary>
        /// <param name="fromVersion">Starting version</param>
        /// <param name="toVersion">Target version</param>
        /// <returns>List of versions representing the migration path</returns>
        private List<string> GetMigrationPath(string fromVersion, string toVersion)
        {
            // Build a graph of all possible migrations
            var graph = new Dictionary<string, List<string>>();
            
            foreach (var rule in migrationRules.Values)
            {
                if (!graph.ContainsKey(rule.fromVersion))
                {
                    graph[rule.fromVersion] = new List<string>();
                }
                
                graph[rule.fromVersion].Add(rule.toVersion);
            }
            
            // Find shortest path using BFS
            var queue = new Queue<string>();
            var visited = new HashSet<string>();
            var parent = new Dictionary<string, string>();
            
            queue.Enqueue(fromVersion);
            visited.Add(fromVersion);
            
            while (queue.Count > 0)
            {
                string current = queue.Dequeue();
                
                if (current == toVersion)
                {
                    // Reconstruct path
                    var path = new List<string>();
                    string node = toVersion;
                    
                    while (node != fromVersion)
                    {
                        path.Insert(0, node);
                        node = parent[node];
                    }
                    
                    path.Insert(0, fromVersion);
                    return path;
                }
                
                if (graph.ContainsKey(current))
                {
                    foreach (string neighbor in graph[current])
                    {
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            parent[neighbor] = current;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }
            
            // No path found
            return new List<string>();
        }
        
        /// <summary>
        /// Checks if a string is valid JSON
        /// </summary>
        /// <param name="json">String to check</param>
        /// <returns>True if the string is valid JSON</returns>
        private bool IsValidJson(string json)
        {
            if (string.IsNullOrEmpty(json))
                return false;
                
            try
            {
                ParseJson(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Simple JSON parser that converts a JSON string to our SimpleJSON structure
        /// </summary>
        /// <param name="jsonString">JSON string to parse</param>
        /// <returns>SimpleJSON object</returns>
        private SimpleJSON ParseJson(string jsonString)
        {
            var result = new SimpleJSON();
            result.AsObject();
            
            try
            {
                // Remove whitespace
                jsonString = jsonString.Trim();
                
                // Check if it's an object
                if (jsonString.StartsWith("{") && jsonString.EndsWith("}"))
                {
                    // Remove braces
                    jsonString = jsonString.Substring(1, jsonString.Length - 2);
                    
                    // Split by commas but not within quotes
                    var parts = SplitJsonString(jsonString, ',');
                    
                    foreach (var part in parts)
                    {
                        var keyValue = SplitJsonString(part, ':');
                        if (keyValue.Count == 2)
                        {
                            string key = keyValue[0].Trim().Trim('"');
                            string value = keyValue[1].Trim().Trim('"');
                            result.data[key] = value;
                        }
                    }
                }
                // Check if it's an array
                else if (jsonString.StartsWith("[") && jsonString.EndsWith("]"))
                {
                    result.AsArray();
                    
                    // Remove brackets
                    jsonString = jsonString.Substring(1, jsonString.Length - 2);
                    
                    // Split by commas but not within quotes
                    var parts = SplitJsonString(jsonString, ',');
                    
                    foreach (var part in parts)
                    {
                        var element = ParseJson(part);
                        result.array.Add(element);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse JSON: {e.Message}");
            }
            
            return result;
        }
        
        /// <summary>
        /// Helper method to split JSON string by delimiter, ignoring delimiters within quotes
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="delimiter">Delimiter character</param>
        /// <returns>List of split parts</returns>
        private List<string> SplitJsonString(string input, char delimiter)
        {
            var result = new List<string>();
            var current = new System.Text.StringBuilder();
            bool inQuotes = false;
            
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == delimiter && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                    continue;
                }
                
                current.Append(c);
            }
            
            // Add the last part
            if (current.Length > 0)
            {
                result.Add(current.ToString());
            }
            
            return result;
        }
        
        #endregion
        
        #region Editor Methods
        
#if UNITY_EDITOR
        /// <summary>
        /// Called when the script is loaded or a value is changed in the inspector
        /// </summary>
        private void OnValidate()
        {
            // Validate version format (should be semantic versioning like 1.0.0)
            if (!Regex.IsMatch(currentDataVersion, @"^\d+\.\d+\.\d+$"))
            {
                Debug.LogWarning("DataValidationAndMigration: Current data version should be in semantic versioning format (e.g., 1.0.0)");
            }
        }
        
        /// <summary>
        /// Creates a menu item to print migration rules
        /// </summary>
        [UnityEditor.MenuItem("MixDrop/Debug/Print Migration Rules")]
        private static void PrintMigrationRules()
        {
            DataValidationAndMigration[] instances = FindObjectsOfType<DataValidationAndMigration>();
            
            if (instances.Length == 0)
            {
                UnityEditor.EditorUtility.DisplayDialog(
                    "Migration Rules",
                    "No DataValidationAndMigration found in the scene.",
                    "OK"
                );
                return;
            }
            
            foreach (DataValidationAndMigration instance in instances)
            {
                if (!instance.isInitialized)
                {
                    instance.Initialize();
                }
                
                string info = $"Migration Rules for {instance.name}:\n\n";
                
                foreach (var rule in instance.migrationRules.Values)
                {
                    info += $"{rule.fromVersion} -> {rule.toVersion}: {rule.description}\n";
                    info += $"  Required: {rule.isRequired}\n\n";
                }
                
                Debug.Log(info);
            }
            
            UnityEditor.EditorUtility.DisplayDialog(
                "Migration Rules",
                "Migration rules have been printed to the console.",
                "OK"
            );
        }
        
        /// <summary>
        /// Creates a menu item to print validation rules
        /// </summary>
        [UnityEditor.MenuItem("MixDrop/Debug/Print Validation Rules")]
        private static void PrintValidationRules()
        {
            DataValidationAndMigration[] instances = FindObjectsOfType<DataValidationAndMigration>();
            
            if (instances.Length == 0)
            {
                UnityEditor.EditorUtility.DisplayDialog(
                    "Validation Rules",
                    "No DataValidationAndMigration found in the scene.",
                    "OK"
                );
                return;
            }
            
            foreach (DataValidationAndMigration instance in instances)
            {
                if (!instance.isInitialized)
                {
                    instance.Initialize();
                }
                
                string info = $"Validation Rules for {instance.name}:\n\n";
                
                foreach (var rule in instance.validationRules.Values)
                {
                    info += $"{rule.name}: {rule.description}\n";
                    info += $"  Required: {rule.isRequired}\n";
                    info += $"  Applies to: {rule.appliesToVersion ?? "All versions"}\n\n";
                }
                
                Debug.Log(info);
            }
            
            UnityEditor.EditorUtility.DisplayDialog(
                "Validation Rules",
                "Validation rules have been printed to the console.",
                "OK"
            );
        }
        
        /// <summary>
        /// Creates a menu item to test data validation and migration
        /// </summary>
        [UnityEditor.MenuItem("MixDrop/Debug/Test Data Validation and Migration")]
        private static async Task TestDataValidationAndMigration()
        {
            DataValidationAndMigration[] instances = FindObjectsOfType<DataValidationAndMigration>();
            
            if (instances.Length == 0)
            {
                UnityEditor.EditorUtility.DisplayDialog(
                    "Test Data Validation and Migration",
                    "No DataValidationAndMigration found in the scene.",
                    "OK"
                );
                return;
            }
            
            foreach (DataValidationAndMigration instance in instances)
            {
                if (!instance.isInitialized)
                {
                    instance.Initialize();
                }
                
                // Create test data (old version)
                string testData = @"{
                    ""levels"": [
                        {
                            ""id"": ""level1"",
                            ""stars"": 2,
                            ""bestTime"": 120.5
                        },
                        {
                            ""id"": ""level2"",
                            ""stars"": 1,
                            ""bestTime"": 180.0
                        }
                    ]
                }";
                
                // Test validation and migration
                var result = await instance.ValidateAndMigrateDataAsync(testData, "0.9.0");
                
                string info = $"Test Results for {instance.name}:\n\n";
                info += $"Validation: {(result.validationResult.isValid ? "Success" : "Failed")}\n";
                
                if (result.validationResult.errorMessages.Count > 0)
                {
                    info += "Errors:\n";
                    foreach (string error in result.validationResult.errorMessages)
                    {
                        info += $"  - {error}\n";
                    }
                }
                
                if (result.validationResult.warningMessages.Count > 0)
                {
                    info += "Warnings:\n";
                    foreach (string warning in result.validationResult.warningMessages)
                    {
                        info += $"  - {warning}\n";
                    }
                }
                
                info += $"\nMigration: {(result.migrationResult.success ? "Success" : "Failed")}\n";
                
                if (result.migrationResult.messages.Count > 0)
                {
                    info += "Messages:\n";
                    foreach (string message in result.migrationResult.messages)
                    {
                        info += $"  - {message}\n";
                    }
                }
                
                info += $"\nFinal Data:\n{result.finalData}";
                
                Debug.Log(info);
            }
            
            UnityEditor.EditorUtility.DisplayDialog(
                "Test Data Validation and Migration",
                "Test results have been printed to the console.",
                "OK"
            );
        }
#endif
        
        #endregion
    }
}