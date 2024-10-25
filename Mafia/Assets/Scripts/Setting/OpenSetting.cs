using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class OpenSetting : MonoBehaviour
{
    public GameObject settingsPanel; // 설정 패널 (비활성화 상태로 시작)
    
    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        // ESC 키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.Escape) && currentScene.name == "Level_0")
        {
            ToggleSettingsPanel();
        }
    }

    void ToggleSettingsPanel()
    {
        // 패널의 활성화 상태를 토글
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }

    public void TogglePanel()
    {
        // 패널이 현재 활성화되어 있는지 확인한 후, 반대로 설정
        bool isActive = settingsPanel.activeSelf;
        settingsPanel.SetActive(!isActive);
    }
}
