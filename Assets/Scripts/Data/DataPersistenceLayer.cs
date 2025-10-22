using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.IO.Compression;

namespace MixDrop.Data
{
    /// <summary>
    /// Handles data persistence for the game, including saving, loading,
    /// encryption, and cloud sync capabilities.
    /// </summary>
    public class DataPersistenceLayer : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Whether to encrypt save data")]
        [SerializeField] private bool encryptData = true;
        
        [Tooltip("Encryption key (must be 16, 24, or 32 characters for AES)")]
        [SerializeField] private string encryptionKey = "MixDropGameKey123";
        
        [Tooltip("Whether to enable cloud save synchronization")]
        [SerializeField] private bool enableCloudSync = false;
        
        [Tooltip("Maximum number of local backup files to keep")]
        [Range(1, 10)]
        [SerializeField] private int maxBackupFiles = 3;
        
        [Tooltip("Whether to compress save data")]
        [SerializeField] private bool compressData = true;
        
        [Header("Events")]
        [Tooltip("Event triggered when data is successfully saved")]
        [SerializeField] private UnityEngine.Events.UnityEvent onSaveSuccess;
        
        [Tooltip("Event triggered when data fails to save")]
        [SerializeField] private UnityEngine.Events.UnityEvent<string> onSaveFailure;
        
        [Tooltip("Event triggered when data is successfully loaded")]
        [SerializeField] private UnityEngine.Events.UnityEvent onLoadSuccess;
        
        [Tooltip("Event triggered when data fails to load")]
        [SerializeField] private UnityEngine.Events.UnityEvent<string> onLoadFailure;
        
        [Tooltip("Event triggered when cloud sync completes")]
        [SerializeField] private UnityEngine.Events.UnityEvent<bool> onCloudSyncComplete;
        
        // Private fields
        private string saveFilePath;
        private string backupDirectoryPath;
        private bool isInitialized = false;
        
        #region Constants
        
        private const string SAVE_FILE_NAME = "MixDrop_SaveData.json";
        private const string BACKUP_DIRECTORY_NAME = "MixDrop_Backups";
        private const string CLOUD_SAVE_KEY = "MixDrop_CloudSaveData";
        private const string ENCRYPTION_PREFIX = "ENC_";
        private const string COMPRESSION_PREFIX = "COMP_";
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Gets whether the persistence layer is initialized
        /// </summary>
        public bool IsInitialized => isInitialized;
        
        /// <summary>
        /// Gets the full path to the save file
        /// </summary>
        public string SaveFilePath => saveFilePath;
        
        /// <summary>
        /// Gets the full path to the backup directory
        /// </summary>
        public string BackupDirectoryPath => backupDirectoryPath;
        
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
        /// Initializes the data persistence layer
        /// </summary>
        public void Initialize()
        {
            if (isInitialized)
                return;
                
            // Set up file paths
            SetupFilePaths();
            
            // Create backup directory if it doesn't exist
            if (!Directory.Exists(backupDirectoryPath))
            {
                Directory.CreateDirectory(backupDirectoryPath);
            }
            
            // Validate encryption key
            if (encryptData && !IsValidEncryptionKey(encryptionKey))
            {
                Debug.LogWarning("DataPersistenceLayer: Invalid encryption key. Disabling encryption.");
                encryptData = false;
            }
            
            isInitialized = true;
            
            Debug.Log("DataPersistenceLayer: Initialized successfully.");
        }
        
        /// <summary>
        /// Saves data to persistent storage
        /// </summary>
        /// <typeparam name="T">Type of data to save</typeparam>
        /// <param name="data">Data to save</param>
        /// <param name="createBackup">Whether to create a backup before saving</param>
        /// <returns>Task representing the save operation</returns>
        public async Task<bool> SaveDataAsync<T>(T data, bool createBackup = true)
        {
            if (!isInitialized)
            {
                string error = "DataPersistenceLayer: Not initialized. Call Initialize() first.";
                Debug.LogError(error);
                onLoadFailure?.Invoke(error);
                return false;
            }
            
            try
            {
                // Convert data to JSON
                string json = JsonUtility.ToJson(data, true);
                
                // Compress data if enabled
                if (compressData)
                {
                    json = CompressString(json);
                    json = COMPRESSION_PREFIX + json;
                }
                
                // Encrypt data if enabled
                if (encryptData)
                {
                    json = EncryptString(json, encryptionKey);
                    json = ENCRYPTION_PREFIX + json;
                }
                
                // Create backup if requested
                if (createBackup && File.Exists(saveFilePath))
                {
                    CreateBackupFile();
                }
                
                // Write data to file asynchronously
                await Task.Run(() => File.WriteAllText(saveFilePath, json));
                
                // Save to cloud if enabled
                if (enableCloudSync)
                {
                    await SaveToCloudAsync(json);
                }
                
                // Trigger success event
                onSaveSuccess?.Invoke();
                
                Debug.Log("DataPersistenceLayer: Data saved successfully.");
                return true;
            }
            catch (Exception e)
            {
                string error = $"DataPersistenceLayer: Failed to save data: {e.Message}";
                Debug.LogError(error);
                onSaveFailure?.Invoke(error);
                return false;
            }
        }
        
        /// <summary>
        /// Loads data from persistent storage
        /// </summary>
        /// <typeparam name="T">Type of data to load</typeparam>
        /// <param name="loadFromCloud">Whether to attempt loading from cloud if local file doesn't exist</param>
        /// <returns>Task representing the load operation with the loaded data</returns>
        public async Task<(bool success, T data)> LoadDataAsync<T>(bool loadFromCloud = true) where T : new()
        {
            if (!isInitialized)
            {
                string error = "DataPersistenceLayer: Not initialized. Call Initialize() first.";
                Debug.LogError(error);
                onLoadFailure?.Invoke(error);
                return (false, new T());
            }
            
            try
            {
                string json = null;
                
                // Try to load from local file
                if (File.Exists(saveFilePath))
                {
                    json = await Task.Run(() => File.ReadAllText(saveFilePath));
                }
                // If local file doesn't exist and cloud sync is enabled, try to load from cloud
                else if (loadFromCloud && enableCloudSync)
                {
                    json = await LoadFromCloudAsync();
                    
                    if (!string.IsNullOrEmpty(json))
                    {
                        // Save cloud data to local file
                        await Task.Run(() => File.WriteAllText(saveFilePath, json));
                    }
                }
                
                // If no data found, return default
                if (string.IsNullOrEmpty(json))
                {
                    Debug.Log("DataPersistenceLayer: No save data found.");
                    onLoadSuccess?.Invoke();
                    return (true, new T());
                }
                
                // Decrypt data if encrypted
                if (json.StartsWith(ENCRYPTION_PREFIX))
                {
                    json = json.Substring(ENCRYPTION_PREFIX.Length);
                    json = DecryptString(json, encryptionKey);
                }
                
                // Decompress data if compressed
                if (json.StartsWith(COMPRESSION_PREFIX))
                {
                    json = json.Substring(COMPRESSION_PREFIX.Length);
                    json = DecompressString(json);
                }
                
                // Parse JSON
                T data = JsonUtility.FromJson<T>(json);
                
                // Trigger success event
                onLoadSuccess?.Invoke();
                
                Debug.Log("DataPersistenceLayer: Data loaded successfully.");
                return (true, data);
            }
            catch (Exception e)
            {
                string error = $"DataPersistenceLayer: Failed to load data: {e.Message}";
                Debug.LogError(error);
                onLoadFailure?.Invoke(error);
                
                // Try to load from backup
                Debug.Log("DataPersistenceLayer: Attempting to load from backup...");
                return await LoadFromBackupAsync<T>();
            }
        }
        
        /// <summary>
        /// Deletes the save file
        /// </summary>
        /// <returns>True if deletion was successful</returns>
        public bool DeleteSaveFile()
        {
            if (!isInitialized)
            {
                Debug.LogError("DataPersistenceLayer: Not initialized. Call Initialize() first.");
                return false;
            }
            
            try
            {
                if (File.Exists(saveFilePath))
                {
                    File.Delete(saveFilePath);
                    Debug.Log("DataPersistenceLayer: Save file deleted successfully.");
                    return true;
                }
                
                Debug.Log("DataPersistenceLayer: No save file to delete.");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"DataPersistenceLayer: Failed to delete save file: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Checks if a save file exists
        /// </summary>
        /// <returns>True if save file exists</returns>
        public bool SaveFileExists()
        {
            if (!isInitialized)
            {
                Debug.LogError("DataPersistenceLayer: Not initialized. Call Initialize() first.");
                return false;
            }
            
            return File.Exists(saveFilePath);
        }
        
        /// <summary>
        /// Gets the size of the save file in bytes
        /// </summary>
        /// <returns>Size of save file in bytes, or 0 if file doesn't exist</returns>
        public long GetSaveFileSize()
        {
            if (!isInitialized)
            {
                Debug.LogError("DataPersistenceLayer: Not initialized. Call Initialize() first.");
                return 0;
            }
            
            try
            {
                if (File.Exists(saveFilePath))
                {
                    FileInfo fileInfo = new FileInfo(saveFilePath);
                    return fileInfo.Length;
                }
                
                return 0;
            }
            catch (Exception e)
            {
                Debug.LogError($"DataPersistenceLayer: Failed to get save file size: {e.Message}");
                return 0;
            }
        }
        
        /// <summary>
        /// Gets the last modified time of the save file
        /// </summary>
        /// <returns>Last modified time, or DateTime.MinValue if file doesn't exist</returns>
        public DateTime GetSaveFileLastModified()
        {
            if (!isInitialized)
            {
                Debug.LogError("DataPersistenceLayer: Not initialized. Call Initialize() first.");
                return DateTime.MinValue;
            }
            
            try
            {
                if (File.Exists(saveFilePath))
                {
                    FileInfo fileInfo = new FileInfo(saveFilePath);
                    return fileInfo.LastWriteTime;
                }
                
                return DateTime.MinValue;
            }
            catch (Exception e)
            {
                Debug.LogError($"DataPersistenceLayer: Failed to get save file last modified time: {e.Message}");
                return DateTime.MinValue;
            }
        }
        
        /// <summary>
        /// Creates a backup of the current save file
        /// </summary>
        /// <returns>True if backup was successful</returns>
        public bool CreateBackupFile()
        {
            if (!isInitialized)
            {
                Debug.LogError("DataPersistenceLayer: Not initialized. Call Initialize() first.");
                return false;
            }
            
            try
            {
                if (!File.Exists(saveFilePath))
                {
                    Debug.LogWarning("DataPersistenceLayer: No save file to backup.");
                    return false;
                }
                
                // Create backup file name with timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFileName = $"Backup_{timestamp}_{Path.GetFileName(saveFilePath)}";
                string backupFilePath = Path.Combine(backupDirectoryPath, backupFileName);
                
                // Copy save file to backup location
                File.Copy(saveFilePath, backupFilePath, true);
                
                // Clean up old backup files if we have too many
                CleanupOldBackupFiles();
                
                Debug.Log($"DataPersistenceLayer: Backup created at {backupFilePath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"DataPersistenceLayer: Failed to create backup: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Gets a list of all backup files
        /// </summary>
        /// <returns>List of backup file information</returns>
        public List<BackupFileInfo> GetBackupFiles()
        {
            List<BackupFileInfo> backupFiles = new List<BackupFileInfo>();
            
            if (!isInitialized)
            {
                Debug.LogError("DataPersistenceLayer: Not initialized. Call Initialize() first.");
                return backupFiles;
            }
            
            try
            {
                if (!Directory.Exists(backupDirectoryPath))
                {
                    return backupFiles;
                }
                
                DirectoryInfo directoryInfo = new DirectoryInfo(backupDirectoryPath);
                FileInfo[] files = directoryInfo.GetFiles("Backup_*");
                
                foreach (FileInfo file in files)
                {
                    backupFiles.Add(new BackupFileInfo
                    {
                        FileName = file.Name,
                        FilePath = file.FullName,
                        SizeBytes = file.Length,
                        LastModified = file.LastWriteTime
                    });
                }
                
                // Sort by last modified time (newest first)
                backupFiles.Sort((a, b) => b.LastModified.CompareTo(a.LastModified));
                
                return backupFiles;
            }
            catch (Exception e)
            {
                Debug.LogError($"DataPersistenceLayer: Failed to get backup files: {e.Message}");
                return backupFiles;
            }
        }
        
        /// <summary>
        /// Restores a backup file
        /// </summary>
        /// <param name="backupFilePath">Path to the backup file to restore</param>
        /// <returns>True if restore was successful</returns>
        public bool RestoreBackupFile(string backupFilePath)
        {
            if (!isInitialized)
            {
                Debug.LogError("DataPersistenceLayer: Not initialized. Call Initialize() first.");
                return false;
            }
            
            try
            {
                if (!File.Exists(backupFilePath))
                {
                    Debug.LogError($"DataPersistenceLayer: Backup file not found: {backupFilePath}");
                    return false;
                }
                
                // Create backup of current save file before restoring
                if (File.Exists(saveFilePath))
                {
                    CreateBackupFile();
                }
                
                // Copy backup file to save file location
                File.Copy(backupFilePath, saveFilePath, true);
                
                Debug.Log($"DataPersistenceLayer: Backup restored from {backupFilePath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"DataPersistenceLayer: Failed to restore backup: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Manually triggers cloud synchronization
        /// </summary>
        /// <returns>Task representing the sync operation</returns>
        public async Task<bool> SyncWithCloudAsync()
        {
            if (!isInitialized)
            {
                Debug.LogError("DataPersistenceLayer: Not initialized. Call Initialize() first.");
                return false;
            }
            
            if (!enableCloudSync)
            {
                Debug.LogWarning("DataPersistenceLayer: Cloud sync is disabled.");
                onCloudSyncComplete?.Invoke(false);
                return false;
            }
            
            try
            {
                // Get local save data
                string localData = null;
                if (File.Exists(saveFilePath))
                {
                    localData = await Task.Run(() => File.ReadAllText(saveFilePath));
                }
                
                // Get cloud save data
                string cloudData = await LoadFromCloudAsync();
                
                // Determine which data is newer
                DateTime localModified = GetSaveFileLastModified();
                DateTime cloudModified = GetCloudSaveLastModified();
                
                bool syncSuccess = false;
                
                if (localModified > cloudModified)
                {
                    // Local is newer, save to cloud
                    if (!string.IsNullOrEmpty(localData))
                    {
                        syncSuccess = await SaveToCloudAsync(localData);
                    }
                }
                else if (cloudModified > localModified)
                {
                    // Cloud is newer, save to local
                    if (!string.IsNullOrEmpty(cloudData))
                    {
                        await Task.Run(() => File.WriteAllText(saveFilePath, cloudData));
                        syncSuccess = true;
                    }
                }
                else
                {
                    // Both are the same, no sync needed
                    syncSuccess = true;
                }
                
                onCloudSyncComplete?.Invoke(syncSuccess);
                return syncSuccess;
            }
            catch (Exception e)
            {
                Debug.LogError($"DataPersistenceLayer: Cloud sync failed: {e.Message}");
                onCloudSyncComplete?.Invoke(false);
                return false;
            }
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Sets up file paths for save data and backups
        /// </summary>
        private void SetupFilePaths()
        {
            // Get persistent data path
            string persistentPath = Application.persistentDataPath;
            
            // Set save file path
            saveFilePath = Path.Combine(persistentPath, SAVE_FILE_NAME);
            
            // Set backup directory path
            backupDirectoryPath = Path.Combine(persistentPath, BACKUP_DIRECTORY_NAME);
            
            Debug.Log($"DataPersistenceLayer: Save file path: {saveFilePath}");
            Debug.Log($"DataPersistenceLayer: Backup directory path: {backupDirectoryPath}");
        }
        
        /// <summary>
        /// Validates that the encryption key is a valid length for AES
        /// </summary>
        /// <param name="key">Encryption key to validate</param>
        /// <returns>True if key is valid</returns>
        private bool IsValidEncryptionKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;
                
            // AES supports 128-bit (16 characters), 192-bit (24 characters), and 256-bit (32 characters) keys
            return key.Length == 16 || key.Length == 24 || key.Length == 32;
        }
        
        /// <summary>
        /// Encrypts a string using AES encryption
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="key">Encryption key</param>
        /// <returns>Encrypted string as Base64</returns>
        private string EncryptString(string plainText, string key)
        {
            byte[] iv = new byte[16];
            byte[] array;
            
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }
                        
                        array = memoryStream.ToArray();
                    }
                }
            }
            
            return Convert.ToBase64String(array);
        }
        
        /// <summary>
        /// Decrypts a string using AES decryption
        /// </summary>
        /// <param name="cipherText">Encrypted text as Base64</param>
        /// <param name="key">Decryption key</param>
        /// <returns>Decrypted string</returns>
        private string DecryptString(string cipherText, string key)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Compresses a string using GZip compression
        /// </summary>
        /// <param name="text">Text to compress</param>
        /// <returns>Compressed string as Base64</returns>
        private string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                {
                    gzipStream.Write(buffer, 0, buffer.Length);
                }
                
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }
        
        /// <summary>
        /// Decompresses a string using GZip decompression
        /// </summary>
        /// <param name="compressedText">Compressed text as Base64</param>
        /// <returns>Decompressed string</returns>
        private string DecompressString(string compressedText)
        {
            byte[] buffer = Convert.FromBase64String(compressedText);
            
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    using (MemoryStream resultStream = new MemoryStream())
                    {
                        gzipStream.CopyTo(resultStream);
                        return Encoding.UTF8.GetString(resultStream.ToArray());
                    }
                }
            }
        }
        
        /// <summary>
        /// Saves data to cloud storage
        /// </summary>
        /// <param name="data">Data to save</param>
        /// <returns>Task representing the save operation</returns>
        private async Task<bool> SaveToCloudAsync(string data)
        {
            // In a real implementation, this would integrate with a cloud service like:
            // - Google Play Games for Android
            // - iCloud for iOS
            // - Steam Cloud for PC
            // - A custom backend service
            
            // For this example, we'll just save to PlayerPrefs as a simulation
            try
            {
                await Task.Run(() => PlayerPrefs.SetString(CLOUD_SAVE_KEY, data));
                PlayerPrefs.Save();
                
                Debug.Log("DataPersistenceLayer: Data saved to cloud successfully.");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"DataPersistenceLayer: Failed to save to cloud: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Loads data from cloud storage
        /// </summary>
        /// <returns>Task representing the load operation with the loaded data</returns>
        private async Task<string> LoadFromCloudAsync()
        {
            // In a real implementation, this would integrate with a cloud service
            
            // For this example, we'll just load from PlayerPrefs as a simulation
            try
            {
                string data = await Task.Run(() => PlayerPrefs.GetString(CLOUD_SAVE_KEY, ""));
                
                if (!string.IsNullOrEmpty(data))
                {
                    Debug.Log("DataPersistenceLayer: Data loaded from cloud successfully.");
                }
                
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"DataPersistenceLayer: Failed to load from cloud: {e.Message}");
                return null;
            }
        }
        
        /// <summary>
        /// Gets the last modified time of the cloud save
        /// </summary>
        /// <returns>Last modified time, or DateTime.MinValue if not available</returns>
        private DateTime GetCloudSaveLastModified()
        {
            // In a real implementation, this would get the actual last modified time from the cloud service
            
            // For this example, we'll just return a default value
            return DateTime.MinValue;
        }
        
        /// <summary>
        /// Cleans up old backup files, keeping only the most recent ones
        /// </summary>
        private void CleanupOldBackupFiles()
        {
            try
            {
                if (!Directory.Exists(backupDirectoryPath))
                    return;
                    
                DirectoryInfo directoryInfo = new DirectoryInfo(backupDirectoryPath);
                FileInfo[] files = directoryInfo.GetFiles("Backup_*");
                
                // Sort by last modified time (oldest first)
                Array.Sort(files, (a, b) => a.LastWriteTime.CompareTo(b.LastWriteTime));
                
                // Delete oldest files if we have too many
                int filesToDelete = files.Length - maxBackupFiles;
                for (int i = 0; i < filesToDelete; i++)
                {
                    files[i].Delete();
                    Debug.Log($"DataPersistenceLayer: Deleted old backup file: {files[i].Name}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"DataPersistenceLayer: Failed to cleanup old backup files: {e.Message}");
            }
        }
        
        /// <summary>
        /// Loads data from the most recent backup file
        /// </summary>
        /// <typeparam name="T">Type of data to load</typeparam>
        /// <returns>Task representing the load operation with the loaded data</returns>
        private async Task<(bool success, T data)> LoadFromBackupAsync<T>() where T : new()
        {
            try
            {
                List<BackupFileInfo> backupFiles = GetBackupFiles();
                
                if (backupFiles.Count == 0)
                {
                    Debug.LogWarning("DataPersistenceLayer: No backup files found.");
                    return (false, new T());
                }
                
                // Get the most recent backup file
                BackupFileInfo mostRecentBackup = backupFiles[0];
                
                // Read backup file
                string json = await Task.Run(() => File.ReadAllText(mostRecentBackup.FilePath));
                
                // Decrypt data if encrypted
                if (json.StartsWith(ENCRYPTION_PREFIX))
                {
                    json = json.Substring(ENCRYPTION_PREFIX.Length);
                    json = DecryptString(json, encryptionKey);
                }
                
                // Decompress data if compressed
                if (json.StartsWith(COMPRESSION_PREFIX))
                {
                    json = json.Substring(COMPRESSION_PREFIX.Length);
                    json = DecompressString(json);
                }
                
                // Parse JSON
                T data = JsonUtility.FromJson<T>(json);
                
                Debug.Log($"DataPersistenceLayer: Data loaded from backup: {mostRecentBackup.FileName}");
                return (true, data);
            }
            catch (Exception e)
            {
                Debug.LogError($"DataPersistenceLayer: Failed to load from backup: {e.Message}");
                return (false, new T());
            }
        }
        
        #endregion
        
        #region Nested Classes
        
        /// <summary>
        /// Information about a backup file
        /// </summary>
        [System.Serializable]
        public class BackupFileInfo
        {
            public string FileName;
            public string FilePath;
            public long SizeBytes;
            public DateTime LastModified;
            
            /// <summary>
            /// Gets the file size in a human-readable format
            /// </summary>
            public string FormattedSize
            {
                get
                {
                    string[] sizes = { "B", "KB", "MB", "GB" };
                    double len = SizeBytes;
                    int order = 0;
                    
                    while (len >= 1024 && order < sizes.Length - 1)
                    {
                        order++;
                        len = len / 1024;
                    }
                    
                    return $"{len:0.##} {sizes[order]}";
                }
            }
            
            /// <summary>
            /// Gets the last modified time in a human-readable format
            /// </summary>
            public string FormattedLastModified
            {
                get
                {
                    return LastModified.ToString("yyyy-MM-dd HH:mm:ss");
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
            // Validate max backup files
            maxBackupFiles = Mathf.Clamp(maxBackupFiles, 1, 10);
        }
        
        /// <summary>
        /// Creates a menu item to open the save data folder
        /// </summary>
        [UnityEditor.MenuItem("MixDrop/Debug/Open Save Data Folder")]
        private static void OpenSaveDataFolder()
        {
            string path = Application.persistentDataPath;
            
            if (Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
            else
            {
                UnityEditor.EditorUtility.DisplayDialog(
                    "Open Save Data Folder",
                    "Save data folder does not exist yet.",
                    "OK"
                );
            }
        }
        
        /// <summary>
        /// Creates a menu item to delete all save data
        /// </summary>
        [UnityEditor.MenuItem("MixDrop/Debug/Delete All Save Data")]
        private static void DeleteAllSaveData()
        {
            if (UnityEditor.EditorUtility.DisplayDialog(
                "Delete All Save Data",
                "Are you sure you want to delete all save data? This action cannot be undone.",
                "Delete",
                "Cancel"
            ))
            {
                DataPersistenceLayer[] persistenceLayers = FindObjectsOfType<DataPersistenceLayer>();
                
                foreach (DataPersistenceLayer persistenceLayer in persistenceLayers)
                {
                    if (!persistenceLayer.isInitialized)
                    {
                        persistenceLayer.Initialize();
                    }
                    
                    // Delete save file
                    persistenceLayer.DeleteSaveFile();
                    
                    // Delete backup files
                    List<BackupFileInfo> backupFiles = persistenceLayer.GetBackupFiles();
                    foreach (BackupFileInfo backupFile in backupFiles)
                    {
                        try
                        {
                            File.Delete(backupFile.FilePath);
                            Debug.Log($"Deleted backup file: {backupFile.FileName}");
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Failed to delete backup file {backupFile.FileName}: {e.Message}");
                        }
                    }
                    
                    // Delete cloud save
                    if (persistenceLayer.enableCloudSync)
                    {
                        PlayerPrefs.DeleteKey(CLOUD_SAVE_KEY);
                        PlayerPrefs.Save();
                    }
                }
                
                UnityEditor.EditorUtility.DisplayDialog(
                    "Delete All Save Data",
                    "All save data has been deleted.",
                    "OK"
                );
            }
        }
        
        /// <summary>
        /// Creates a menu item to print save data information
        /// </summary>
        [UnityEditor.MenuItem("MixDrop/Debug/Print Save Data Information")]
        private static void PrintSaveDataInformation()
        {
            DataPersistenceLayer[] persistenceLayers = FindObjectsOfType<DataPersistenceLayer>();
            
            if (persistenceLayers.Length == 0)
            {
                UnityEditor.EditorUtility.DisplayDialog(
                    "Save Data Information",
                    "No DataPersistenceLayer found in the scene.",
                    "OK"
                );
                return;
            }
            
            foreach (DataPersistenceLayer persistenceLayer in persistenceLayers)
            {
                if (!persistenceLayer.isInitialized)
                {
                    persistenceLayer.Initialize();
                }
                
                string info = $"Save Data Information for {persistenceLayer.name}:\n\n";
                info += $"Save File Path: {persistenceLayer.SaveFilePath}\n";
                info += $"Save File Exists: {persistenceLayer.SaveFileExists()}\n";
                info += $"Save File Size: {persistenceLayer.GetSaveFileSize()} bytes\n";
                info += $"Save File Last Modified: {persistenceLayer.GetSaveFileLastModified()}\n";
                info += $"Backup Directory: {persistenceLayer.BackupDirectoryPath}\n";
                info += $"Encryption Enabled: {persistenceLayer.encryptData}\n";
                info += $"Cloud Sync Enabled: {persistenceLayer.enableCloudSync}\n\n";
                
                info += "Backup Files:\n";
                List<BackupFileInfo> backupFiles = persistenceLayer.GetBackupFiles();
                
                if (backupFiles.Count == 0)
                {
                    info += "  No backup files found.\n";
                }
                else
                {
                    foreach (BackupFileInfo backupFile in backupFiles)
                    {
                        info += $"  {backupFile.FileName}: {backupFile.FormattedSize}, {backupFile.FormattedLastModified}\n";
                    }
                }
                
                Debug.Log(info);
            }
            
            UnityEditor.EditorUtility.DisplayDialog(
                "Save Data Information",
                "Save data information has been printed to the console.",
                "OK"
            );
        }
#endif
        
        #endregion
    }
}