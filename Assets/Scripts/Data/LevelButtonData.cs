using UnityEngine;
using MixDrop.Data;

/// <summary>
/// Data container for a level button in the UI
/// </summary>
public class LevelButtonData
{
    /// <summary>
    /// Level data associated with this button
    /// </summary>
    public LevelData LevelData { get; private set; }
    
    /// <summary>
    /// Player progress for this level
    /// </summary>
    public PlayerLevelProgress Progress { get; private set; }
    
    /// <summary>
    /// Whether the level is currently unlocked
    /// </summary>
    public bool IsUnlocked { get; private set; }
    
    /// <summary>
    /// Whether the level has been completed
    /// </summary>
    public bool IsCompleted { get; private set; }
    
    /// <summary>
    /// Number of stars earned (0-3)
    /// </summary>
    public int StarsEarned { get; private set; }
    
    /// <summary>
    /// Initializes a new instance of LevelButtonData
    /// </summary>
    /// <param name="levelData">Level data</param>
    /// <param name="progress">Player progress</param>
    /// <param name="isUnlocked">Whether the level is unlocked</param>
    public LevelButtonData(LevelData levelData, PlayerLevelProgress progress, bool isUnlocked)
    {
        LevelData = levelData;
        Progress = progress;
        IsUnlocked = isUnlocked;
        IsCompleted = progress.isCompleted;
        StarsEarned = progress.starsEarned;
    }
}