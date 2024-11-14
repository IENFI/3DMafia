using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;

    [Header("배경음악")]
    public Slider bgmSlider;
    public TMP_Text bgmVolumeText;
    public Image bgmSoundIcon;
    public Image bgmMuteIcon;
    private bool isBgmMuted = false;

    [Header("효과음")]
    public Slider sfxSlider;
    public TMP_Text sfxVolumeText;
    public Image sfxSoundIcon;
    public Image sfxMuteIcon;
    private bool isSfxMuted = false;

    private void Start()
    {
        // BGM 슬라이더 이벤트 설정
        bgmSlider.onValueChanged.AddListener(SetBGMLevel);

        // 효과음 슬라이더 이벤트 설정
        sfxSlider.onValueChanged.AddListener(SetSFXLevel);

        // 씬 로드 이벤트 리스너 추가
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 초기 UI 상태 업데이트
        UpdateUIForCurrentScene();

        // BGM 아이콘 클릭 이벤트 추가
        if (bgmSoundIcon != null)
            bgmSoundIcon.GetComponent<Button>().onClick.AddListener(ToggleBGMMute);
        if (bgmMuteIcon != null)
            bgmMuteIcon.GetComponent<Button>().onClick.AddListener(ToggleBGMMute);

        // 효과음 아이콘 클릭 이벤트 추가
        if (sfxSoundIcon != null)
            sfxSoundIcon.GetComponent<Button>().onClick.AddListener(ToggleSFXMute);
        if (sfxMuteIcon != null)
            sfxMuteIcon.GetComponent<Button>().onClick.AddListener(ToggleSFXMute);

        // 초기 아이콘 상태 설정
        UpdateSoundIcon(bgmSlider.value, true);
        UpdateSoundIcon(sfxSlider.value, false);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUIForCurrentScene();
    }

    // BGM 음소거 토글
    public void ToggleBGMMute()
    {
        if (!isBgmMuted)
        {
            isBgmMuted = true;
            bgmSlider.value = 0f;
        }
        else
        {
            isBgmMuted = false;
            bgmSlider.value = 0.3f;
        }
        SetBGMLevel(bgmSlider.value);
    }

    // 효과음 음소거 토글
    public void ToggleSFXMute()
    {
        if (!isSfxMuted)
        {
            isSfxMuted = true;
            sfxSlider.value = 0f;
        }
        else
        {
            isSfxMuted = false;
            sfxSlider.value = 0.3f;
        }
        SetSFXLevel(sfxSlider.value);
    }

    // 사운드 아이콘 업데이트 (isBGM: true면 BGM, false면 효과음)
    private void UpdateSoundIcon(float volume, bool isBGM)
    {
        if (isBGM)
        {
            if (bgmSoundIcon != null && bgmMuteIcon != null)
            {
                bgmSoundIcon.gameObject.SetActive(volume > 0.0001);
                bgmMuteIcon.gameObject.SetActive(volume <= 0.0001);
            }
        }
        else
        {
            if (sfxSoundIcon != null && sfxMuteIcon != null)
            {
                sfxSoundIcon.gameObject.SetActive(volume > 0.0001);
                sfxMuteIcon.gameObject.SetActive(volume <= 0.0001);
            }
        }
    }

    void UpdateUIForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        //if (currentSceneName == "Level_1")
        //{
        //    // BGM UI 비활성화
        //    bgmSlider.interactable = false;
        //    bgmVolumeText.text = "음소거";
        //    if (bgmSoundIcon != null) bgmSoundIcon.gameObject.SetActive(false);
        //    if (bgmMuteIcon != null) bgmMuteIcon.gameObject.SetActive(false);

        //    // 효과음 UI 비활성화
        //    sfxSlider.interactable = false;
        //    sfxVolumeText.text = "음소거";
        //    if (sfxSoundIcon != null) sfxSoundIcon.gameObject.SetActive(false);
        //    if (sfxMuteIcon != null) sfxMuteIcon.gameObject.SetActive(false);
        //}
        //else
        {
            bgmSlider.interactable = true;
            sfxSlider.interactable = true;
            SetBGMLevel(bgmSlider.value);
            SetSFXLevel(sfxSlider.value);
        }
    }

    public void SetBGMLevel(float sliderVal)
    {
        
       
            mixer.SetFloat("BGM", Mathf.Log10(sliderVal) * 20);
            int volumePercent = Mathf.RoundToInt(sliderVal * 100);
            bgmVolumeText.text = volumePercent.ToString();
            UpdateSoundIcon(sliderVal, true);
       
    }

    public void SetSFXLevel(float sliderVal)
    {
       
        
            mixer.SetFloat("Other", Mathf.Log10(sliderVal) * 20);
            int volumePercent = Mathf.RoundToInt(sliderVal * 100);
            sfxVolumeText.text = volumePercent.ToString();
            UpdateSoundIcon(sliderVal, false);
        
    }
}