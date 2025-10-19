using System.Collections.Generic;
using UnityEngine;

namespace CrazyDev007.LevelEditor
{
    [System.Serializable]
    public class TubeData
    {
        public List<int> values = new List<int>();

        public TubeData()
        {
            values = new List<int>();
        }

        public TubeData(List<int> tubeValues)
        {
            values = new List<int>(tubeValues);
        }

        public bool IsFull()
        {
            return values.Count == 4;
        }

        public bool IsEmpty()
        {
            return values.Count == 0;
        }

        public void AddColor(int colorValue)
        {
            if (values.Count < 4)
            {
                values.Add(colorValue);
            }
        }

        public int RemoveTopColor()
        {
            if (values.Count > 0)
            {
                int topColor = values[values.Count - 1];
                values.RemoveAt(values.Count - 1);
                return topColor;
            }
            return -1;
        }

        public int GetTopColor()
        {
            if (values.Count > 0)
            {
                return values[values.Count - 1];
            }
            return -1;
        }

        public void ClearTube()
        {
            values.Clear();
        }
    }
}
