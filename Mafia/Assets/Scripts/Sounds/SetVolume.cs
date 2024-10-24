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

    private void Start()
    {
        // 슬라이더의 값이 변경될 때마다 SetLevel 메서드를 호출합니다.
        slider.onValueChanged.AddListener(SetLevel);

        // 씬 로드 이벤트에 대한 리스너를 추가합니다.
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 현재 씬에 대해 초기 설정을 수행합니다.
        UpdateUIForCurrentScene();
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

    void UpdateUIForCurrentScene()
    {
        // 현재 씬의 이름을 가져옵니다.
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Level_1")
        {
            // Level_1 씬에서는 슬라이더를 비활성화하고 텍스트를 "없음"으로 설정합니다.
            slider.interactable = false;
            volumeText.text = "없음";
        }
        else
        {
            // 다른 씬에서는 슬라이더를 활성화하고 현재 볼륨 값을 표시합니다.
            slider.interactable = true;
            SetLevel(slider.value);
        }
    }

    public void SetLevel(float sliderVal)
    {
        // 현재 씬이 Level_1이 아닐 때만 볼륨을 설정합니다.
        if (SceneManager.GetActiveScene().name != "Level_1")
        {
            // 오디오 믹서의 볼륨을 설정합니다.
            mixer.SetFloat("BGM", Mathf.Log10(sliderVal) * 20);
            // 슬라이더 값을 0-100 범위로 변환하여 텍스트로 표시합니다.
            int volumePercent = Mathf.RoundToInt(sliderVal * 100);
            volumeText.text = volumePercent.ToString();
        }
    }
}