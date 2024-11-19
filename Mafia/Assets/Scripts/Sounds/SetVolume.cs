using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;
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
    private float previousBgmVolume = 0.3f; // 이전 BGM 볼륨 저장

    [Header("효과음")]
    public Slider sfxSlider;
    public TMP_Text sfxVolumeText;
    public Image sfxSoundIcon;
    public Image sfxMuteIcon;
    private bool isSfxMuted = false;
    private float previousSfxVolume = 0.3f; // 이전 SFX 볼륨 저장

    // PlayerPrefs 인터페이스
    private IPlayerPrefs prefs;

    [Header("설정 저장")]

    private const string BGM_VOLUME_KEY = "BGM_VOLUME";
    private const string SFX_VOLUME_KEY = "SFX_VOLUME";
    private const string BGM_MUTED_KEY = "BGM_MUTED";
    private const string SFX_MUTED_KEY = "SFX_MUTED";

#region 사운드 설정

    private void Awake()
    {
        #if TEST_ENVIRONMENT
            prefs = new MockPlayerPrefs(); // 테스트 환경에서는 Mock 사용
        #else
            prefs = new RealPlayerPrefs(); // 실제 환경에서는 PlayerPrefs 사용
        #endif
    }
    private void Start()
    {
        // 사운드 설정 로드
        LoadSettings();
        
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
        // 사운드 설정 저장
        SaveSettings();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUIForCurrentScene();
    }

    // 설정 저장
    private void SaveSettings()
    {
        prefs.SetFloat(BGM_VOLUME_KEY, bgmSlider.value);
        prefs.SetFloat(SFX_VOLUME_KEY, sfxSlider.value);
        prefs.SetFloat(BGM_MUTED_KEY, isBgmMuted ? 1f : 0f); // 음소거 상태 저장
        prefs.SetFloat(SFX_MUTED_KEY, isSfxMuted ? 1f : 0f); // 음소거 상태 저장
        prefs.SetFloat("PREVIOUS_BGM_VOLUME", previousBgmVolume); // 이전 BGM 볼륨 저장
        prefs.SetFloat("PREVIOUS_SFX_VOLUME", previousSfxVolume); // 이전 SFX 볼륨 저장
        prefs.Save();
        Debug.Log($"[SaveSettings] BGM: {bgmSlider.value}, SFX: {sfxSlider.value}, BGM Muted: {isBgmMuted}, SFX Muted: {isSfxMuted}, Previous BGM: {previousBgmVolume}, Previous SFX: {previousSfxVolume}");
    }

    private void LoadSettings()
    {
        if (prefs.HasKey(BGM_VOLUME_KEY))
        {
            bgmSlider.value = prefs.GetFloat(BGM_VOLUME_KEY);
        }
        else
        {
            #if TEST_ENVIRONMENT
            bgmSlider.value = 0f; // 기본값
            #else
            bgmSlider.value = 0.5f; // 기본값
            #endif
        }

        if (prefs.HasKey(SFX_VOLUME_KEY))
        {
            sfxSlider.value = prefs.GetFloat(SFX_VOLUME_KEY);
        }
        else
        {
            sfxSlider.value = 0.5f; // 기본값
        }

        // 음소거 상태 복원
        isBgmMuted = prefs.HasKey(BGM_MUTED_KEY) && prefs.GetFloat(BGM_MUTED_KEY) == 1f;
        isSfxMuted = prefs.HasKey(SFX_MUTED_KEY) && prefs.GetFloat(SFX_MUTED_KEY) == 1f;

        previousBgmVolume = prefs.HasKey("PREVIOUS_BGM_VOLUME") ? prefs.GetFloat("PREVIOUS_BGM_VOLUME") : 0.3f;
        previousSfxVolume = prefs.HasKey("PREVIOUS_SFX_VOLUME") ? prefs.GetFloat("PREVIOUS_SFX_VOLUME") : 0.3f;

        Debug.Log($"[LoadSettings] BGM: {bgmSlider.value}, SFX: {sfxSlider.value}, BGM Muted: {isBgmMuted}, SFX Muted: {isSfxMuted}, Previous BGM: {previousBgmVolume}, Previous SFX: {previousSfxVolume}");

        // 초기 사운드 값 적용
        SetBGMLevel(bgmSlider.value);
        SetSFXLevel(sfxSlider.value);

        // 음소거 상태 적용
        if (isBgmMuted)
        {
            bgmSlider.value = 0f;
            UpdateSoundIcon(0f, true);
        }

        if (isSfxMuted)
        {
            sfxSlider.value = 0f;
            UpdateSoundIcon(0f, false);
        }
    }

    // BGM 음소거 토글
    public void ToggleBGMMute()
    {
        if (!isBgmMuted)
        {
            isBgmMuted = true;
            // 슬라이더 값이 0이 아닐 때만 이전 값을 저장
            if (bgmSlider.value > 0.0001f)
            {
                previousBgmVolume = bgmSlider.value;
                prefs.SetFloat("PREVIOUS_BGM_VOLUME", previousBgmVolume);
            }
            bgmSlider.value = 0f;
        }
        else
        {
            isBgmMuted = false;
            // 이전 값이 0이라면 기본값(예: 0.3f)으로 설정
            bgmSlider.value = prefs.HasKey("PREVIOUS_BGM_VOLUME") && (prefs.GetFloat("PREVIOUS_BGM_VOLUME") > 0f) ? prefs.GetFloat("PREVIOUS_BGM_VOLUME") : 0.3f;
        }
        SetBGMLevel(bgmSlider.value);
    }

    // 효과음 음소거 토글
    public void ToggleSFXMute()
    {
        // 슬라이더를 서서히 0으로 줄이면 오류 존재함
        // 로직을 0보다 크면 이전 값으로 돌리는 것이 아니라, 1보다 작으면 음소거를 하는 로직으로 고치는게 나아보임
        if (!isSfxMuted)
        {
            isSfxMuted = true;
            // 슬라이더 값이 0이 아닐 때만 이전 값을 저장
            if (sfxSlider.value > 0.0001f)
            {
                previousSfxVolume = sfxSlider.value;
                prefs.SetFloat("PREVIOUS_SFX_VOLUME", previousSfxVolume);

            }
            sfxSlider.value = 0f;
        }
        else
        {
            isSfxMuted = false;
            // 이전 값이 0이라면 기본값(예: 0.3f)으로 설정
            sfxSlider.value = prefs.HasKey("PREVIOUS_SFX_VOLUME") && (prefs.GetFloat("PREVIOUS_SFX_VOLUME") > 0f )? prefs.GetFloat("PREVIOUS_SFX_VOLUME") : 0.3f;
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

#endregion

    #region 테스트 환경을 위한 인터페이스
    public interface IPlayerPrefs
    {
        void SetFloat(string key, float value);
        float GetFloat(string key, float defaultValue = 0f);
        bool HasKey(string key);
        void DeleteKey(string key);
        void Save();
    }

    public class RealPlayerPrefs : IPlayerPrefs
    {
        public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
        public float GetFloat(string key, float defaultValue = 0f) => PlayerPrefs.GetFloat(key, defaultValue);
        public bool HasKey(string key) => PlayerPrefs.HasKey(key);
        public void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);
        public void Save() => PlayerPrefs.Save();
    }


    public class MockPlayerPrefs : IPlayerPrefs
    {
        private static Dictionary<string, float> storage = new Dictionary<string, float>();

        public void SetFloat(string key, float value) => storage[key] = value;
        public float GetFloat(string key, float defaultValue = 0f) =>
            storage.ContainsKey(key) ? storage[key] : defaultValue;
        public bool HasKey(string key) => storage.ContainsKey(key);
        public void DeleteKey(string key) => storage.Remove(key);
        public void Save() { /* No action needed for mock */ }
    }
    #endregion

}