using UnityEngine;
using UnityEngine.UIElements;

namespace Examples
{
    /// <summary>
    /// Example component demonstrating how to use the abstraction services
    /// </summary>
    public class AbstractionServicesExample : MonoBehaviour
    {
        [SerializeField]
        private UIDocument _uiDocument;
        
        private IUnityService _unityService;
        private IUIService _uiService;
        private IAudioService _audioService;
        private IInputService _inputService;
        
        private VisualElement _rootElement;
        private Button _playSoundButton;
        private Button _playMusicButton;
        private Button _stopMusicButton;
        private Label _statusLabel;
        
        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        private void Start()
        {
            // Get services from the service locator
            _unityService = ServiceLocator.Container.Resolve<IUnityService>();
            _uiService = ServiceLocator.Container.Resolve<IUIService>();
            _audioService = ServiceLocator.Container.Resolve<IAudioService>();
            _inputService = ServiceLocator.Container.Resolve<IInputService>();
            
            // Set up UI
            SetupUI();
            
            // Register input callbacks
            RegisterInputCallbacks();
            
            // Log initialization
            _unityService.Log("AbstractionServicesExample initialized successfully");
        }
        
        /// <summary>
        /// Sets up the UI elements
        /// </summary>
        private void SetupUI()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }
            
            if (_uiDocument != null)
            {
                _rootElement = _uiDocument.rootVisualElement;
                
                // Get UI elements
                _playSoundButton = _uiService.Query<Button>(_rootElement, "PlaySoundButton");
                _playMusicButton = _uiService.Query<Button>(_rootElement, "PlayMusicButton");
                _stopMusicButton = _uiService.Query<Button>(_rootElement, "StopMusicButton");
                _statusLabel = _uiService.Query<Label>(_rootElement, "StatusLabel");
                
                // Register button click callbacks
                if (_playSoundButton != null)
                {
                    _uiService.RegisterClickCallback(_playSoundButton, OnPlaySoundButtonClicked);
                }
                
                if (_playMusicButton != null)
                {
                    _uiService.RegisterClickCallback(_playMusicButton, OnPlayMusicButtonClicked);
                }
                
                if (_stopMusicButton != null)
                {
                    _uiService.RegisterClickCallback(_stopMusicButton, OnStopMusicButtonClicked);
                }
                
                // Set initial status
                if (_statusLabel != null)
                {
                    _uiService.SetLabelText(_statusLabel, "Ready");
                }
            }
        }
        
        /// <summary>
        /// Registers input callbacks
        /// </summary>
        private void RegisterInputCallbacks()
        {
            // Register button press callbacks
            _inputService.ButtonPressed += OnButtonPressed;
            _inputService.ButtonReleased += OnButtonReleased;
            
            // Register mouse move callback
            _inputService.MouseMoved += OnMouseMoved;
            
            // Register touch callbacks
            _inputService.TouchStarted += OnTouchStarted;
            _inputService.TouchMoved += OnTouchMoved;
            _inputService.TouchEnded += OnTouchEnded;
        }
        
        /// <summary>
        /// Called when a button is pressed
        /// </summary>
        /// <param name="buttonName">The name of the button that was pressed</param>
        private void OnButtonPressed(string buttonName)
        {
            _unityService.Log($"Button pressed: {buttonName}");
            UpdateStatus($"Button pressed: {buttonName}");
        }
        
        /// <summary>
        /// Called when a button is released
        /// </summary>
        /// <param name="buttonName">The name of the button that was released</param>
        private void OnButtonReleased(string buttonName)
        {
            _unityService.Log($"Button released: {buttonName}");
            UpdateStatus($"Button released: {buttonName}");
        }
        
        /// <summary>
        /// Called when the mouse is moved
        /// </summary>
        /// <param name="position">The current mouse position</param>
        private void OnMouseMoved(Vector2 position)
        {
            // Update status with mouse position
            UpdateStatus($"Mouse position: {position}");
        }
        
        /// <summary>
        /// Called when a touch starts
        /// </summary>
        /// <param name="position">The touch position</param>
        private void OnTouchStarted(Vector2 position)
        {
            _unityService.Log($"Touch started at: {position}");
            UpdateStatus($"Touch started at: {position}");
        }
        
        /// <summary>
        /// Called when a touch moves
        /// </summary>
        /// <param name="position">The touch position</param>
        private void OnTouchMoved(Vector2 position)
        {
            // Update status with touch position
            UpdateStatus($"Touch position: {position}");
        }
        
        /// <summary>
        /// Called when a touch ends
        /// </summary>
        /// <param name="position">The touch position</param>
        private void OnTouchEnded(Vector2 position)
        {
            _unityService.Log($"Touch ended at: {position}");
            UpdateStatus($"Touch ended at: {position}");
        }
        
        /// <summary>
        /// Called when the play sound button is clicked
        /// </summary>
        /// <param name="evt">The click event</param>
        private void OnPlaySoundButtonClicked(ClickEvent evt)
        {
            _unityService.Log("Play sound button clicked");
            
            // Load and play a sound effect
            AudioClip soundClip = _audioService.LoadAudioClip("Sounds/Click");
            if (soundClip != null)
            {
                _audioService.PlaySoundEffect(soundClip);
                UpdateStatus("Playing sound effect");
            }
            else
            {
                _unityService.LogWarning("Sound clip not found");
                UpdateStatus("Sound clip not found");
            }
        }
        
        /// <summary>
        /// Called when the play music button is clicked
        /// </summary>
        /// <param name="evt">The click event</param>
        private void OnPlayMusicButtonClicked(ClickEvent evt)
        {
            _unityService.Log("Play music button clicked");
            
            // Load and play background music
            AudioClip musicClip = _audioService.LoadAudioClip("Music/Background");
            if (musicClip != null)
            {
                _audioService.PlayMusic(musicClip);
                UpdateStatus("Playing background music");
            }
            else
            {
                _unityService.LogWarning("Music clip not found");
                UpdateStatus("Music clip not found");
            }
        }
        
        /// <summary>
        /// Called when the stop music button is clicked
        /// </summary>
        /// <param name="evt">The click event</param>
        private void OnStopMusicButtonClicked(ClickEvent evt)
        {
            _unityService.Log("Stop music button clicked");
            
            // Stop background music
            _audioService.StopMusic();
            UpdateStatus("Stopped background music");
        }
        
        /// <summary>
        /// Updates the status label
        /// </summary>
        /// <param name="status">The status text</param>
        private void UpdateStatus(string status)
        {
            if (_statusLabel != null)
            {
                _uiService.SetLabelText(_statusLabel, status);
            }
        }
        
        /// <summary>
        /// OnDestroy is called when the script instance is being destroyed
        /// </summary>
        private void OnDestroy()
        {
            // Unregister input callbacks
            if (_inputService != null)
            {
                _inputService.ButtonPressed -= OnButtonPressed;
                _inputService.ButtonReleased -= OnButtonReleased;
                _inputService.MouseMoved -= OnMouseMoved;
                _inputService.TouchStarted -= OnTouchStarted;
                _inputService.TouchMoved -= OnTouchMoved;
                _inputService.TouchEnded -= OnTouchEnded;
            }
        }
    }
}