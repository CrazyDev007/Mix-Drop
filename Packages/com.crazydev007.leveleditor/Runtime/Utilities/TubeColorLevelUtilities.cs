using System.Collections.Generic;
using UnityEngine;

namespace CrazyDev007.LevelEditor
{
    public class TubeColorLevelValidator
    {
        public List<string> ValidateLevel(TubeColorLevel level)
        {
            List<string> errors = new List<string>();

            // Check if level has at least one tube
            if (level.tubes.Count == 0)
            {
                errors.Add("Level must have at least one tube.");
            }

            // Check empty tubes count
            int actualEmptyTubes = 0;
            foreach (TubeData tube in level.tubes)
            {
                if (tube.IsEmpty())
                {
                    actualEmptyTubes++;
                }
            }

            if (actualEmptyTubes != level.emptyTubes)
            {
                errors.Add($"Empty tubes mismatch: Expected {level.emptyTubes}, but found {actualEmptyTubes}");
            }

            // Check if any locked tube index is out of range
            foreach (int lockedIndex in level.lockedTubes)
            {
                if (lockedIndex < 0 || lockedIndex >= level.tubes.Count)
                {
                    errors.Add($"Locked tube index {lockedIndex} is out of range.");
                }
            }

            // Check if each tube has valid color values
            for (int i = 0; i < level.tubes.Count; i++)
            {
                TubeData tube = level.tubes[i];
                foreach (int colorValue in tube.values)
                {
                    if (colorValue < 0)
                    {
                        errors.Add($"Tube {i} has invalid color value: {colorValue}");
                    }
                }
            }

            return errors;
        }

        public bool IsLevelValid(TubeColorLevel level)
        {
            return ValidateLevel(level).Count == 0;
        }
    }

    public class TubeColorLevelHelper
    {
        public static void ShuffleTubeColors(TubeData tube)
        {
            for (int i = tube.values.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                int temp = tube.values[i];
                tube.values[i] = tube.values[randomIndex];
                tube.values[randomIndex] = temp;
            }
        }

        public static void FillTubeWithColor(TubeData tube, int colorValue, int count = 4)
        {
            tube.ClearTube();
            for (int i = 0; i < count && i < 4; i++)
            {
                tube.AddColor(colorValue);
            }
        }

        public static int CountColors(TubeColorLevel level)
        {
            HashSet<int> colors = new HashSet<int>();
            foreach (TubeData tube in level.tubes)
            {
                foreach (int color in tube.values)
                {
                    if (color >= 0)
                    {
                        colors.Add(color);
                    }
                }
            }
            return colors.Count;
        }

        public static int GetTotalColorCount(TubeColorLevel level)
        {
            int count = 0;
            foreach (TubeData tube in level.tubes)
            {
                count += tube.values.Count;
            }
            return count;
        }

        public static bool CanBeSolved(TubeColorLevel level)
        {
            int uniqueColors = CountColors(level);
            int maxTubesNeeded = uniqueColors;
            int availableTubes = level.emptyTubes + 1; // +1 for the working tube
            
            return availableTubes >= maxTubesNeeded;
        }
    }
}
