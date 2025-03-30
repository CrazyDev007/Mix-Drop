using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public enum ScreenType
    {
        Main,
        Setting,
        Level
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

            ChangeScreen(ScreenType.Main);
        }

        public void ChangeScreen(ScreenType nextScreen)
        {
            currentScreen = nextScreen;
            foreach (var screen in screens)
            {
                screen.SetActive(screen.screenType == currentScreen);
            }
        }
    }
}