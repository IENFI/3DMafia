using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;

    public void SetLevel(float sliderVal)
    {
        mixer.SetFloat("BGM", Mathf.Log10(sliderVal) * 20);
    }
}