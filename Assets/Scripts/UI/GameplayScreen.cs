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
            TubeManager.Instance.AddEmptyTube();
        }

        public void OnClickBtnPause()
        {
            ScreenManager.Instance.ShowScreen("pause-screen");
        }

        public void OnLevelFailed()
        {
            ScreenManager.Instance.ShowScreen("level-failed-screen");
        }

        private void OnClickBtnHint()
        {
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
            UpdateLevel(0);
            UpdateTimer("Unlimited Time");
            UpdateMoves("Unlimited Moves");
        }

        public void UpdateLevel(int level)
        {
            if (levelNumberLabel != null)
                levelNumberLabel.text = level.ToString();
        }

        public void UpdateTimer(string timeInSec)
        {
            if (timerLabel != null)
                timerLabel.text = timeInSec;
        }

        public void UpdateMoves(string moves)
        {
            if (movesLabel != null)
                movesLabel.text = moves;
        }
    }
}