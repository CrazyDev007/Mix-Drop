using System.Collections.Generic;
using System.Threading.Tasks;
using MixDrop.Data;

/// <summary>
/// Interface for level data repository operations
/// </summary>
public interface ILevelRepository
{
    /// <summary>
    /// Gets a level by its unique identifier
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <returns>The level data if found, null otherwise</returns>
    LevelData GetLevelById(int levelId);

    /// <summary>
    /// Gets all available levels
    /// </summary>
    /// <returns>A collection of all level data</returns>
    IEnumerable<LevelData> GetAllLevels();

    /// <summary>
    /// Gets levels by difficulty rating
    /// </summary>
    /// <param name="difficulty">The difficulty rating (1-5)</param>
    /// <returns>A collection of levels with the specified difficulty</returns>
    IEnumerable<LevelData> GetLevelsByDifficulty(int difficulty);

    /// <summary>
    /// Gets levels that are unlocked by default
    /// </summary>
    /// <returns>A collection of levels that are unlocked by default</returns>
    IEnumerable<LevelData> GetDefaultUnlockedLevels();

    /// <summary>
    /// Saves level data
    /// </summary>
    /// <param name="level">The level data to save</param>
    /// <returns>A task that represents the save operation</returns>
    Task SaveLevelAsync(LevelData level);

    /// <summary>
    /// Checks if a level exists
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <returns>True if the level exists, false otherwise</returns>
    bool LevelExists(int levelId);
}
 
