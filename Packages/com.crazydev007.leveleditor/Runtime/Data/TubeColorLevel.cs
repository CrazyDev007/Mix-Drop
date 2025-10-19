using System.Collections.Generic;
using UnityEngine;

namespace CrazyDev007.LevelEditor
{
    [System.Serializable]
    public class TubeColorLevel
    {
        public int level;
        public List<TubeData> tubes = new List<TubeData>();
        public int emptyTubes;
        public int maxMoves;
        public int timeLimit;
        public List<int> lockedTubes = new List<int>();
        public int availableSwaps;
        public List<object> twists = new List<object>();

        public TubeColorLevel()
        {
            tubes = new List<TubeData>();
            lockedTubes = new List<int>();
            twists = new List<object>();
        }

        public TubeColorLevel(int levelNumber)
        {
            level = levelNumber;
            tubes = new List<TubeData>();
            emptyTubes = 1;
            maxMoves = 0;
            timeLimit = 0;
            lockedTubes = new List<int>();
            availableSwaps = 0;
            twists = new List<object>();
        }

        public void AddTube(TubeData tube)
        {
            tubes.Add(tube);
        }

        public int GetTotalTubes()
        {
            return tubes.Count;
        }

        public bool IsLocked(int tubeIndex)
        {
            return lockedTubes.Contains(tubeIndex);
        }

        public void LockTube(int tubeIndex)
        {
            if (!lockedTubes.Contains(tubeIndex))
            {
                lockedTubes.Add(tubeIndex);
            }
        }

        public void UnlockTube(int tubeIndex)
        {
            lockedTubes.Remove(tubeIndex);
        }
    }

    [System.Serializable]
    public class TubeColorLevelsWrapper
    {
        public List<TubeColorLevel> levels = new List<TubeColorLevel>();
    }
}
