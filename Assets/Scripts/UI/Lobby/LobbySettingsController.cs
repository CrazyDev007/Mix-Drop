using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Lobby
{
    /// <summary>
    /// Controls the Settings overlay UI, binding toggle controls to the LobbyPreferencesModel
    /// and providing immediate feedback for SFX/VFX changes.
    /// Performance target: overlay open/close ≤ 500ms per PRF-001.
    /// </summary>
    [DisallowMultipleComponent]
    public class LobbySettingsController : MonoBehaviour, ILobbyOverlay
    {
        [SerializeField] private VisualTreeAsset settingsUxml;
        [SerializeField] private LobbyPreferencesModel preferencesModel;
        
        private UIDocument uiDocument;
        private VisualElement rootElement;
        private VisualElement settingsContainer;
        
        // UI Controls
        private Button closeButton;
        private Toggle sfxToggle;
        private Toggle vfxToggle;
        private DropdownField themeDropdown;
        private Button resetButton;
        private Button confirmButton;
        
        // Feedback systems (placeholders for audio/visual feedback)
        [Header("Feedback")]
        [SerializeField] private AudioClip sfxTestClip;
        [SerializeField] private ParticleSystem vfxTestEffect;
        [SerializeField] private AudioSource audioSource;
        
        private bool isInitialized;
        private string overlayId = "Settings";
        
        public string OverlayId => overlayId;
        
        public VisualElement RootElement => rootElement;
        
        public bool IsVisible => settingsContainer != null && settingsContainer.style.display == DisplayStyle.Flex;
        
        public event Action OnOverlayClosed;
        
        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                uiDocument = gameObject.AddComponent<UIDocument>();
            }
            
            // Initialize dependencies if not set
            if (preferencesModel == null)
            {
                preferencesModel = new LobbyPreferencesModel();
            }
            
            // Find or add audio source for SFX feedback
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                }
            }
        }
        
        private void Start()
        {
            InitializeUI();
            BindEvents();
            LoadPreferences();
            isInitialized = true;
        }
        
        private void OnDestroy()
        {
            UnbindEvents();
        }
        
        /// <summary>
        /// Initializes the UI from the UXML template and caches references to controls.
        /// </summary>
        private void InitializeUI()
        {
            if (settingsUxml == null)
            {
                Debug.LogError("LobbySettingsController: Settings UXML is not assigned.");
                return;
            }
            
            rootElement = settingsUxml.Instantiate();
            uiDocument.visualTreeAsset = null; // Clear any existing asset
            uiDocument.rootVisualElement.Clear();
            uiDocument.rootVisualElement.Add(rootElement);
            
            // Cache UI element references - the root element itself is the settings overlay
            settingsContainer = rootElement.Q<VisualElement>("settings-overlay");
            if (settingsContainer == null)
            {
                // If no specific overlay container found, use the root element
                settingsContainer = rootElement;
            }
            
            closeButton = rootElement.Q<Button>("close-button");
            sfxToggle = rootElement.Q<Toggle>("sfx-toggle");
            vfxToggle = rootElement.Q<Toggle>("vfx-toggle");
            themeDropdown = rootElement.Q<DropdownField>("theme-dropdown");
            resetButton = rootElement.Q<Button>("reset-button");
            confirmButton = rootElement.Q<Button>("confirm-button");
            
            // Initialize theme dropdown with available themes
            if (themeDropdown != null)
            {
                themeDropdown.choices = new System.Collections.Generic.List<string> { "Default", "Dark", "Light" };
                themeDropdown.index = 0;
            }
            
            // Initially hide the overlay
            Hide();
        }
        
        /// <summary>
        /// Binds UI control events to their handlers.
        /// </summary>
        private void BindEvents()
        {
            if (closeButton != null)
                closeButton.clicked += HandleCloseButtonClicked;
                
            if (resetButton != null)
                resetButton.clicked += HandleResetButtonClicked;
                
            if (confirmButton != null)
                confirmButton.clicked += HandleConfirmButtonClicked;
                
            if (sfxToggle != null)
                sfxToggle.RegisterValueChangedCallback(HandleSfxToggleChanged);
                
            if (vfxToggle != null)
                vfxToggle.RegisterValueChangedCallback(HandleVfxToggleChanged);
                
            if (themeDropdown != null)
                themeDropdown.RegisterValueChangedCallback(HandleThemeDropdownChanged);
                
            // Subscribe to preference changes
            if (preferencesModel != null)
                preferencesModel.PreferencesChanged += HandlePreferencesChanged;
        }
        
        /// <summary>
        /// Unbinds UI control events.
        /// </summary>
        private void UnbindEvents()
        {
            if (closeButton != null)
                closeButton.clicked -= HandleCloseButtonClicked;
                
            if (resetButton != null)
                resetButton.clicked -= HandleResetButtonClicked;
                
            if (confirmButton != null)
                confirmButton.clicked -= HandleConfirmButtonClicked;
                
            if (sfxToggle != null)
                sfxToggle.UnregisterValueChangedCallback(HandleSfxToggleChanged);
                
            if (vfxToggle != null)
                vfxToggle.UnregisterValueChangedCallback(HandleVfxToggleChanged);
                
            if (themeDropdown != null)
                themeDropdown.UnregisterValueChangedCallback(HandleThemeDropdownChanged);
                
            if (preferencesModel != null)
                preferencesModel.PreferencesChanged -= HandlePreferencesChanged;
        }
        
        /// <summary>
        /// Loads current preferences into the UI controls.
        /// </summary>
        private void LoadPreferences()
        {
            if (preferencesModel == null || !isInitialized) return;
            
            if (sfxToggle != null)
                sfxToggle.SetValueWithoutNotify(preferencesModel.IsSfxEnabled);
                
            if (vfxToggle != null)
                vfxToggle.SetValueWithoutNotify(preferencesModel.IsVfxEnabled);
                
            if (themeDropdown != null)
            {
                var themeIndex = themeDropdown.choices.IndexOf(preferencesModel.ThemeId);
                if (themeIndex >= 0)
                {
                    themeDropdown.index = themeIndex;
                }
            }
        }
        
        #region Event Handlers
        
        private void HandleCloseButtonClicked()
        {
            Hide();
        }
        
        private void HandleResetButtonClicked()
        {
            preferencesModel?.ResetToDefaults();
            LoadPreferences();
        }
        
        private void HandleConfirmButtonClicked()
        {
            Hide();
        }
        
        private void HandleSfxToggleChanged(ChangeEvent<bool> evt)
        {
            if (preferencesModel != null)
            {
                preferencesModel.IsSfxEnabled = evt.newValue;
                
                // Provide immediate audio feedback if SFX is enabled
                if (evt.newValue && sfxTestClip != null && audioSource != null)
                {
                    audioSource.PlayOneShot(sfxTestClip);
                }
            }
        }
        
        private void HandleVfxToggleChanged(ChangeEvent<bool> evt)
        {
            if (preferencesModel != null)
            {
                preferencesModel.IsVfxEnabled = evt.newValue;
                
                // Provide immediate visual feedback if VFX is enabled
                if (evt.newValue && vfxTestEffect != null)
                {
                    vfxTestEffect.Play();
                }
            }
        }
        
        private void HandleThemeDropdownChanged(ChangeEvent<string> evt)
        {
            if (preferencesModel != null)
            {
                preferencesModel.ThemeId = evt.newValue;
            }
        }
        
        private void HandlePreferencesChanged(LobbyPreferencesSnapshot snapshot)
        {
            // UI is already updated via individual handlers, but this ensures
            // consistency if preferences are changed externally
            if (isInitialized)
            {
                LoadPreferences();
            }
        }
        
        #endregion
        
        #region ILobbyOverlay Implementation
        
        public void Show()
        {
            if (settingsContainer != null)
            {
                settingsContainer.style.display = DisplayStyle.Flex;
            }
        }

        public void Hide()
        {
            if (settingsContainer != null)
            {
                settingsContainer.style.display = DisplayStyle.None;
                OnOverlayClosed?.Invoke();
            }
        }
        
        #endregion
        
        /// <summary>
        /// Validates that all required UI elements are present.
        /// </summary>
        private bool ValidateUIElements()
        {
            if (settingsContainer == null)
            {
                Debug.LogError("LobbySettingsController: Settings container not found.");
                return false;
            }
            
            if (closeButton == null)
                Debug.LogWarning("LobbySettingsController: Close button not found.");
                
            if (sfxToggle == null)
                Debug.LogWarning("LobbySettingsController: SFX toggle not found.");
                
            if (vfxToggle == null)
                Debug.LogWarning("LobbySettingsController: VFX toggle not found.");
                
            if (themeDropdown == null)
                Debug.LogWarning("LobbySettingsController: Theme dropdown not found.");
                
            return true;
        }
    }
}