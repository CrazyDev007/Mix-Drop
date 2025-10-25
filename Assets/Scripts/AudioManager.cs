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

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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

}
