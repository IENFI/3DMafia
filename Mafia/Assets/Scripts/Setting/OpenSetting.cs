using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class OpenSetting : MonoBehaviour
{
    public GameObject settingsPanel; // 설정 패널 (비활성화 상태로 시작)
    AudioSource audioSource;
    // Start is called before the first frame update

    // Update is called once per frame

    void Start()
    {
        settingsPanel.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }
    
    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        // ESC 키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.Escape) && currentScene.name == "Level_0")
        {
            ToggleSettingsPanel();
        }
    }

    public void ToggleSettingsPanel()
    {
        Debug.Log("OpenSetting.cs : ToggleSettingsPanel.activeSelf : " + settingsPanel.activeSelf);
        // 패널의 활성화 상태를 토글
        if (settingsPanel.activeSelf){
            settingsPanel.SetActive(false);
        }
        else {
            settingsPanel.SetActive(true);
        }
    }
    public void onClick()
    {


        audioSource.Play();

    }
}
