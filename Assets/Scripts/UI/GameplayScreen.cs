using ScreenFlow;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class GameplayScreen : ScreenUI
    {
        [SerializeField] private GameObject gameplayScreen;

        private Label levelNumberLabel;
        private Label timerLabel;
        private Label movesLabel;
        private Button addTubeButton;
        private Button hintButton;
        private Button pauseButton;

        [SerializeField] private HintScreenUI hintScreenUI;

        public void OnClickBtnAddTube()
        {
            TubeManager.Instance.AddEmptyTube();
        }

        public void OnClickBtnPause()
        {
            //uiManager.ChangeScreen(ScreenType.Pause);
        }

        public void OnLevelFailed()
        {
            //uiManager.ChangeScreen(ScreenType.LevelFailed);
        }

        private void OnClickBtnHint()
        {
            // Show hint screen
            if (hintScreenUI != null && hintScreenUI.gameObject != null)
            {
                hintScreenUI.gameObject.SetActive(true);
                // Set hint text based on current level
                hintScreenUI.SetHintText("Try to sort the colors by moving liquids between tubes. You can only move to an empty tube or on top of the same color!");
            }
        }

        protected override void SetupScreen(VisualElement screen)
        {
            levelNumberLabel = screen.Q<Label>("level-number");
            timerLabel = screen.Q<Label>("timer-label");
            movesLabel = screen.Q<Label>("moves-label");
            addTubeButton = screen.Q<Button>("add-tube-button");
            hintButton = screen.Q<Button>("hint-button");
            pauseButton = screen.Q<Button>("pause-button");

            addTubeButton.clicked += OnClickBtnAddTube;
            hintButton.clicked += OnClickBtnHint;
            pauseButton.clicked += OnClickBtnPause;

            // Initialize values
            UpdateLevel(1);
            UpdateTimer("00:00");
            UpdateMoves(0);
        }

        public void UpdateLevel(int level)
        {
            if (levelNumberLabel != null)
                levelNumberLabel.text = level.ToString();
        }

        public void UpdateTimer(string time)
        {
            if (timerLabel != null)
                timerLabel.text = time;
        }

        public void UpdateMoves(int moves)
        {
            if (movesLabel != null)
                movesLabel.text = "Moves: " + moves.ToString();
        }
    }
}