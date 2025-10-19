using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CrazyDev007.LevelEditor
{
    public class TubeColorLevelSerializer
    {
        public void SaveLevels(string filePath, List<TubeColorLevel> levels)
        {
            var wrapper = new TubeColorLevelsWrapper { levels = levels };
            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"Levels saved to {filePath}");
        }

        public List<TubeColorLevel> LoadLevels(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Level file not found: {filePath}");
                return new List<TubeColorLevel>();
            }

            string json = File.ReadAllText(filePath);
            TubeColorLevelsWrapper wrapper = JsonUtility.FromJson<TubeColorLevelsWrapper>(json);
            return wrapper.levels ?? new List<TubeColorLevel>();
        }

        public TubeColorLevel LoadSingleLevel(string filePath, int levelNumber)
        {
            List<TubeColorLevel> levels = LoadLevels(filePath);
            return levels.Find(l => l.level == levelNumber);
        }
    }
}
