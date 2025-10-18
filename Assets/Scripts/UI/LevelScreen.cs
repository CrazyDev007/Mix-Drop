using ScreenFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI
{
    public class LevelScreen : ScreenUI
    {
        private ScrollView levelList;
        private Button backButton;

        protected override void SetupScreen(VisualElement screen)
        {
            levelList = screen.Q<ScrollView>("level-list");
            backButton = screen.Q<Button>("back-button");

            if (backButton != null)
            {
                backButton.clicked += OnClickBackButton;
            }

            SetupLevelList();
        }

        private void SetupLevelList()
        {
            levelList.Clear();
            int completedLevels = PlayerPrefs.GetInt("CompletedLevels", 0);
            int levelsPerRow = 10;

            for (int row = 0; row < 100; row++) // 100 rows for 1000 levels
            {
                var rowElement = new VisualElement { style = { flexDirection = FlexDirection.Row, justifyContent = Justify.SpaceAround } };
                for (int col = 0; col < levelsPerRow; col++)
                {
                    int levelNumber = row * levelsPerRow + col + 1;
                    if (levelNumber > 1000) break;

                    bool isLocked = levelNumber > completedLevels + 1;
                    var button = new Button { text = $"{levelNumber}" };
                    button.SetEnabled(!isLocked);
                    button.style.width = 80;
                    button.style.height = 80;
                    button.clicked += () => OnLevelSelected(levelNumber);
                    rowElement.Add(button);
                }
                levelList.Add(rowElement);
            }
        }

        private void OnLevelSelected(int levelNumber)
        {
            PlayerPrefs.SetInt("Hardness", 0);
            PlayerPrefs.SetInt("ActiveLevel", levelNumber);
            SceneManager.LoadSceneAsync("GamePlay", LoadSceneMode.Single);
        }

        public void OnClickBackButton()
        {
            //uiManager.ChangeScreen(ScreenType.Main);
            ScreenManager.Instance.ShowScreen("LobbyScreen");
        }
    }
}