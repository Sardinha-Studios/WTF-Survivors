using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LLAudioManager : AudioManager
{
    [Header("Sounds")]
    public List<Sound> soundsData;

    [Header("Volume Settings")]
    public float GeralVolume = 1;
    public float MusicVolume = 1;
    public float SFXVolume = 1;

    private Sound[] sounds;
    private Sound m_CurrentMusic;

    private const string GENERAL_KEY = "general_audio_value";
    private const string MUSIC_KEY = "general_audio_value";
    private const string SFX_KEY = "general_audio_value";

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        SetupAudioSources();

        if (!PlayerPrefs.HasKey(GENERAL_KEY))
            PlayerPrefs.SetFloat(GENERAL_KEY, GeralVolume);

        if (!PlayerPrefs.HasKey(MUSIC_KEY))
            PlayerPrefs.SetFloat(MUSIC_KEY, MusicVolume);

        if (!PlayerPrefs.HasKey(SFX_KEY))
            PlayerPrefs.SetFloat(SFX_KEY, SFXVolume);

        GeralVolume = PlayerPrefs.GetFloat(GENERAL_KEY);
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_KEY);
        SFXVolume = PlayerPrefs.GetFloat(SFX_KEY);
    }

    private void SetupAudioSources()
    {
        sounds = soundsData.ToArray();
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].Source = gameObject.AddComponent<AudioSource>();
            sounds[i].Source.clip = sounds[i].Clip;
            sounds[i].Source.volume = sounds[i].Volume;
            sounds[i].Source.pitch = sounds[i].Pitch;
            sounds[i].Source.loop = sounds[i].Loop;
            sounds[i].Source.playOnAwake = false;
        }
    }

    public override void Play(string newSound, bool isMusic = false)
    {
        var sound = Array.Find(sounds, sound => sound.Name == newSound);
        if (sound == null) 
        {
            Debug.LogError("Sound \"" + newSound + "\" not found");
            return;
        }

        if (isMusic)
        {
            if (m_CurrentMusic != null)
            {
                if (m_CurrentMusic == sound) return;
                else m_CurrentMusic.Source.Stop();
            }
            m_CurrentMusic = sound;
        }

        sound.Source.Stop();
        sound.Source.Play();
    }

    public override void StopMusic()
    {
        if (m_CurrentMusic != null)
        {
            m_CurrentMusic.Source.Stop();
        }
    }

    public override void SetVolume(AudioTrack track, float newVolume)
    {

    }

    public override float GetVolume(AudioTrack track)
    {
        return 0f;
    }

    private void ChangeGeralVolume(float newVolume)
    {
        GeralVolume = newVolume;
        PlayerPrefs.SetFloat(GENERAL_KEY, GeralVolume);

        foreach (Sound sound in sounds)
        {
            if (sound.Type == SoundType.sfx) sound.Source.volume = sound.Volume * GeralVolume * SFXVolume;
            else sound.Source.volume = sound.Volume * GeralVolume * MusicVolume;
        }
    }

    private void ChangeSFXVolume(float newVolume)
    {
        SFXVolume = newVolume;
        PlayerPrefs.SetFloat(SFX_KEY, SFXVolume);

        foreach (Sound sound in sounds)
        {
            if (sound.Type != SoundType.sfx) continue;
            sound.Source.volume = sound.Volume * GeralVolume * SFXVolume;
        }
    }

    private void ChangeMusicVolume(float newVolume)
    {
        MusicVolume = newVolume;
        PlayerPrefs.SetFloat(MUSIC_KEY, MusicVolume);

        foreach (Sound sounds in sounds)
        {
            if (sounds.Type != SoundType.music) continue;
            sounds.Source.volume = sounds.Volume * GeralVolume * MusicVolume;
        }
    }

    public override void ChangeMusicVolumeWithLerp(float volume, float lerpTime, float startVolume= -1)
    {
        if (m_CurrentMusic == null) return;
        if (startVolume == -1) startVolume = m_CurrentMusic.Source.volume;

        StopAllCoroutines();
        StartCoroutine(LerpMusicVolume(m_CurrentMusic, startVolume, volume, lerpTime));
    }

    private IEnumerator LerpMusicVolume(Sound music, float startVolume, float volume, float lerpTime)
    {
        float oldVolume = startVolume;
        volume *= music.Volume * GeralVolume * MusicVolume;
        float lerp = 0;

        while (lerp <= lerpTime)
        {
            music.Source.volume = Mathf.Lerp(oldVolume, volume, lerp / lerpTime);
            yield return null;
            lerp += Time.deltaTime;
        }
    }
}
