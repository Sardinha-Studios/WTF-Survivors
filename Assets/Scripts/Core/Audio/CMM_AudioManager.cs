using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using UnityEngine;
using MoreMountains.Tools;

// todo: use ScriptableOcject to modularize the sounds creating categories
public class CMM_AudioManager : AudioManager
{
    [Header("Sounds")]
    public List<Sound> soundsData;

    [Header("Volume Settings")]
    public float GeralVolume = 1;
    public float MusicVolume = 1;
    public float SFXVolume = 1;

    private const string GENERAL_KEY = "general_audio_value";
    private const string MUSIC_KEY = "music_audio_value";
    private const string SFX_KEY = "sfx_audio_value";

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        if (!PlayerPrefs.HasKey(GENERAL_KEY))
            PlayerPrefs.SetFloat(GENERAL_KEY, GeralVolume);

        if (!PlayerPrefs.HasKey(MUSIC_KEY))
            PlayerPrefs.SetFloat(MUSIC_KEY, MusicVolume);

        if (!PlayerPrefs.HasKey(SFX_KEY))
            PlayerPrefs.SetFloat(SFX_KEY, SFXVolume);

        ChangeGeralVolume(PlayerPrefs.GetFloat(GENERAL_KEY));
        ChangeMusicVolume(PlayerPrefs.GetFloat(MUSIC_KEY));
        ChangeSFXVolume(PlayerPrefs.GetFloat(SFX_KEY));
    }

    public static ulong GetIdFromString(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            ulong id = BitConverter.ToUInt64(hashBytes, 0);
            return id;
        }
    }

    public override void Play(string newSound, bool isMusic = false)
    {
        var sound = Array.Find(soundsData.ToArray(), sound => sound.Name == newSound);
        if (sound == null) 
        {
            Debug.LogError("Sound \"" + newSound + "\" not found");
            return;
        }

        var ID = (int)GetIdFromString(sound.Clip.name);
        var volume = (isMusic ? MusicVolume : SFXVolume) * GeralVolume;
        var track = isMusic ?
            MMSoundManager.MMSoundManagerTracks.Music :
            MMSoundManager.MMSoundManagerTracks.Sfx;

        var options = MMSoundManagerPlayOptions.Default;
        options.MmSoundManagerTrack = track;
        options.Location = Vector3.zero;
        options.Volume = sound.Volume;
        options.Persistent = isMusic;
        options.Pitch = sound.Pitch;
        options.Loop = sound.Loop;
        options.ID = ID;

        if (isMusic)
        {
            var soundPlaying = MMSoundManager.Instance.GetSoundsPlaying(track).FirstOrDefault();
            if (soundPlaying.ID == ID)
                return;

            MMSoundManager.Instance.StopTrack(track);
        }

        MMSoundManager.Instance.SetTrackVolume(track, volume);
        MMSoundManagerSoundPlayEvent.Trigger(sound.Clip, options);
    }

    public override void StopMusic()
    {        
        ChangeMusicVolumeWithLerp(GeralVolume * MusicVolume, 0.0f, 1.0f);
    }

    public override void SetVolume(AudioTrack track, float newVolume)
    {
        switch (track)
        {
            case AudioTrack.General:
                ChangeGeralVolume(newVolume);
                break;
            case AudioTrack.Music:
                ChangeMusicVolume(newVolume);
                break;
            case AudioTrack.Sfx:
                ChangeSFXVolume(newVolume);
                break;
            default:
                Debug.Log($"Default type");
                break;
        }
    }

    public override float GetVolume(AudioTrack track)
    {
        switch (track)
        {
            case AudioTrack.General:
                return PlayerPrefs.GetFloat(GENERAL_KEY);
            case AudioTrack.Music:
                return PlayerPrefs.GetFloat(MUSIC_KEY);
            case AudioTrack.Sfx:
                return PlayerPrefs.GetFloat(SFX_KEY);
            default:
                Debug.Log($"Type is not implemented!");
                return 0f;
        }
    }

    private  void ChangeGeralVolume(float newVolume)
    {
        GeralVolume = newVolume;
        MMSoundManager.Instance.SetVolumeMaster(GeralVolume);
        MMSoundManager.Instance.SetVolumeMusic(GeralVolume * MusicVolume);
        MMSoundManager.Instance.SetVolumeSfx(GeralVolume * SFXVolume);
        PlayerPrefs.SetFloat(GENERAL_KEY, GeralVolume);
        PlayerPrefs.Save();
    }

    private  void ChangeMusicVolume(float newVolume)
    {
        MusicVolume = newVolume;
        MMSoundManager.Instance.SetVolumeMusic(GeralVolume * MusicVolume);
        PlayerPrefs.SetFloat(MUSIC_KEY, MusicVolume);
        PlayerPrefs.Save();
    }

    private  void ChangeSFXVolume(float newVolume)
    {
        SFXVolume = newVolume;
        MMSoundManager.Instance.SetVolumeSfx(GeralVolume * SFXVolume);
        PlayerPrefs.SetFloat(SFX_KEY, SFXVolume);
        PlayerPrefs.Save();
    }

    public override void ChangeMusicVolumeWithLerp(float startVolume, float finalVolume, float duration)
    {
        var track = MMSoundManager.MMSoundManagerTracks.Music;
        MMSoundManager.Instance.SetTrackVolume(MMSoundManager.MMSoundManagerTracks.Music, startVolume);
        MMSoundManagerTrackFadeEvent.Trigger(MMSoundManagerTrackFadeEvent.Modes.PlayFade, track, duration, finalVolume, MMTweenType.DefaultEaseInCubic);
    }
}
