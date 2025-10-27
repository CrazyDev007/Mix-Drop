using ScreenFlow;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class SettingScreenUI : ScreenUI
    {

        [Header("UI Elements")]
        private string backButtonName = "close-button";
        private string musicSliderName = "music-slider";
        private string sfxSliderName = "sfx-slider";
        private string musicMuteToggleName = "music-mute-toggle";
        private string confirmButtonName = "confirm-button";

        private Button backButton;
        private Button confirmButton;
        private Slider musicSlider;
        private Slider sfxSlider;
        private Toggle musicMuteToggle;

        private const string MusicVolumeKey = "MusicVolume";
        private const string SfxVolumeKey = "SfxVolume";

        private void InitializeUIElements(VisualElement root)
        {
            backButton = root.Q<Button>(backButtonName);
            confirmButton = root.Q<Button>(confirmButtonName);
            musicSlider = root.Q<Slider>(musicSliderName);
            sfxSlider = root.Q<Slider>(sfxSliderName);
            musicMuteToggle = root.Q<Toggle>(musicMuteToggleName);

            if (backButton == null) Debug.LogWarning($"Back button '{backButtonName}' not found");
            if (confirmButton == null) Debug.LogWarning($"Confirm button '{confirmButtonName}' not found");
            Debug.Log($"[DEBUG] backButtonName: '{backButtonName}', backButton is null: {backButton == null}");
            Debug.Log($"[DEBUG] Available buttons in root: {string.Join(", ", root.Query<Button>().ToList().Select(b => b.name))}");
            if (musicSlider == null) Debug.LogWarning($"Music slider '{musicSliderName}' not found");
            if (sfxSlider == null) Debug.LogWarning($"SFX slider '{sfxSliderName}' not found");
            if (musicMuteToggle == null) Debug.LogWarning($"Music mute toggle '{musicMuteToggleName}' not found");
        }

        private void SetupEventHandlers()
        {
            if (backButton != null)
            {
                backButton.clicked += OnBackButtonClicked;
            }
            if (confirmButton != null)
            {
                confirmButton.clicked += OnConfirmButtonClicked;
            }
            if (musicSlider != null)
            {
                musicSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
            }
            if (sfxSlider != null)
            {
                sfxSlider.RegisterValueChangedCallback(OnSfxVolumeChanged);
            }
            if (musicMuteToggle != null)
            {
                musicMuteToggle.RegisterValueChangedCallback(OnMusicMuteToggleChanged);
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
            if (musicMuteToggle != null && AudioManager.Instance != null)
            {
                musicMuteToggle.value = !AudioManager.Instance.IsMuted;
            }
        }

        private void SaveSettings()
        {
            SaveVolumeSettingsOnly();
            // Save the mute state when confirm button is clicked
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SaveMuteState();
            }
        }

        private void SaveVolumeSettingsOnly()
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

        private void OnMusicMuteToggleChanged(ChangeEvent<bool> evt)
        {
            if (AudioManager.Instance != null)
            {
                // Temporarily apply the mute state without saving
                AudioManager.Instance.SetMuteState(!evt.newValue);
                Debug.Log($"Music mute temporarily set to: {AudioManager.Instance.IsMuted}");
            }
        }

        private void OnConfirmButtonClicked()
        {
            AudioManager.Instance?.PlayButtonTap();
            Debug.Log("[DEBUG] OnConfirmButtonClicked called");
            SaveSettings();
            // Show How to Play overlay through ScreenManager
            if (ScreenManager.Instance.GetAvailableScreenTypes().Contains("LobbyScreen"))
            {
                ScreenManager.Instance.ShowScreen("LobbyScreen");
            }
            else
            {
                Debug.LogWarning("LobbyScreen not found");
            }
        }

        private void OnBackButtonClicked()
        {
            AudioManager.Instance?.PlayButtonTap();
            Debug.Log("[DEBUG] OnBackButtonClicked called");
            // Save volume settings but not mute state
            SaveVolumeSettingsOnly();
            // Restore the original mute state from PlayerPrefs
            if (AudioManager.Instance != null)
            {
                bool originalMuteState = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
                AudioManager.Instance.SetMuteState(originalMuteState);
            }
            // Show How to Play overlay through ScreenManager
            if (ScreenManager.Instance.GetAvailableScreenTypes().Contains("LobbyScreen"))
            {
                ScreenManager.Instance.ShowScreen("LobbyScreen");
            }
            else
            {
                Debug.LogWarning("LobbyScreen not found");
            }
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