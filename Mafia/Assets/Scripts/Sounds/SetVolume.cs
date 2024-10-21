using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider slider;
    public TMP_Text volumeText;

    private void Start()
    {
        // 슬라이더의 값이 변경될 때마다 SetLevel 메서드를 호출합니다.
        slider.onValueChanged.AddListener(SetLevel);
    }

    public void SetLevel(float sliderVal)
    {
        // 오디오 믹서의 볼륨을 설정합니다.
        mixer.SetFloat("BGM", Mathf.Log10(sliderVal) * 20);

        // 슬라이더 값을 0-100 범위로 변환하여 텍스트로 표시합니다.
        int volumePercent = Mathf.RoundToInt(sliderVal * 100);
        volumeText.text = volumePercent.ToString();
    }
}