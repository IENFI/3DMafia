using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;

public class MicrophoneVolumeIndicator : MonoBehaviour
{
    private Recorder recorder;
    public Image volumeIndicator; // UI에 사용할 이미지 (볼륨 표시용)
    public float volumeThreshold = 0.1f; // 음성 인식 기준 볼륨 레벨 (조정 가능)

    // Start is called before the first frame update
    void Start()
    {
        recorder = GetComponent<Recorder>();
        if (recorder == null)
        {
            Debug.LogWarning("Recorder component not found on this object.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (recorder != null && recorder.LevelMeter != null)
        {
            // 현재 입력 음량을 가져옴
            float currentVolume = recorder.LevelMeter.CurrentPeakAmp;

            // UI 인디케이터로 볼륨 표시 (크기에 따라 색상 또는 투명도를 변경 가능)
            if (volumeIndicator != null)
            {
                // 현재 볼륨에 따라 투명도 조절 (0~1 범위로 조절)
                float alpha = Mathf.Clamp01(currentVolume * 10);
                Color color = volumeIndicator.color;
                color.a = alpha; // 투명도 설정
                volumeIndicator.color = color;

                // 임계값을 초과하면 색상 변경
                volumeIndicator.color = currentVolume > volumeThreshold ? new Color(color.r, color.g, color.b, alpha) : new Color(color.r, color.g, color.b, alpha * 0.5f);
            }
        }
    }
}
