using System;
using UnityEngine;

/// <summary>
/// Interface for audio-specific Unity services that need to be abstracted
/// </summary>
public interface IAudioService
{
    /// <summary>
    /// Plays a sound effect
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="volume">The volume to play the clip at (0.0 to 1.0)</param>
    /// <param name="pitch">The pitch to play the clip at (default: 1.0)</param>
    void PlaySoundEffect(AudioClip clip, float volume = 1.0f, float pitch = 1.0f);
    
    /// <summary>
    /// Plays a sound effect at a specific position
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="position">The position to play the clip at</param>
    /// <param name="volume">The volume to play the clip at (0.0 to 1.0)</param>
    /// <param name="pitch">The pitch to play the clip at (default: 1.0)</param>
    void PlaySoundEffectAtPosition(AudioClip clip, Vector3 position, float volume = 1.0f, float pitch = 1.0f);
    
    /// <summary>
    /// Plays background music
    /// </summary>
    /// <param name="clip">The audio clip to play</param>
    /// <param name="volume">The volume to play the clip at (0.0 to 1.0)</param>
    /// <param name="loop">Whether the music should loop (default: true)</param>
    void PlayMusic(AudioClip clip, float volume = 1.0f, bool loop = true);
    
    /// <summary>
    /// Stops the currently playing background music
    /// </summary>
    void StopMusic();
    
    /// <summary>
    /// Pauses the currently playing background music
    /// </summary>
    void PauseMusic();
    
    /// <summary>
    /// Resumes the currently paused background music
    /// </summary>
    void ResumeMusic();
    
    /// <summary>
    /// Sets the volume of the sound effects
    /// </summary>
    /// <param name="volume">The volume to set (0.0 to 1.0)</param>
    void SetSoundEffectVolume(float volume);
    
    /// <summary>
    /// Gets the volume of the sound effects
    /// </summary>
    /// <returns>The volume of the sound effects (0.0 to 1.0)</returns>
    float GetSoundEffectVolume();
    
    /// <summary>
    /// Sets the volume of the background music
    /// </summary>
    /// <param name="volume">The volume to set (0.0 to 1.0)</param>
    void SetMusicVolume(float volume);
    
    /// <summary>
    /// Gets the volume of the background music
    /// </summary>
    /// <returns>The volume of the background music (0.0 to 1.0)</returns>
    float GetMusicVolume();
    
    /// <summary>
    /// Mutes all sound effects
    /// </summary>
    /// <param name="muted">Whether to mute the sound effects</param>
    void MuteSoundEffects(bool muted);
    
    /// <summary>
    /// Checks if sound effects are muted
    /// </summary>
    /// <returns>True if sound effects are muted</returns>
    bool IsSoundEffectsMuted();
    
    /// <summary>
    /// Mutes all background music
    /// </summary>
    /// <param name="muted">Whether to mute the background music</param>
    void MuteMusic(bool muted);
    
    /// <summary>
    /// Checks if background music is muted
    /// </summary>
    /// <returns>True if background music is muted</returns>
    bool IsMusicMuted();
    
    /// <summary>
    /// Loads an audio clip from resources
    /// </summary>
    /// <param name="path">The path to the audio clip in resources</param>
    /// <returns>The loaded audio clip</returns>
    AudioClip LoadAudioClip(string path);
    
    /// <summary>
    /// Loads an audio clip asynchronously from resources
    /// </summary>
    /// <param name="path">The path to the audio clip in resources</param>
    /// <param name="callback">Callback to invoke when the clip is loaded</param>
    void LoadAudioClipAsync(string path, Action<AudioClip> callback);
}