using LL;

public abstract class AudioManager : Singleton<AudioManager>
{
    public abstract void Play(string newSound, bool isMusic = false);
    public abstract void StopMusic();
    public abstract void SetVolume(AudioTrack track, float newVolume);
    public abstract float GetVolume(AudioTrack track);
    public abstract void ChangeMusicVolumeWithLerp(float volume, float lerpTime, float startVolume = 0);
}

// todo: implement audio tracks
public enum AudioTrack 
{
    General,
    Music,
    Sfx,
}
