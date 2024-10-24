using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider slider;
    public TMP_Text volumeText;

    // 새로 추가할 변수들
    public Image soundIcon;    // Sound 이미지
    public Image muteIcon;     // Mute 이미지
    private bool isMuted = false;

    private void Start()
    {
        // 슬라이더의 값이 변경될 때마다 SetLevel 메서드를 호출합니다.
        slider.onValueChanged.AddListener(SetLevel);

        // 씬 로드 이벤트에 대한 리스너를 추가합니다.
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 현재 씬에 대해 초기 설정을 수행합니다.
        UpdateUIForCurrentScene();

        // 이미지 클릭 이벤트 추가
        if (soundIcon != null)
            soundIcon.GetComponent<Button>().onClick.AddListener(ToggleMute);
        if (muteIcon != null)
            muteIcon.GetComponent<Button>().onClick.AddListener(ToggleMute);

        // 초기 이미지 상태 설정
        UpdateSoundIcon(slider.value);
    }

    private void OnDestroy()
    {
        // 스크립트가 파괴될 때 이벤트 리스너를 제거합니다.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUIForCurrentScene();
    }

    // 새로 추가하는 메서드: 음소거 토글
    public void ToggleMute()
    {
        if (!isMuted)
        {
            // 음소거로 전환
            isMuted = true;
            slider.value = 0f;
        }
        else
        {
            // 음소거 해제 (볼륨 30으로 설정)
            isMuted = false;
            slider.value = 0.3f;
        }
        SetLevel(slider.value);
    }

    // 새로 추가하는 메서드: 아이콘 상태 업데이트
    private void UpdateSoundIcon(float volume)
    {
        if (soundIcon != null && muteIcon != null)
        {
            soundIcon.gameObject.SetActive(volume > 0.0001);
            muteIcon.gameObject.SetActive(volume <= 0.0001);
        }
    }

    void UpdateUIForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Level_1")
        {
            slider.interactable = false;
            volumeText.text = "없음";
            // Level_1에서는 아이콘도 비활성화
            if (soundIcon != null) soundIcon.gameObject.SetActive(false);
            if (muteIcon != null) muteIcon.gameObject.SetActive(false);
        }
        else
        {
            slider.interactable = true;
            SetLevel(slider.value);
        }
    }

    public void SetLevel(float sliderVal)
    {
        if (SceneManager.GetActiveScene().name != "Level_1")
        {
            mixer.SetFloat("BGM", Mathf.Log10(sliderVal) * 20);
            int volumePercent = Mathf.RoundToInt(sliderVal * 100);
            volumeText.text = volumePercent.ToString();

            // 아이콘 상태 업데이트
            UpdateSoundIcon(sliderVal);
        }
    }
}