using UnityEngine;

public class ToggleUI : MonoBehaviour
{
    [SerializeField] private GameObject uiObject; // UI 오브젝트를 인스펙터에서 할당

    void Start()
    {
        // 시작할 때 UI가 보이지 않게 설정 (선택사항)
        if (uiObject != null)
        {
            uiObject.SetActive(false);
        }
    }

    // UI를 켜고 끄는 함수
    public void ToggleUIVisibility()
    {
        if (uiObject != null)
        {
            uiObject.SetActive(!uiObject.activeSelf);
        }
    }
}