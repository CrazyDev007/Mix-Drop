using UnityEngine;

namespace CrazyDev007.LevelEditor
{
    public class PlaceableObject : MonoBehaviour
    {
        public string objectName;
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;
        public bool snapToGrid = true;
        public float gridSize = 1.0f;

        public void Place(Vector3 newPosition, Quaternion newRotation)
        {
            position = newPosition;
            rotation = newRotation;
            Instantiate(prefab, position, rotation);
        }

        public void SetObjectName(string name)
        {
            objectName = name;
        }
    }
}