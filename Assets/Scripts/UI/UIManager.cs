using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public enum ScreenType
    {
        Main,
        Setting,
        Level,
        Pause,
        Gameplay,
        Hint,
        LevelFailed,
        LevelCompleted
    }

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private ScreenType currentScreen = ScreenType.Main;
        [SerializeField] private List<ScreenUI> screens = new List<ScreenUI>();


        private void Awake()
        {
            foreach (var screen in screens)
            {
                screen.Setup(this);
            }

            ChangeScreen(currentScreen);
        }

        public void ChangeScreen(ScreenType nextScreen)
        {
            currentScreen = nextScreen;
            foreach (var screen in screens)
            {
                Debug.Log(">>>>> UIManager " + screen.screenType + " : " + (screen.screenType == currentScreen));
                screen.SetActive(screen.screenType == currentScreen);
            }
        }
    }
}