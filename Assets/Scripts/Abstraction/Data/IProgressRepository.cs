using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Interface for player progress data repository operations
/// </summary>
public interface IProgressRepository
{
    /// <summary>
    /// Gets progress data for a specific level
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <returns>The progress data if found, null otherwise</returns>
    PlayerLevelProgress GetProgress(int levelId);

    /// <summary>
    /// Gets progress data for all levels
    /// </summary>
    /// <returns>A dictionary mapping level IDs to progress data</returns>
    Dictionary<int, PlayerLevelProgress> GetAllProgress();

    /// <summary>
    /// Saves progress data for a specific level
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <param name="progress">The progress data to save</param>
    /// <returns>A task that represents the save operation</returns>
    Task SaveProgressAsync(int levelId, PlayerLevelProgress progress);

    /// <summary>
    /// Updates progress data for a specific level
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <param name="progress">The updated progress data</param>
    /// <returns>A task that represents the update operation</returns>
    Task UpdateProgressAsync(int levelId, PlayerLevelProgress progress);

    /// <summary>
    /// Deletes progress data for a specific level
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <returns>A task that represents the delete operation</returns>
    Task DeleteProgressAsync(int levelId);

    /// <summary>
    /// Gets all completed levels
    /// </summary>
    /// <returns>A collection of level IDs that have been completed</returns>
    IEnumerable<int> GetCompletedLevels();

    /// <summary>
    /// Gets all unlocked levels
    /// </summary>
    /// <returns>A collection of level IDs that are unlocked</returns>
    IEnumerable<int> GetUnlockedLevels();

    /// <summary>
    /// Gets the total stars earned across all levels
    /// </summary>
    /// <returns>The total number of stars earned</returns>
    int GetTotalStarsEarned();

    /// <summary>
    /// Checks if progress data exists for a level
    /// </summary>
    /// <param name="levelId">The unique identifier of the level</param>
    /// <returns>True if progress data exists, false otherwise</returns>
    bool ProgressExists(int levelId);
}
 
