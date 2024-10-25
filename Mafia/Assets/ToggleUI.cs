using UnityEngine;

public class ToggleUI : MonoBehaviour
{
    [SerializeField] private GameObject uiObject; // UI ������Ʈ�� �ν����Ϳ��� �Ҵ�

    void Start()
    {
        // ������ �� UI�� ������ �ʰ� ���� (���û���)
        if (uiObject != null)
        {
            uiObject.SetActive(false);
        }
    }

    // UI�� �Ѱ� ���� �Լ�
    public void ToggleUIVisibility()
    {
        if (uiObject != null)
        {
            uiObject.SetActive(!uiObject.activeSelf);
        }
    }
}