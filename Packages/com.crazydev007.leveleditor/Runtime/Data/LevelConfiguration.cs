using System;

namespace CrazyDev007.LevelEditor
{
    [Serializable]
    public class LevelConfiguration
    {
        public string LevelName;
        public int Difficulty;
        public string[] Objectives;

        public LevelConfiguration()
        {
            LevelName = "";
            Difficulty = 0;
            Objectives = new string[0];
        }

        public LevelConfiguration(string levelName, int difficulty, string[] objectives)
        {
            LevelName = levelName;
            Difficulty = difficulty;
            Objectives = objectives;
        }
    }
}