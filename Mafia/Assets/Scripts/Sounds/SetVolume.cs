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
        // �����̴��� ���� ����� ������ SetLevel �޼��带 ȣ���մϴ�.
        slider.onValueChanged.AddListener(SetLevel);
    }

    public void SetLevel(float sliderVal)
    {
        // ����� �ͼ��� ������ �����մϴ�.
        mixer.SetFloat("BGM", Mathf.Log10(sliderVal) * 20);

        // �����̴� ���� 0-100 ������ ��ȯ�Ͽ� �ؽ�Ʈ�� ǥ���մϴ�.
        int volumePercent = Mathf.RoundToInt(sliderVal * 100);
        volumeText.text = volumePercent.ToString();
    }
}