using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Background Music Clips")]
    [SerializeField] private AudioClip bgHomeClip;
    [SerializeField] private AudioClip bgGameClip;

    [Header("Sound Effect Clips")]
    [SerializeField] private AudioClip buttonTapClip;
    [SerializeField] private AudioClip bottlePickClip;
    [SerializeField] private AudioClip bottlePickPourRestrictClip;
    [SerializeField] private AudioClip liquidPourClip;
    [SerializeField] private AudioClip levelCompleteClip;
    [SerializeField] private AudioClip levelFailClip;
    [SerializeField] private AudioClip undoClip;
    [SerializeField] private AudioClip hintClip;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgAudioSource;  // for background music
    [SerializeField] private AudioSource uiAudioSource;  // for buttons/UI taps
    [SerializeField] private AudioSource sfxAudioSource; // for in-game sounds

    // optional volume controls
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 0.6f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    
    // Default values for reset
    private const float DEFAULT_MUSIC_VOLUME = 0.5f;
    private const float DEFAULT_SFX_VOLUME = 0.5f;
    private const bool DEFAULT_MUSIC_MUTED = false;
    private const bool DEFAULT_SFX_MUTED = false;
    
    // Mute states
    public bool IsMuted { get; private set; }
    public bool IsSFXMuted { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Load mute states from PlayerPrefs
        IsMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
        IsSFXMuted = PlayerPrefs.GetInt("SFXMuted", 0) == 1;
        
        if (bgAudioSource != null)
        {
            bgAudioSource.mute = IsMuted;
        }
        if (uiAudioSource != null)
        {
            uiAudioSource.mute = IsSFXMuted;
        }
        if (sfxAudioSource != null)
        {
            sfxAudioSource.mute = IsSFXMuted;
        }
    }

    private void Start()
    {
        ApplyVolume();
    }

    #region Background Music

    public void PlayHomeBGAudio()
    {
        PlayBG(bgHomeClip);
    }

    public void PlayGameBGAudio()
    {
        PlayBG(bgGameClip);
    }

    public void StopBG()
    {
        if (bgAudioSource != null && bgAudioSource.isPlaying)
            bgAudioSource.Stop();
    }

    private void PlayBG(AudioClip clip)
    {
        if (bgAudioSource == null || clip == null) return;
        if (bgAudioSource.clip == clip && bgAudioSource.isPlaying) return;

        bgAudioSource.clip = clip;
        bgAudioSource.loop = true;
        bgAudioSource.volume = musicVolume * masterVolume;
        bgAudioSource.Play();
    }

    #endregion

    #region Sound Effects
    public void PlayButtonTap() => PlayUI(buttonTapClip);
    public void PlayBottlePick() => PlaySFX(bottlePickClip);
    public void PlayBottlePickPourRestrict() => PlaySFX(bottlePickPourRestrictClip);
    public void PlayLiquidPour() => PlaySFX(liquidPourClip);
    public void PlayUndo() => PlaySFX(undoClip);
    public void PlayHint() => PlaySFX(hintClip);
    public void PlayLevelComplete() => PlaySFX(levelCompleteClip);
    public void PlayLevelFail() => PlaySFX(levelFailClip);



    private void PlayUI(AudioClip clip)
    {
        if (clip == null || uiAudioSource == null) return;
        uiAudioSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxAudioSource == null) return;
        sfxAudioSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    #endregion

    #region Volume Management

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyVolume();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplyVolume();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        ApplyVolume();
    }

    private void ApplyVolume()
    {
        if (bgAudioSource != null)
            bgAudioSource.volume = musicVolume * masterVolume;
        // sfx and ui sources use volume multipliers during PlayOneShot
    }

    #endregion

    #region Mute Management
    
    public void ToggleMute()
    {
        IsMuted = !IsMuted;
        
        if (bgAudioSource != null)
        {
            bgAudioSource.mute = IsMuted;
        }
    }
    
    public void SetMuteState(bool isMuted)
    {
        IsMuted = isMuted;
        
        if (bgAudioSource != null)
        {
            bgAudioSource.mute = IsMuted;
        }
    }
    
    public void SetSFXMuteState(bool isSFXMuted)
    {
        IsSFXMuted = isSFXMuted;
        
        if (uiAudioSource != null)
        {
            uiAudioSource.mute = IsSFXMuted;
        }
        if (sfxAudioSource != null)
        {
            sfxAudioSource.mute = IsSFXMuted;
        }
    }
    
    public void SaveMuteState()
    {
        PlayerPrefs.SetInt("MusicMuted", IsMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void SaveSFXMuteState()
    {
        PlayerPrefs.SetInt("SFXMuted", IsSFXMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void SaveAllMuteStates()
    {
        PlayerPrefs.SetInt("MusicMuted", IsMuted ? 1 : 0);
        PlayerPrefs.SetInt("SFXMuted", IsSFXMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void ResetAllAudioSettings()
    {
        // Reset volume values
        musicVolume = DEFAULT_MUSIC_VOLUME;
        sfxVolume = DEFAULT_SFX_VOLUME;
        
        // Reset mute states
        IsMuted = DEFAULT_MUSIC_MUTED;
        IsSFXMuted = DEFAULT_SFX_MUTED;
        
        // Apply volume changes
        ApplyVolume();
        
        // Apply mute changes
        if (bgAudioSource != null)
        {
            bgAudioSource.mute = IsMuted;
        }
        if (uiAudioSource != null)
        {
            uiAudioSource.mute = IsSFXMuted;
        }
        if (sfxAudioSource != null)
        {
            sfxAudioSource.mute = IsSFXMuted;
        }
        
        // Save all settings to PlayerPrefs
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
        PlayerPrefs.SetInt("MusicMuted", IsMuted ? 1 : 0);
        PlayerPrefs.SetInt("SFXMuted", IsSFXMuted ? 1 : 0);
        PlayerPrefs.Save();
        
        Debug.Log("All audio settings reset to defaults");
    }
    
    #endregion

}
