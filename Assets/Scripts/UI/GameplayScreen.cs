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

        public void OnClickBtnAddTube()
        {
            AudioManager.Instance?.PlayButtonTap();
            TubeManager.Instance.AddEmptyTube();
        }

        public void OnClickBtnPause()
        {
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
            AudioManager.Instance?.PlayButtonTap();
            ScreenManager.Instance.ShowScreen("hint-screen");
        }

        protected override void SetupScreen(VisualElement screen)
        {
            levelNumberLabel = screen.Q<Label>("level-number");
            timerLabel = screen.Q<Label>("timer-label");
            movesLabel = screen.Q<Label>("moves-label");

            screen.Q<Button>("add-tube-button").clicked += OnClickBtnAddTube;
            screen.Q<Button>("hint-button").clicked += OnClickBtnHint;
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
        public void UpdateTimer(string timeInSec)
        {
            _storedTimeInSec = timeInSec;
            if (timerLabel != null)
                timerLabel.text = timeInSec;
        }
        string _storedMoves = "";
        public void UpdateMoves(string moves)
        {
            _storedMoves = moves;
            if (movesLabel != null)
                movesLabel.text = moves;
        }
    }
}