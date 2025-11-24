using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private Slider audioSlider;
    [SerializeField] private AudioTrack type;

    private void Start()
    {
        audioSlider.maxValue = 1.0f;
        audioSlider.value = AudioManager.Instance.GetVolume(type);
        audioSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volume)
    {
        AudioManager.Instance.SetVolume(type, volume);
    }
}
