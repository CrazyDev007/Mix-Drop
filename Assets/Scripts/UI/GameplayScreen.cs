using ScreenFlow;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class GameplayScreen : ScreenUI
    {

        private Label levelNumberLabel;
        private Label timerLabel;
        private Label movesLabel;
        private Label levelTypeBadge;

        public void OnClickBtnAddTube()
        {
            AudioManager.Instance?.PlayButtonTap();
            TubeManager.Instance.AddEmptyTube();
        }

        public void OnClickBtnPause()
        {
            GameManager.Instance.IsUIOpen = true;
            AudioManager.Instance?.PlayButtonTap();
            ScreenManager.Instance.ShowScreen("pause-screen");
        }

        public void OnLevelFailed()
        {
            AudioManager.Instance?.PlayLevelFail();
            ScreenManager.Instance.ShowScreen("level-failed-screen");
        }

        private void OnClickBtnHint()
        {
            GameManager.Instance.IsUIOpen = true;
            AudioManager.Instance?.PlayButtonTap();
            ScreenManager.Instance.ShowScreen("hint-screen");
        }

        protected override void SetupScreen(VisualElement screen)
        {
            levelNumberLabel = screen.Q<Label>("level-number");
            timerLabel = screen.Q<Label>("timer-label");
            movesLabel = screen.Q<Label>("moves-label");
            levelTypeBadge = screen.Q<Label>("level-type-badge");

            screen.Q<Button>("add-tube-button").clicked += OnClickBtnAddTube;
            //screen.Q<Button>("hint-button").clicked += OnClickBtnHint;
            screen.Q<Button>("pause-button").clicked += OnClickBtnPause;

            // Initialize values
            UpdateLevel(_storedLevel);
            UpdateTimer(_storedTimeInSec);
            UpdateMoves(_storedMoves);
        }
        int _storedLevel = 0;
        public void UpdateLevel(int level)
        {
            _storedLevel = level;
            if (levelNumberLabel != null)
                levelNumberLabel.text = level.ToString();
        }
        string _storedTimeInSec = "";
        public void UpdateTimer(string timeInSec, bool isWarning = false)
        {
            _storedTimeInSec = timeInSec;
            if (timerLabel != null)
            {
                timerLabel.text = timeInSec;
                
                // Handle warning state
                if (isWarning)
                    timerLabel.AddToClassList("warning");
                else
                    timerLabel.RemoveFromClassList("warning");
            }
        }
        string _storedMoves = "";
        public void UpdateMoves(string moves, bool isWarning = false)
        {
            _storedMoves = moves;
            if (movesLabel != null)
            {
                movesLabel.text = moves;
                
                // Handle warning state
                if (isWarning)
                    movesLabel.AddToClassList("warning");
                else
                    movesLabel.RemoveFromClassList("warning");
            }
        }
        
        /// <summary>
        /// Updates the level type badge display
        /// </summary>
        /// <param name="levelType">The level type to display</param>
        public void UpdateLevelType(LevelType levelType)
        {
            if (levelTypeBadge != null)
            {
                levelTypeBadge.text = LevelTypeDetector.GetLevelTypeDisplayText(levelType);
                levelTypeBadge.style.display = DisplayStyle.Flex;
                
                // Remove existing type classes
                levelTypeBadge.RemoveFromClassList("normal");
                levelTypeBadge.RemoveFromClassList("timer");
                levelTypeBadge.RemoveFromClassList("moves");
                
                // Add appropriate type class
                switch (levelType)
                {
                    case LevelType.Normal:
                        levelTypeBadge.AddToClassList("normal");
                        break;
                    case LevelType.Timer:
                        levelTypeBadge.AddToClassList("timer");
                        break;
                    case LevelType.Moves:
                        levelTypeBadge.AddToClassList("moves");
                        break;
                }
            }
        }
    }
}