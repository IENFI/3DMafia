using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWrite : MonoBehaviour
{
    [SerializeField]
    private GameObject letterUI; // 활성화하고 싶은 UI

    void Update()
    {
        // L키를 눌렀을 때 UI를 활성화
        if (Input.GetKeyDown(KeyCode.L))
        {
            ToggleUI();
        }
    }

    void ToggleUI()
    {
        // UI가 활성화되어 있으면 비활성화하고, 비활성화되어 있으면 활성화
        bool isActive = letterUI.activeSelf;
        letterUI.SetActive(!isActive); // 현재 상태의 반대값으로 설정
    }
}
