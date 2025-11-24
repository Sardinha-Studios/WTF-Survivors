using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundsData", menuName = "ScriptableObjects/Sounds", order = 10)]
public class Sounds : ScriptableObject
{
    [Header("Sounds")]
    public List<Sound> sounds;
}

[System.Serializable]
public class Sound
{
    [HideInInspector]
    public AudioSource Source = null;
    public string Name = "";
    public AudioClip Clip = null;
    public SoundType Type = SoundType.sfx;
    public bool Loop = false;

    [Range(0f, 1f)] public float Volume = 1f;
    [Range(0.1f, 3f)] public float Pitch = 1f;
}

[System.Serializable]
public enum SoundType
{
    sfx,
    music
}
