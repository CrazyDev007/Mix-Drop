using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Validates level configurations and provides diagnostic information
/// </summary>
public static class LevelTypeValidator
{
    /// <summary>
    /// Validates all levels in provided data
    /// </summary>
    /// <param name="levelData">List of level data to validate</param>
    /// <returns>Validation report with issues found</returns>
    public static ValidationReport ValidateLevels(List<TubeLiquidData> levelData)
    {
        var report = new ValidationReport();
        
        for (int i = 0; i < levelData.Count; i++)
        {
            var level = levelData[i];
            var issues = ValidateSingleLevel(level, i + 1);
            
            if (issues.Count > 0)
            {
                report.AddLevelIssues(i + 1, issues);
            }
        }
        
        return report;
    }
    
    /// <summary>
    /// Validates a single level configuration
    /// </summary>
    /// <param name="level">Level data to validate</param>
    /// <param name="levelNumber">Level number for reporting</param>
    /// <returns>List of validation issues</returns>
    public static List<string> ValidateSingleLevel(TubeLiquidData level, int levelNumber)
    {
        var issues = new List<string>();
        
        // Check for negative values
        if (level.maxMoves < 0)
            issues.Add($"Negative maxMoves: {level.maxMoves}");
        
        if (level.timeLimit < 0)
            issues.Add($"Negative timeLimit: {level.timeLimit}");
        
        // Check for hybrid levels
        if (level.maxMoves > 0 && level.timeLimit > 0)
            issues.Add($"Hybrid level (both time and moves limited)");
        
        // Check for reasonable limits
        if (level.maxMoves > 1000)
            issues.Add($"Very high maxMoves: {level.maxMoves} (possible error)");
        
        if (level.timeLimit > 3600) // More than 1 hour
            issues.Add($"Very high timeLimit: {level.timeLimit} seconds (possible error)");
        
        return issues;
    }
}

/// <summary>
/// Contains results of level validation
/// </summary>
public class ValidationReport
{
    public Dictionary<int, List<string>> LevelIssues { get; private set; }
    public int TotalIssues => LevelIssues.Values.Count;
    public bool IsValid => TotalIssues == 0;
    
    public ValidationReport()
    {
        LevelIssues = new Dictionary<int, List<string>>();
    }
    
    public void AddLevelIssues(int levelNumber, List<string> issues)
    {
        LevelIssues[levelNumber] = issues;
    }
    
    public void LogToConsole()
    {
        if (IsValid)
        {
            Debug.Log("Level validation passed: All levels are properly configured.");
            return;
        }
        
        Debug.LogWarning($"Level validation found {TotalIssues} issues:");
        
        foreach (var kvp in LevelIssues)
        {
            Debug.LogWarning($"Level {kvp.Key}:");
            foreach (var issue in kvp.Value)
            {
                Debug.LogWarning($"  - {issue}");
            }
        }
    }
}