using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CrazyDev007.LevelEditor.Editor
{
    public class TubeColorLevelMenu
    {
        private static string jsonFilePath = "Packages/com.crazydev007.leveleditor/Editor/Resources/color_sort_1000_levels.json";
        private static TubeColorLevelSerializer serializer = new TubeColorLevelSerializer();

        [MenuItem("Tools/Tube Color Level Editor/Validate All Levels")]
        public static void ValidateAllLevels()
        {
            string fullPath = Path.Combine(Application.dataPath, "..", jsonFilePath);
            List<TubeColorLevel> levels = serializer.LoadLevels(fullPath);
            TubeColorLevelValidator validator = new TubeColorLevelValidator();

            int validLevels = 0;
            int invalidLevels = 0;

            foreach (TubeColorLevel level in levels)
            {
                List<string> errors = validator.ValidateLevel(level);
                if (errors.Count == 0)
                {
                    validLevels++;
                }
                else
                {
                    invalidLevels++;
                    Debug.LogWarning($"Level {level.level} has errors:\n{string.Join("\n", errors)}");
                }
            }

            Debug.Log($"Validation Complete: {validLevels} valid levels, {invalidLevels} invalid levels");
        }

        [MenuItem("Tools/Tube Color Level Editor/Export Level Stats")]
        public static void ExportLevelStats()
        {
            string fullPath = Path.Combine(Application.dataPath, "..", jsonFilePath);
            List<TubeColorLevel> levels = serializer.LoadLevels(fullPath);

            string statsPath = Path.Combine(Application.dataPath, "level_stats.txt");
            using (StreamWriter writer = new StreamWriter(statsPath))
            {
                writer.WriteLine("Tube Color Level Statistics");
                writer.WriteLine("============================\n");
                writer.WriteLine($"Total Levels: {levels.Count}");

                foreach (TubeColorLevel level in levels)
                {
                    int colorCount = TubeColorLevelHelper.CountColors(level);
                    int totalColors = TubeColorLevelHelper.GetTotalColorCount(level);
                    bool solvable = TubeColorLevelHelper.CanBeSolved(level);

                    writer.WriteLine($"\nLevel {level.level}:");
                    writer.WriteLine($"  Tubes: {level.tubes.Count}");
                    writer.WriteLine($"  Unique Colors: {colorCount}");
                    writer.WriteLine($"  Total Colors: {totalColors}");
                    writer.WriteLine($"  Empty Tubes: {level.emptyTubes}");
                    writer.WriteLine($"  Max Moves: {level.maxMoves}");
                    writer.WriteLine($"  Locked Tubes: {level.lockedTubes.Count}");
                    writer.WriteLine($"  Solvable: {(solvable ? "Yes" : "No")}");
                }
            }

            Debug.Log($"Level stats exported to {statsPath}");
        }

        [MenuItem("Tools/Tube Color Level Editor/Find Problem Levels")]
        public static void FindProblemLevels()
        {
            string fullPath = Path.Combine(Application.dataPath, "..", jsonFilePath);
            List<TubeColorLevel> levels = serializer.LoadLevels(fullPath);
            TubeColorLevelValidator validator = new TubeColorLevelValidator();

            List<int> problemLevels = new List<int>();

            foreach (TubeColorLevel level in levels)
            {
                if (!validator.IsLevelValid(level))
                {
                    problemLevels.Add(level.level);
                }

                if (!TubeColorLevelHelper.CanBeSolved(level))
                {
                    if (!problemLevels.Contains(level.level))
                    {
                        problemLevels.Add(level.level);
                    }
                }
            }

            if (problemLevels.Count > 0)
            {
                Debug.LogWarning($"Found {problemLevels.Count} problem levels: {string.Join(", ", problemLevels)}");
            }
            else
            {
                Debug.Log("All levels are valid and solvable!");
            }
        }

        [MenuItem("Tools/Tube Color Level Editor/Quick Test")]
        public static void QuickTest()
        {
            string fullPath = Path.Combine(Application.dataPath, "..", jsonFilePath);
            List<TubeColorLevel> levels = serializer.LoadLevels(fullPath);

            if (levels.Count > 0)
            {
                TubeColorLevel testLevel = levels[0];
                Debug.Log($"First level has {testLevel.tubes.Count} tubes with {TubeColorLevelHelper.GetTotalColorCount(testLevel)} total colors");
                
                foreach (TubeData tube in testLevel.tubes)
                {
                    Debug.Log($"Tube: {string.Join(",", tube.values)}");
                }
            }
        }
    }
}
