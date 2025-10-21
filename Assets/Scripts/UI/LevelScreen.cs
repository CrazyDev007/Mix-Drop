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
        private VisualTreeAsset levelRowTemplate;

        protected override void SetupScreen(VisualElement screen)
        {
            levelList = screen.Q<ScrollView>("level-list");
            backButton = screen.Q<Button>("back-button");

            if (backButton != null)
            {
                backButton.clicked += OnClickBackButton;
            }

            levelRowTemplate = Resources.Load<VisualTreeAsset>("LevelRow");
            SetupLevelList();
        }

        private void SetupLevelList()
        {
            levelList.Clear();
            int completedLevels = PlayerPrefs.GetInt("CompletedLevels", 0);

            for (int row = 0; row < 44; row++) // 200 rows for 1000 levels with 5 per row
            {
                var rowElement = levelRowTemplate.CloneTree();
                for (int col = 0; col < 5; col++)
                {
                    var button = rowElement.Q<Button>($"level-{col + 1}");
                    int levelNumber = row * 5 + col + 1;
                    if (levelNumber > 1000)
                    {
                        button.style.display = DisplayStyle.None;
                        continue;
                    }

                    bool isLocked = levelNumber > completedLevels + 1;
                    button.text = $"{levelNumber}";
                    button.SetEnabled(!isLocked);
                    button.clicked += () => OnLevelSelected(levelNumber);
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