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
        // �����̴��� ���� ����� ������ SetLevel �޼��带 ȣ���մϴ�.
        slider.onValueChanged.AddListener(SetLevel);

        // �� �ε� �̺�Ʈ�� ���� �����ʸ� �߰��մϴ�.
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ���� ���� ���� �ʱ� ������ �����մϴ�.
        UpdateUIForCurrentScene();
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

    void UpdateUIForCurrentScene()
    {
        // ���� ���� �̸��� �����ɴϴ�.
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "Level_1")
        {
            // Level_1 �������� �����̴��� ��Ȱ��ȭ�ϰ� �ؽ�Ʈ�� "����"���� �����մϴ�.
            slider.interactable = false;
            volumeText.text = "����";
        }
        else
        {
            // �ٸ� �������� �����̴��� Ȱ��ȭ�ϰ� ���� ���� ���� ǥ���մϴ�.
            slider.interactable = true;
            SetLevel(slider.value);
        }
    }

    public void SetLevel(float sliderVal)
    {
        // ���� ���� Level_1�� �ƴ� ���� ������ �����մϴ�.
        if (SceneManager.GetActiveScene().name != "Level_1")
        {
            // ����� �ͼ��� ������ �����մϴ�.
            mixer.SetFloat("BGM", Mathf.Log10(sliderVal) * 20);
            // �����̴� ���� 0-100 ������ ��ȯ�Ͽ� �ؽ�Ʈ�� ǥ���մϴ�.
            int volumePercent = Mathf.RoundToInt(sliderVal * 100);
            volumeText.text = volumePercent.ToString();
        }
    }
}