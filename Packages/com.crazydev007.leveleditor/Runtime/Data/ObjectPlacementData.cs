using System.Collections.Generic;
using UnityEngine;

namespace CrazyDev007.LevelEditor
{
    [System.Serializable]
    public class ObjectPlacementData
    {
        public List<PlacedObject> PlacedObjects;

        public ObjectPlacementData()
        {
            PlacedObjects = new List<PlacedObject>();
        }

        [System.Serializable]
        public class PlacedObject
        {
            public string ObjectID;
            public Vector3 Position;
            public Quaternion Rotation;

            public PlacedObject(string objectId, Vector3 position, Quaternion rotation)
            {
                ObjectID = objectId;
                Position = position;
                Rotation = rotation;
            }
        }
    }
}