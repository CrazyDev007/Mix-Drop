using System.IO;
using UnityEngine;

namespace CrazyDev007.LevelEditor
{
    public class LevelSerializer
    {
        public void SaveLevel(string filePath, LevelData levelData)
        {
            string json = JsonUtility.ToJson(levelData, true);
            File.WriteAllText(filePath, json);
        }

        public LevelData LoadLevel(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"Level file not found: {filePath}");
                return null;
            }

            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<LevelData>(json);
        }
    }
}