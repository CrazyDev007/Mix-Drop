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

        public void OnItemClick(LevelCell cell)
        {
            PlayerPrefs.SetInt("Hardness", 0);
            PlayerPrefs.SetInt("Level", cell.CellIndex + 1);
            SceneManager.LoadSceneAsync("GamePlay", LoadSceneMode.Single);
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