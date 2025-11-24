using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlay : MonoBehaviour
{
    [SerializeField] private string audiosName;
    [SerializeField] private bool isMusic = false;

    private void Start()
    {
        AudioManager.Instance.Play(audiosName, isMusic);
    }
}
