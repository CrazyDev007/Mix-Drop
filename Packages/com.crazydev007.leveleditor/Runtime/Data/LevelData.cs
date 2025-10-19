using System.Collections.Generic;
using UnityEngine;

namespace CrazyDev007.LevelEditor
{
    [CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Editor/Level Data")]
    public class LevelData : ScriptableObject
    {
        public string levelName;
        public Difficulty difficulty;
        public List<PlaceableObject> placedObjects = new List<PlaceableObject>();
        public LevelConfiguration configuration = new LevelConfiguration();

        public void Initialize(string name)
        {
            levelName = name;
            difficulty = Difficulty.Easy;
            placedObjects = new List<PlaceableObject>();
            configuration = new LevelConfiguration();
        }

        public void AddObject(PlaceableObject obj)
        {
            if (placedObjects == null)
                placedObjects = new List<PlaceableObject>();
            
            placedObjects.Add(obj);
        }
    }
}