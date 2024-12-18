using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class OpenSetting : MonoBehaviour
{
    public GameObject settingsPanel; // 설정 패널 (비활성화 상태로 시작)
    AudioSource audioSource;
    [SerializeField]
    private GameObject createRoomPanel;

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
            // 설정 패널이 열려있지 않은 상태에서 다른 UI가 열려있다면 무시
            if (!settingsPanel.activeSelf && GameManager.instance.IsAnyUIOpen())
            {
                return;
            }

            ToggleSettingsPanel(currentScene.name);
        }       
        
        if (Input.GetKeyDown(KeyCode.Escape) &&
            (currentScene.name == "ServerScene"))
        {
            ToggleSettingsPanel(currentScene.name);
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

    public void ToggleSettingsPanel(string scene)
    {
        if (scene == "ServerScene")
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
            return;
        }

        if (settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
        }
        else if (!GameManager.instance.IsAnyUIOpen())
        {
            settingsPanel.SetActive(true);
            if (!PhotonNetwork.IsMasterClient && scene == "Level_0")
            {
                createRoomPanel.SetActive(false);
            }
        }
    }

    public void onClick()
    {
        audioSource.Play();
    }
}
