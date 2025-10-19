using System;
using System.IO;
using UnityEngine;

namespace CrazyDev007.LevelEditor
{
    public class LevelDeserializer
    {
        public LevelData Deserialize(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Level file not found.", filePath);
            }

            string json = File.ReadAllText(filePath);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);
            return levelData;
        }
    }
}