using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using MixDrop.Data;

/// <summary>
/// Implementation of the level repository for accessing level data
/// </summary>
public class LevelRepository : ILevelRepository
{
    private Dictionary<int, LevelData> _levelsCache;
    private bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the LevelRepository class
    /// </summary>
    public LevelRepository()
    {
        _levelsCache = new Dictionary<int, LevelData>();
        _isInitialized = false;
    }

    /// <summary>
    /// Gets a level by its unique identifier
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <returns>The level data if found, null otherwise</returns>
    public LevelData GetLevelById(int levelId)
    {
        EnsureInitialized();
        return _levelsCache.TryGetValue(levelId, out var level) ? level : null;
    }

    /// <summary>
    /// Gets all available levels
    /// </summary>
    /// <returns>A collection of all level data</returns>
    public IEnumerable<LevelData> GetAllLevels()
    {
        EnsureInitialized();
        return _levelsCache.Values;
    }

    /// <summary>
    /// Gets levels by difficulty rating
    /// </summary>
    /// <param name="difficulty">The difficulty rating (1-5)</param>
    /// <returns>A collection of levels with the specified difficulty</returns>
    public IEnumerable<LevelData> GetLevelsByDifficulty(int difficulty)
    {
        EnsureInitialized();
        return _levelsCache.Values.Where(level => level.Difficulty == difficulty);
    }

    /// <summary>
    /// Gets levels that are unlocked by default
    /// </summary>
    /// <returns>A collection of levels that are unlocked by default</returns>
    public IEnumerable<LevelData> GetDefaultUnlockedLevels()
    {
        EnsureInitialized();
        return _levelsCache.Values.Where(level => !level.IsLockedByDefault);
    }

    /// <summary>
    /// Saves level data
    /// </summary>
    /// <param name="level">The level data to save</param>
    /// <returns>A task that represents the save operation</returns>
    public async Task SaveLevelAsync(LevelData level)
    {
        if (level == null)
            throw new System.ArgumentNullException(nameof(level));

        EnsureInitialized();
        _levelsCache[int.Parse(level.LevelId)] = level;

        // TODO: Implement actual persistence logic
        await Task.CompletedTask;
    }

    /// <summary>
    /// Checks if a level exists
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <returns>True if the level exists, false otherwise</returns>
    public bool LevelExists(int levelId)
    {
        EnsureInitialized();
        return _levelsCache.ContainsKey(levelId);
    }

    /// <summary>
    /// Ensures the repository is initialized by loading level data
    /// </summary>
    private void EnsureInitialized()
    {
        if (_isInitialized)
            return;

        LoadLevelsFromResources();
        _isInitialized = true;
    }

    /// <summary>
    /// Loads level data from Resources folder
    /// </summary>
    private void LoadLevelsFromResources()
    {
        var levels = Resources.LoadAll<LevelData>("Levels");
        foreach (var level in levels)
        {
            int levelId = int.Parse(level.LevelId);
            if (!_levelsCache.ContainsKey(levelId))
            {
                _levelsCache[levelId] = level;
            }
        }
    }
}
 
