using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("SFX Clips")]
    [SerializeField] private AudioClip buttonTapClip;
    [SerializeField] private AudioClip liquidPoreClip;
    [SerializeField] private AudioClip levelCompleteClip;
    [SerializeField] private AudioClip levelFailClip;

    [Header("BG Clips")]
    [SerializeField] private AudioClip bgHomeClip;
    [SerializeField] private AudioClip bgGameClip;

    [Header("Audio Source References")]
    [SerializeField] private AudioSource bgAudioSource;
    [SerializeField] private AudioSource buttontapAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this.gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayButtonClickAudio() { PlayOnButtonCLick(buttonTapClip); }
    public void PlayHomeBGAudio() { PlayBG(bgGameClip); }
    public void PlayGameBGAudio() { PlayBG(bgGameClip); }



    void PlayOnButtonCLick(AudioClip clip)
    {
        if (clip == null) return;
        buttontapAudioSource?.PlayOneShot(clip);
    }
    void PlayBG(AudioClip clip)
    {
        if (clip == null) return;
        bgAudioSource.clip = clip;
        bgAudioSource.loop = true;
        bgAudioSource?.Play();
    }
}
