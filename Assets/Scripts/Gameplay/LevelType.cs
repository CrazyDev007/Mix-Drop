using System;
using UnityEngine;

/// <summary>
/// Defines different types of level gameplay modes
/// </summary>
public enum LevelType
{
    Normal,  // Unlimited time and moves
    Timer,   // Limited time, unlimited moves
    Moves,    // Limited moves, unlimited time
    TimerAndMoves // Both limited
}

/// <summary>
/// Utility class for level type detection and validation
/// </summary>
public static class LevelTypeDetector
{
    /// <summary>
    /// Determines level type based on maxMoves and timeLimit values
    /// </summary>
    /// <param name="maxMoves">Maximum moves allowed (0 for unlimited)</param>
    /// <param name="timeLimit">Time limit in seconds (0 for unlimited)</param>
    /// <returns>The detected LevelType</returns>
    public static LevelType DetectLevelType(int maxMoves, float timeLimit)
    {
        // Normal mode: unlimited time and moves
        if (maxMoves == 0 && timeLimit == 0)
            return LevelType.Normal;
        
        // Timer mode: limited time, unlimited moves
        if (maxMoves == 0 && timeLimit > 0)
            return LevelType.Timer;
        
        // Moves mode: limited moves, unlimited time
        if (maxMoves > 0 && timeLimit == 0)
            return LevelType.Moves;
        
        // Hybrid mode: both limited (should be rare, treat as Timer for now)
        if (maxMoves > 0 && timeLimit > 0)
        {
            Debug.LogWarning($"Hybrid level detected (maxMoves: {maxMoves}, timeLimit: {timeLimit}). Treating as Timer mode.");
            return LevelType.TimerAndMoves;
        }
        
        // Fallback to Normal for invalid combinations
        Debug.LogError($"Invalid level configuration (maxMoves: {maxMoves}, timeLimit: {timeLimit}). Defaulting to Normal.");
        return LevelType.Normal;
    }
    
    /// <summary>
    /// Gets the display text for level type badge
    /// </summary>
    /// <param name="levelType">The level type</param>
    /// <returns>Display text for badge</returns>
    public static string GetLevelTypeDisplayText(LevelType levelType)
    {
        return levelType switch
        {
            LevelType.Normal => "NORMAL",
            LevelType.Timer => "TIMER",
            LevelType.Moves => "MOVES",
            _ => "UNKNOWN"
        };
    }
    
    /// <summary>
    /// Validates level configuration for consistency
    /// </summary>
    /// <param name="maxMoves">Maximum moves allowed</param>
    /// <param name="timeLimit">Time limit in seconds</param>
    /// <returns>True if configuration is valid</returns>
    public static bool ValidateLevelConfiguration(int maxMoves, float timeLimit)
    {
        // Check for negative values
        if (maxMoves < 0 || timeLimit < 0)
            return false;
        
        // Check for hybrid levels (both limited)
        if (maxMoves > 0 && timeLimit > 0)
        {
            Debug.LogWarning("Hybrid level detected (both time and moves limited). Consider using explicit level type.");
            return true; // Allow but warn
        }
        
        return true;
    }
}