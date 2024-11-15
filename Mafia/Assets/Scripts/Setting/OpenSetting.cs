using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class OpenSetting : MonoBehaviour
{
    public GameObject settingsPanel; // 설정 패널 (비활성화 상태로 시작)
    AudioSource audioSource;

    void Start()
    {
        // settingsPanel.SetActive(false);
        audioSource = GetComponent<AudioSource>();

        // 설정 패널을 UI 관리 목록에 등록
        // if (settingsPanel != null)
        // {
        //     GameManager.instance.RegisterUIWindow(settingsPanel);
        // }
    }

    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        // 현재 씬이 Level_0 또는 Level_1이고 ESC 키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.Escape) &&
            (currentScene.name == "Level_0" || currentScene.name == "Level_1"))
        {
            Debug.Log("설정 창 열기 시도");
            // 설정 패널이 열려있지 않은 상태에서 다른 UI가 열려있다면 무시
            if (!settingsPanel.activeSelf && GameManager.instance.IsAnyUIOpen())
            {
                return;
            }

            ToggleSettingsPanel();
        }
    }
    
    void OnDestroy()
    {
        // 설정 패널을 UI 관리 목록에서 제거
        // if (settingsPanel != null)
        // {
        //     GameManager.instance.UnregisterUIWindow(settingsPanel);
        // }
    }

    public void ToggleSettingsPanel()
    {
        Debug.Log("OpenSetting.cs : ToggleSettingsPanel.activeSelf : " + settingsPanel.activeSelf);

        // 패널의 활성화 상태를 토글
        if (settingsPanel.activeSelf ){
            settingsPanel.SetActive(false);
        }
        else if ( !GameManager.instance.IsAnyUIOpen()) {
            Debug.Log("settingsPanel.SetActive(true)");
            settingsPanel.SetActive(true);
        }
        // settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void onClick()
    {
        audioSource.Play();
    }
}
