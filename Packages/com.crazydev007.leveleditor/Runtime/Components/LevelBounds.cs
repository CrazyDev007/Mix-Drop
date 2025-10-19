using UnityEngine;

namespace CrazyDev007.LevelEditor
{
    public class LevelBounds : MonoBehaviour
    {
        [SerializeField]
        private Vector3 minBounds;

        [SerializeField]
        private Vector3 maxBounds;

        public Vector3 MinBounds => minBounds;
        public Vector3 MaxBounds => maxBounds;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(minBounds, new Vector3(minBounds.x, minBounds.y, maxBounds.z));
            Gizmos.DrawLine(minBounds, new Vector3(minBounds.x, maxBounds.y, minBounds.z));
            Gizmos.DrawLine(minBounds, new Vector3(maxBounds.x, minBounds.y, minBounds.z));
            Gizmos.DrawLine(maxBounds, new Vector3(minBounds.x, maxBounds.y, maxBounds.z));
            Gizmos.DrawLine(maxBounds, new Vector3(maxBounds.x, minBounds.y, maxBounds.z));
            Gizmos.DrawLine(maxBounds, new Vector3(maxBounds.x, maxBounds.y, minBounds.z));
            Gizmos.DrawLine(new Vector3(minBounds.x, maxBounds.y, minBounds.z), new Vector3(maxBounds.x, maxBounds.y, minBounds.z));
            Gizmos.DrawLine(new Vector3(minBounds.x, minBounds.y, maxBounds.z), new Vector3(maxBounds.x, minBounds.y, maxBounds.z));
            Gizmos.DrawLine(new Vector3(minBounds.x, maxBounds.y, maxBounds.z), new Vector3(maxBounds.x, maxBounds.y, maxBounds.z));
            Gizmos.DrawLine(new Vector3(minBounds.x, minBounds.y, minBounds.z), new Vector3(minBounds.x, minBounds.y, maxBounds.z));
            Gizmos.DrawLine(new Vector3(maxBounds.x, minBounds.y, minBounds.z), new Vector3(maxBounds.x, minBounds.y, maxBounds.z));
            Gizmos.DrawLine(new Vector3(minBounds.x, maxBounds.y, minBounds.z), new Vector3(minBounds.x, maxBounds.y, maxBounds.z));
            Gizmos.DrawLine(new Vector3(maxBounds.x, maxBounds.y, minBounds.z), new Vector3(maxBounds.x, maxBounds.y, maxBounds.z));
        }
    }
}