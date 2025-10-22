using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Implementation of the progress repository for accessing player progress data
/// </summary>
public class ProgressRepository : IProgressRepository
{
    private const string PROGRESS_KEY_PREFIX = "LevelProgress_";
    private const string TOTAL_STARS_KEY = "TotalStarsEarned";

    private Dictionary<int, PlayerLevelProgress> _progressCache;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the ProgressRepository class
    /// </summary>
    public ProgressRepository()
    {
        _progressCache = new Dictionary<int, PlayerLevelProgress>();
        _isInitialized = false;
    }

    /// <summary>
    /// Gets progress data for a specific level
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <returns>The progress data if found, null otherwise</returns>
    public PlayerLevelProgress GetProgress(int levelId)
    {
        EnsureInitialized();
        return _progressCache.TryGetValue(levelId, out var progress) ? progress : null;
    }

    /// <summary>
    /// Gets progress data for all levels
    /// </summary>
    /// <returns>A dictionary mapping level IDs to progress data</returns>
    public Dictionary<int, PlayerLevelProgress> GetAllProgress()
    {
        EnsureInitialized();
        return new Dictionary<int, PlayerLevelProgress>(_progressCache);
    }

    /// <summary>
    /// Saves progress data for a specific level
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <param name="progress">The progress data to save</param>
    /// <returns>A task that represents the save operation</returns>
    public async Task SaveProgressAsync(int levelId, PlayerLevelProgress progress)
    {
        if (progress == null)
            throw new System.ArgumentNullException(nameof(progress));

        EnsureInitialized();
        _progressCache[levelId] = progress;
        await SaveProgressToStorageAsync(levelId, progress);
    }

    /// <summary>
    /// Updates progress data for a specific level
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <param name="progress">The updated progress data</param>
    /// <returns>A task that represents the update operation</returns>
    public async Task UpdateProgressAsync(int levelId, PlayerLevelProgress progress)
    {
        await SaveProgressAsync(levelId, progress);
    }

    /// <summary>
    /// Deletes progress data for a specific level
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <returns>A task that represents the delete operation</returns>
    public async Task DeleteProgressAsync(int levelId)
    {
        EnsureInitialized();
        _progressCache.Remove(levelId);
        PlayerPrefs.DeleteKey(PROGRESS_KEY_PREFIX + levelId);
        PlayerPrefs.Save();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Gets all completed levels
    /// </summary>
    /// <returns>A collection of level IDs that have been completed</returns>
    public IEnumerable<int> GetCompletedLevels()
    {
        EnsureInitialized();
        return _progressCache.Where(p => p.Value.isCompleted).Select(p => p.Key);
    }

    /// <summary>
    /// Gets all unlocked levels
    /// </summary>
    /// <returns>A collection of level IDs that are unlocked</returns>
    public IEnumerable<int> GetUnlockedLevels()
    {
        EnsureInitialized();
        return _progressCache.Where(p => p.Value.isUnlocked).Select(p => p.Key);
    }

    /// <summary>
    /// Gets the total stars earned across all levels
    /// </summary>
    /// <returns>The total number of stars earned</returns>
    public int GetTotalStarsEarned()
    {
        EnsureInitialized();
        return _progressCache.Values.Sum(p => p.starsEarned);
    }

    /// <summary>
    /// Checks if progress data exists for a level
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <returns>True if progress data exists, false otherwise</returns>
    public bool ProgressExists(int levelId)
    {
        EnsureInitialized();
        return _progressCache.ContainsKey(levelId);
    }

    /// <summary>
    /// Ensures the repository is initialized by loading progress data
    /// </summary>
    private void EnsureInitialized()
    {
        if (_isInitialized)
            return;

        LoadAllProgressFromStorage();
        _isInitialized = true;
    }

    /// <summary>
    /// Loads all progress data from storage
    /// </summary>
    private void LoadAllProgressFromStorage()
    {
        // This is a simplified implementation
        // In a real implementation, you might scan for all keys or use a different storage mechanism
        _progressCache.Clear();

        // For demonstration, we'll assume levels 1-10 exist
        for (int i = 1; i <= 10; i++)
        {
            var progress = LoadProgressFromStorage(i);
            if (progress != null)
            {
                _progressCache[i] = progress;
            }
        }
    }

    /// <summary>
    /// Loads progress data for a specific level from storage
    /// </summary>
    /// <param name="levelId">The level ID</param>
    /// <returns>The progress data if found, null otherwise</returns>
    private PlayerLevelProgress LoadProgressFromStorage(int levelId)
    {
        var json = PlayerPrefs.GetString(PROGRESS_KEY_PREFIX + levelId, "");
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<PlayerLevelProgress>(json);
    }

    /// <summary>
    /// Saves progress data for a specific level to storage
    /// </summary>
    /// <param name="levelId">The level ID</param>
    /// <param name="progress">The progress data</param>
    /// <returns>A task that represents the save operation</returns>
    private async Task SaveProgressToStorageAsync(int levelId, PlayerLevelProgress progress)
    {
        var json = JsonUtility.ToJson(progress);
        PlayerPrefs.SetString(PROGRESS_KEY_PREFIX + levelId, json);
        PlayerPrefs.Save();
        await Task.CompletedTask;
    }
}
 
