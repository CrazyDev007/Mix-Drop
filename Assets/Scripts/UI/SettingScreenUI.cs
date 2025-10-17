using ScreenFlow;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class SettingScreenUI : ScreenUI
    {
        [SerializeField] private GameObject settingScreen;

        [Header("UI Elements")]
        [SerializeField] private string backButtonName = "back-button";
        [SerializeField] private string musicSliderName = "music-slider";
        [SerializeField] private string sfxSliderName = "sfx-slider";

        private Button backButton;
        private Slider musicSlider;
        private Slider sfxSlider;

        private const string MusicVolumeKey = "MusicVolume";
        private const string SfxVolumeKey = "SfxVolume";

        private void InitializeUIElements(VisualElement root)
        {
            backButton = root.Q<Button>(backButtonName);
            musicSlider = root.Q<Slider>(musicSliderName);
            sfxSlider = root.Q<Slider>(sfxSliderName);

            if (backButton == null) Debug.LogWarning($"Back button '{backButtonName}' not found");
            if (musicSlider == null) Debug.LogWarning($"Music slider '{musicSliderName}' not found");
            if (sfxSlider == null) Debug.LogWarning($"SFX slider '{sfxSliderName}' not found");
        }

        private void SetupEventHandlers()
        {
            if (backButton != null)
            {
                backButton.clicked += OnBackButtonClicked;
            }
            if (musicSlider != null)
            {
                musicSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
            }
            if (sfxSlider != null)
            {
                sfxSlider.RegisterValueChangedCallback(OnSfxVolumeChanged);
            }
        }

        private void LoadSettings()
        {
            if (musicSlider != null)
            {
                musicSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
            }
            if (sfxSlider != null)
            {
                sfxSlider.value = PlayerPrefs.GetFloat(SfxVolumeKey, 0.5f);
            }
        }

        private void SaveSettings()
        {
            if (musicSlider != null)
            {
                PlayerPrefs.SetFloat(MusicVolumeKey, musicSlider.value);
            }
            if (sfxSlider != null)
            {
                PlayerPrefs.SetFloat(SfxVolumeKey, sfxSlider.value);
            }
            PlayerPrefs.Save();
        }

        private void OnMusicVolumeChanged(ChangeEvent<float> evt)
        {
            // Optionally update audio mixer here
            Debug.Log($"Music volume changed to: {evt.newValue}");
        }

        private void OnSfxVolumeChanged(ChangeEvent<float> evt)
        {
            // Optionally update audio mixer here
            Debug.Log($"SFX volume changed to: {evt.newValue}");
        }

        private void OnBackButtonClicked()
        {
            SaveSettings();
            ScreenManager.Instance.ShowScreen("MainScreen");
        }

        public void OnClickBtnBack()
        {
            OnBackButtonClicked();
        }

        protected override void SetupScreen(VisualElement screen)
        {
            InitializeUIElements(screen);
            SetupEventHandlers();
            LoadSettings();
        }
    }
}