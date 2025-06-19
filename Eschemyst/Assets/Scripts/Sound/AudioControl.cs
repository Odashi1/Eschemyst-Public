using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    public void SetMasterVolume(float VolumeLevel)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(VolumeLevel) * 20f);
    }
    public void SetSoundFXVolume(float VolumeLevel)
    {
        audioMixer.SetFloat("SoundFXVolume", Mathf.Log10(VolumeLevel) * 20f);
    }
    public void SetMusicVolume(float VolumeLevel)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(VolumeLevel) * 20f);
    }
}
