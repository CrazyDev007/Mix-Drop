using ScreenFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace UI
{
    public class LevelScreen : ScreenUI, IOnItemClickListener
    {
        [SerializeField] private GameObject levelScreen;

        [FormerlySerializedAs("m_RecyclableScroller")] [SerializeField]
        private LevelRecyclableScroller recyclableScroller;

        private void Awake()
        {
            recyclableScroller.OnItemClickListener = this;
        }

        public void OnItemClick(LevelInfo info)
        {
            if (info.LevelStatus == LevelStatus.Locked)
            {
                Debug.Log("Level is locked");
            }
            else
            {
                PlayerPrefs.SetInt("Hardness", 0);
                var completedLevels = PlayerPrefs.GetInt("CompletedLevels", 0);
                if (completedLevels <= info.LevelNumber)
                {
                }

                PlayerPrefs.SetInt("ActiveLevel", info.LevelNumber);
                SceneManager.LoadSceneAsync("GamePlay", LoadSceneMode.Single);
            }
        }

        public void OnClickBackButton()
        {
            //uiManager.ChangeScreen(ScreenType.Main);
        }

        protected override void SetupScreen(VisualElement screen)
        {
            throw new System.NotImplementedException();
        }
    }
}