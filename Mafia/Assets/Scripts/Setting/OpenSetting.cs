using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSetting : MonoBehaviour
{
    public GameObject settingsPanel; // 설정 패널 (비활성화 상태로 시작)

    void Update()
    {
        // ESC 키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsPanel();
        }
    }

    void ToggleSettingsPanel()
    {
        // 패널의 활성화 상태를 토글
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}
