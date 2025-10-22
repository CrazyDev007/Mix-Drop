using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Implementation of the audio service interface
/// </summary>
public class AudioService : MonoBehaviour, IAudioService
{
    [SerializeField]
    private AudioMixer _audioMixer;
    
    [SerializeField]
    private AudioSource _musicSource;
    
    [SerializeField]
    private int _soundEffectPoolSize = 10;
    
    private AudioSource[] _soundEffectPool;
    private int _currentSoundEffectIndex = 0;
    
    private float _soundEffectVolume = 1.0f;
    private float _musicVolume = 1.0f;
    private bool _soundEffectsMuted = false;
    private bool _musicMuted = false;
    
    /// <summary>
    /// Awake is called when the script instance is being loaded
    /// </summary>
    private void Awake()
    {
        InitializeAudioSources();
    }
    
    /// <summary>
    /// Initializes the audio sources
    /// </summary>
    private void InitializeAudioSources()
    {
        // Initialize music source
        if (_musicSource == null)
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
        }
        
        // Initialize sound effect pool
        _soundEffectPool = new AudioSource[_soundEffectPoolSize];
        for (int i = 0; i < _soundEffectPoolSize; i++)
        {
            _soundEffectPool[i] = gameObject.AddComponent<AudioSource>();
            _soundEffectPool[i].playOnAwake = false;
        }
    }
    
    /// <summary>
    /// Plays a sound effect
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="volume">The volume to play the clip at (0.0 to 1.0)</param>
    /// <param name="pitch">The pitch to play the clip at (default: 1.0)</param>
    public void PlaySoundEffect(AudioClip clip, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clip == null || _soundEffectsMuted)
        {
            return;
        }
        
        AudioSource source = GetNextAvailableSoundEffectSource();
        if (source != null)
        {
            source.clip = clip;
            source.volume = volume * _soundEffectVolume;
            source.pitch = pitch;
            source.Play();
        }
    }
    
    /// <summary>
    /// Plays a sound effect at a specific position
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="position">The position to play the clip at</param>
    /// <param name="volume">The volume to play the clip at (0.0 to 1.0)</param>
    /// <param name="pitch">The pitch to play the clip at (default: 1.0)</param>
    public void PlaySoundEffectAtPosition(AudioClip clip, Vector3 position, float volume = 1.0f, float pitch = 1.0f)
    {
        if (clip == null || _soundEffectsMuted)
        {
            return;
        }
        
        AudioSource.PlayClipAtPoint(clip, position, volume * _soundEffectVolume);
    }
    
    /// <summary>
    /// Plays background music
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="volume">The volume to play the clip at (0.0 to 1.0)</param>
    /// <param name="loop">Whether the music should loop (default: true)</param>
    public void PlayMusic(AudioClip clip, float volume = 1.0f, bool loop = true)
    {
        if (clip == null || _musicMuted)
        {
            return;
        }
        
        _musicSource.clip = clip;
        _musicSource.volume = volume * _musicVolume;
        _musicSource.loop = loop;
        _musicSource.Play();
    }
    
    /// <summary>
    /// Stops the currently playing background music
    /// </summary>
    public void StopMusic()
    {
        _musicSource.Stop();
    }
    
    /// <summary>
    /// Pauses the currently playing background music
    /// </summary>
    public void PauseMusic()
    {
        _musicSource.Pause();
    }
    
    /// <summary>
    /// Resumes the currently paused background music
    /// </summary>
    public void ResumeMusic()
    {
        _musicSource.UnPause();
    }
    
    /// <summary>
    /// Sets the volume of the sound effects
    /// </summary>
    /// <param name="volume">The volume to set (0.0 to 1.0)</param>
    public void SetSoundEffectVolume(float volume)
    {
        _soundEffectVolume = Mathf.Clamp01(volume);
        
        if (_audioMixer != null)
        {
            _audioMixer.SetFloat("SoundEffectVolume", ConvertToDecibel(_soundEffectVolume));
        }
    }
    
    /// <summary>
    /// Gets the volume of the sound effects
    /// </summary>
    /// <returns>The volume of the sound effects (0.0 to 1.0)</returns>
    public float GetSoundEffectVolume()
    {
        return _soundEffectVolume;
    }
    
    /// <summary>
    /// Sets the volume of the background music
    /// </summary>
    /// <param name="volume">The volume to set (0.0 to 1.0)</param>
    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        _musicSource.volume = _musicVolume;
        
        if (_audioMixer != null)
        {
            _audioMixer.SetFloat("MusicVolume", ConvertToDecibel(_musicVolume));
        }
    }
    
    /// <summary>
    /// Gets the volume of the background music
    /// </summary>
    /// <returns>The volume of the background music (0.0 to 1.0)</returns>
    public float GetMusicVolume()
    {
        return _musicVolume;
    }
    
    /// <summary>
    /// Mutes all sound effects
    /// </summary>
    /// <param name="muted">Whether to mute the sound effects</param>
    public void MuteSoundEffects(bool muted)
    {
        _soundEffectsMuted = muted;
        
        if (_audioMixer != null)
        {
            _audioMixer.SetFloat("SoundEffectMute", muted ? -80f : 0f);
        }
    }
    
    /// <summary>
    /// Checks if sound effects are muted
    /// </summary>
    /// <returns>True if sound effects are muted</returns>
    public bool IsSoundEffectsMuted()
    {
        return _soundEffectsMuted;
    }
    
    /// <summary>
    /// Mutes all background music
    /// </summary>
    /// <param name="muted">Whether to mute the background music</param>
    public void MuteMusic(bool muted)
    {
        _musicMuted = muted;
        _musicSource.mute = muted;
        
        if (_audioMixer != null)
        {
            _audioMixer.SetFloat("MusicMute", muted ? -80f : 0f);
        }
    }
    
    /// <summary>
    /// Checks if background music is muted
    /// </summary>
    /// <returns>True if background music is muted</returns>
    public bool IsMusicMuted()
    {
        return _musicMuted;
    }
    
    /// <summary>
    /// Loads an audio clip from resources
    /// </summary>
    /// <param name="path">The path to the audio clip in resources</param>
    /// <returns>The loaded audio clip</returns>
    public AudioClip LoadAudioClip(string path)
    {
        return Resources.Load<AudioClip>(path);
    }
    
    /// <summary>
    /// Loads an audio clip asynchronously from resources
    /// </summary>
    /// <param name="path">The path to the audio clip in resources</param>
    /// <param name="callback">Callback to invoke when the clip is loaded</param>
    public void LoadAudioClipAsync(string path, Action<AudioClip> callback)
    {
        StartCoroutine(LoadAudioClipAsyncRoutine(path, callback));
    }
    
    /// <summary>
    /// Gets the next available sound effect source from the pool
    /// </summary>
    /// <returns>The next available sound effect source</returns>
    private AudioSource GetNextAvailableSoundEffectSource()
    {
        for (int i = 0; i < _soundEffectPoolSize; i++)
        {
            int index = (_currentSoundEffectIndex + i) % _soundEffectPoolSize;
            if (!_soundEffectPool[index].isPlaying)
            {
                _currentSoundEffectIndex = (index + 1) % _soundEffectPoolSize;
                return _soundEffectPool[index];
            }
        }
        
        // If all sources are playing, return the oldest one
        _currentSoundEffectIndex = (_currentSoundEffectIndex + 1) % _soundEffectPoolSize;
        return _soundEffectPool[_currentSoundEffectIndex];
    }
    
    /// <summary>
    /// Converts a linear volume value to decibel
    /// </summary>
    /// <param name="linear">The linear volume value</param>
    /// <returns>The decibel value</returns>
    private float ConvertToDecibel(float linear)
    {
        if (linear <= 0)
        {
            return -80f;
        }
        
        return 20f * Mathf.Log10(linear);
    }
    
    /// <summary>
    /// Coroutine for loading an audio clip asynchronously
    /// </summary>
    /// <param name="path">The path to the audio clip in resources</param>
    /// <param name="callback">Callback to invoke when the clip is loaded</param>
    /// <returns>IEnumerator for the coroutine</returns>
    private IEnumerator LoadAudioClipAsyncRoutine(string path, Action<AudioClip> callback)
    {
        ResourceRequest request = Resources.LoadAsync<AudioClip>(path);
        
        yield return request;
        
        if (request.asset != null)
        {
            callback?.Invoke(request.asset as AudioClip);
        }
        else
        {
            callback?.Invoke(null);
        }
    }
}