using System.Collections.Generic;
using MixDrop.Data;

/// <summary>
/// Interface for managing player progress across levels
/// </summary>
public interface IProgressManager
{
    /// <summary>
    /// Initializes the progress manager and loads saved data
    /// </summary>
    void Initialize();
    
    /// <summary>
    /// Gets progress data for a specific level
    /// </summary>
    /// <param name="levelId">ID of the level</param>
    /// <returns>Progress data for the level</returns>
    PlayerLevelProgress GetLevelProgress(int levelId);
    
    /// <summary>
    /// Updates progress data for a specific level
    /// </summary>
    /// <param name="levelId">ID of the level</param>
    /// <param name="progress">Updated progress data</param>
    void UpdateLevelProgress(int levelId, PlayerLevelProgress progress);
    
    /// <summary>
    /// Checks if a level is unlocked
    /// </summary>
    /// <param name="levelId">ID of the level to check</param>
    /// <returns>True if the level is unlocked</returns>
    bool IsLevelUnlocked(int levelId);
    
    /// <summary>
    /// Gets all available level data
    /// </summary>
    /// <returns>List of all level data</returns>
    List<LevelData> GetAllLevels();
    
    /// <summary>
    /// Gets progress data for all levels
    /// </summary>
    /// <returns>Dictionary mapping level IDs to progress data</returns>
    Dictionary<int, PlayerLevelProgress> GetAllProgress();
}