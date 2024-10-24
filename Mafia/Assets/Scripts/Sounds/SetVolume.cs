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

    // ���� �߰��� ������
    public Image soundIcon;    // Sound �̹���
    public Image muteIcon;     // Mute �̹���
    private bool isMuted = false;

    private void Start()
    {
        // �����̴��� ���� ����� ������ SetLevel �޼��带 ȣ���մϴ�.
        slider.onValueChanged.AddListener(SetLevel);

        // �� �ε� �̺�Ʈ�� ���� �����ʸ� �߰��մϴ�.
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ���� ���� ���� �ʱ� ������ �����մϴ�.
        UpdateUIForCurrentScene();

        // �̹��� Ŭ�� �̺�Ʈ �߰�
        if (soundIcon != null)
            soundIcon.GetComponent<Button>().onClick.AddListener(ToggleMute);
        if (muteIcon != null)
            muteIcon.GetComponent<Button>().onClick.AddListener(ToggleMute);

        // �ʱ� �̹��� ���� ����
        UpdateSoundIcon(slider.value);
    }

    private void OnDestroy()
    {
        // ��ũ��Ʈ�� �ı��� �� �̺�Ʈ �����ʸ� �����մϴ�.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUIForCurrentScene();
    }

    // ���� �߰��ϴ� �޼���: ���Ұ� ���
    public void ToggleMute()
    {
        if (!isMuted)
        {
            // ���Ұŷ� ��ȯ
            isMuted = true;
            slider.value = 0f;
        }
        else
        {
            // ���Ұ� ���� (���� 30���� ����)
            isMuted = false;
            slider.value = 0.3f;
        }
        SetLevel(slider.value);
    }

    // ���� �߰��ϴ� �޼���: ������ ���� ������Ʈ
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
            volumeText.text = "����";
            // Level_1������ �����ܵ� ��Ȱ��ȭ
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

            // ������ ���� ������Ʈ
            UpdateSoundIcon(sliderVal);
        }
    }
}