using System;

/// <summary>
/// Represents player's progress data for a specific level
/// </summary>
[System.Serializable]
public class PlayerLevelProgress
{
    
    public int levelId;
    
    
    public bool isUnlocked;
    
    
    public bool isCompleted;
    
    
    public int starsEarned;
    
    
    public float bestCompletionTime;
    
    public int attemptCount;
    
    public DateTime completionDate;
}