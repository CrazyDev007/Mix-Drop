using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UI
{
    public class LevelScreenUI : ScreenUI, IOnItemClickListener
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
            uiManager.ChangeScreen(ScreenType.Main);
        }

        public override void SetActive(bool active)
        {
            levelScreen.SetActive(active);
        }
    }
}