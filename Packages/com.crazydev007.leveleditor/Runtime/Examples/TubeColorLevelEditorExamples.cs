using System.Collections.Generic;
using UnityEngine;
using CrazyDev007.LevelEditor;

namespace CrazyDev007.LevelEditor.Examples
{
    /// <summary>
    /// Example usage of the Tube Color Level Editor classes
    /// </summary>
    public class TubeColorLevelEditorExamples : MonoBehaviour
    {
        private TubeColorLevelSerializer serializer = new TubeColorLevelSerializer();

        // Example 1: Create a level programmatically
        public void CreateLevelProgrammatically()
        {
            TubeColorLevel level = new TubeColorLevel(1);
            
            // Create tubes
            TubeData tube1 = new TubeData(new List<int> { 0, 0, 1, 1 });
            TubeData tube2 = new TubeData(new List<int> { 1, 0, 2, 2 });
            TubeData tube3 = new TubeData(); // Empty tube

            // Add tubes to level
            level.AddTube(tube1);
            level.AddTube(tube2);
            level.AddTube(tube3);

            // Set level properties
            level.emptyTubes = 1;
            level.maxMoves = 5;
            level.timeLimit = 60;

            Debug.Log($"Created level with {level.GetTotalTubes()} tubes");
        }

        // Example 2: Load and display level info
        public void LoadAndDisplayLevel()
        {
            string filePath = "Packages/com.crazydev007.leveleditor/Editor/Resources/color_sort_1000_levels.json";
            List<TubeColorLevel> levels = serializer.LoadLevels(filePath);

            if (levels.Count > 0)
            {
                TubeColorLevel firstLevel = levels[0];
                Debug.Log($"Level {firstLevel.level}:");
                Debug.Log($"  Tubes: {firstLevel.GetTotalTubes()}");
                Debug.Log($"  Empty: {firstLevel.emptyTubes}");
                Debug.Log($"  Max Moves: {firstLevel.maxMoves}");

                // Display each tube
                for (int i = 0; i < firstLevel.tubes.Count; i++)
                {
                    TubeData tube = firstLevel.tubes[i];
                    string colors = tube.IsEmpty() ? "(empty)" : string.Join(",", tube.values);
                    Debug.Log($"  Tube {i + 1}: {colors}");
                }
            }
        }

        // Example 3: Validate a level
        public void ValidateLevel()
        {
            TubeColorLevel level = new TubeColorLevel(1);
            level.AddTube(new TubeData(new List<int> { 0, 0, 1, 1 }));
            level.AddTube(new TubeData());
            level.emptyTubes = 1;

            TubeColorLevelValidator validator = new TubeColorLevelValidator();
            List<string> errors = validator.ValidateLevel(level);

            if (errors.Count == 0)
            {
                Debug.Log("Level is valid!");
            }
            else
            {
                Debug.LogWarning("Level has errors:");
                foreach (string error in errors)
                {
                    Debug.LogWarning($"  - {error}");
                }
            }
        }

        // Example 4: Use helper utilities
        public void UseHelperUtilities()
        {
            TubeColorLevel level = new TubeColorLevel(1);
            level.AddTube(new TubeData(new List<int> { 0, 0, 1, 1 }));
            level.AddTube(new TubeData(new List<int> { 1, 2, 0, 2 }));
            level.AddTube(new TubeData(new List<int> { 2, 2, 0, 1 }));
            level.AddTube(new TubeData()); // Empty

            // Count unique colors
            int uniqueColors = TubeColorLevelHelper.CountColors(level);
            Debug.Log($"Unique colors: {uniqueColors}");

            // Get total color count
            int totalColors = TubeColorLevelHelper.GetTotalColorCount(level);
            Debug.Log($"Total colors: {totalColors}");

            // Check if solvable
            bool solvable = TubeColorLevelHelper.CanBeSolved(level);
            Debug.Log($"Solvable: {solvable}");

            // Fill a tube with color
            TubeColorLevelHelper.FillTubeWithColor(level.tubes[3], 3, 4);
            Debug.Log($"Filled tube 4 with color 3: {string.Join(",", level.tubes[3].values)}");
        }

        // Example 5: Manage tubes
        public void ManageTubes()
        {
            TubeData tube = new TubeData();

            // Add colors
            tube.AddColor(0);
            tube.AddColor(0);
            tube.AddColor(1);
            tube.AddColor(1);

            Debug.Log($"Tube is full: {tube.IsFull()}");
            Debug.Log($"Tube is empty: {tube.IsEmpty()}");
            Debug.Log($"Top color: {tube.GetTopColor()}");
            Debug.Log($"Tube contents: {string.Join(",", tube.values)}");

            // Remove color
            int removed = tube.RemoveTopColor();
            Debug.Log($"Removed color: {removed}");
            Debug.Log($"Tube contents: {string.Join(",", tube.values)}");

            // Clear tube
            tube.ClearTube();
            Debug.Log($"After clear, empty: {tube.IsEmpty()}");
        }

        // Example 6: Modify level properties
        public void ModifyLevelProperties()
        {
            TubeColorLevel level = new TubeColorLevel(5);
            
            // Add tubes
            for (int i = 0; i < 4; i++)
            {
                level.AddTube(new TubeData());
            }

            // Lock specific tubes
            level.LockTube(0);
            level.LockTube(1);

            Debug.Log($"Tube 0 locked: {level.IsLocked(0)}");
            Debug.Log($"Tube 2 locked: {level.IsLocked(2)}");
            Debug.Log($"Locked tubes: {string.Join(",", level.lockedTubes)}");

            // Unlock
            level.UnlockTube(0);
            Debug.Log($"After unlock, locked tubes: {string.Join(",", level.lockedTubes)}");
        }

        // Example 7: Save and load custom level
        public void SaveAndLoadCustomLevel()
        {
            // Create level
            TubeColorLevel level = new TubeColorLevel(999);
            level.AddTube(new TubeData(new List<int> { 0, 0, 1, 1 }));
            level.AddTube(new TubeData(new List<int> { 2, 3, 0, 2 }));
            level.AddTube(new TubeData());
            level.emptyTubes = 1;
            level.maxMoves = 10;

            // Save
            List<TubeColorLevel> levels = new List<TubeColorLevel> { level };
            string filePath = "Assets/custom_level.json";
            serializer.SaveLevels(filePath, levels);
            Debug.Log($"Level saved to {filePath}");

            // Load
            List<TubeColorLevel> loadedLevels = serializer.LoadLevels(filePath);
            if (loadedLevels.Count > 0)
            {
                TubeColorLevel loaded = loadedLevels[0];
                Debug.Log($"Loaded level {loaded.level} with {loaded.GetTotalTubes()} tubes");
            }
        }

        // Example 8: Batch validation
        public void BatchValidation()
        {
            string filePath = "Packages/com.crazydev007.leveleditor/Editor/Resources/color_sort_1000_levels.json";
            List<TubeColorLevel> levels = serializer.LoadLevels(filePath);

            TubeColorLevelValidator validator = new TubeColorLevelValidator();
            int validCount = 0;
            int problemCount = 0;
            List<int> problemLevelNumbers = new List<int>();

            foreach (TubeColorLevel level in levels)
            {
                if (validator.IsLevelValid(level))
                {
                    validCount++;
                }
                else
                {
                    problemCount++;
                    problemLevelNumbers.Add(level.level);
                }
            }

            Debug.Log($"Validation Results: {validCount} valid, {problemCount} problems");
            if (problemCount > 0 && problemCount <= 10)
            {
                Debug.Log($"Problem levels: {string.Join(",", problemLevelNumbers)}");
            }
        }

        // Example 9: Find solvable levels
        public void FindSolvableLevels()
        {
            string filePath = "Packages/com.crazydev007.leveleditor/Editor/Resources/color_sort_1000_levels.json";
            List<TubeColorLevel> levels = serializer.LoadLevels(filePath);

            int solvableCount = 0;
            int unsolvableCount = 0;

            foreach (TubeColorLevel level in levels)
            {
                if (TubeColorLevelHelper.CanBeSolved(level))
                {
                    solvableCount++;
                }
                else
                {
                    unsolvableCount++;
                    Debug.LogWarning($"Level {level.level} is not solvable");
                }
            }

            Debug.Log($"Solvable: {solvableCount}, Unsolvable: {unsolvableCount}");
        }

        // Example 10: Analyze difficulty
        public void AnalyzeDifficulty()
        {
            string filePath = "Packages/com.crazydev007.leveleditor/Editor/Resources/color_sort_1000_levels.json";
            List<TubeColorLevel> levels = serializer.LoadLevels(filePath);

            int easy = 0, medium = 0, hard = 0;

            foreach (TubeColorLevel level in levels)
            {
                int colors = TubeColorLevelHelper.CountColors(level);
                
                if (colors <= 2)
                    easy++;
                else if (colors <= 3)
                    medium++;
                else
                    hard++;
            }

            Debug.Log($"Difficulty Distribution:");
            Debug.Log($"  Easy (≤2 colors): {easy}");
            Debug.Log($"  Medium (3 colors): {medium}");
            Debug.Log($"  Hard (4+ colors): {hard}");
        }
    }
}
