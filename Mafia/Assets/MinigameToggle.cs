using UnityEngine;
using UnityEngine.UI;

public class MinigameToggle : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject toggleUI;  // ToggleUI ������Ʈ
    [SerializeField] private GameObject listUI;    // ListUI ������Ʈ

    [Header("Toggle Buttons")]
    [SerializeField] private Button toggleButton;  // ToggleUI�� ��� ��ư
    [SerializeField] private Button listButton;    // ListUI�� ��� ��ư

    private void Awake()
    {
        // �ʱ� ���� ���� (������ �� ToggleUI�� Ȱ��ȭ)
        if (toggleUI != null && listUI != null)
        {
            toggleUI.SetActive(true);
            listUI.SetActive(false);
        }

        // ��ư�� ��� �Լ� ����
        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleUI);
        if (listButton != null)
            listButton.onClick.AddListener(ToggleUI);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleUI();
        }
    }

    public void ToggleUI()
    {
        if (toggleUI != null && listUI != null)
        {
            // ���� ���¸� �ݴ�� ��ȯ
            toggleUI.SetActive(!toggleUI.activeSelf);
            listUI.SetActive(!listUI.activeSelf);
        }
    }

    private void OnDestroy()
    {
        // ��ư ������ ����
        if (toggleButton != null)
            toggleButton.onClick.RemoveListener(ToggleUI);
        if (listButton != null)
            listButton.onClick.RemoveListener(ToggleUI);
    }
}