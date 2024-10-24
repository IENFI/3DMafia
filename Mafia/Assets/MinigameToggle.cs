using UnityEngine;
using UnityEngine.UI;

public class MinigameToggle : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] private GameObject toggleUI;  // ToggleUI 오브젝트
    [SerializeField] private GameObject listUI;    // ListUI 오브젝트

    [Header("Toggle Buttons")]
    [SerializeField] private Button toggleButton;  // ToggleUI의 토글 버튼
    [SerializeField] private Button listButton;    // ListUI의 토글 버튼

    private void Awake()
    {
        // 초기 상태 설정 (시작할 때 ToggleUI만 활성화)
        if (toggleUI != null && listUI != null)
        {
            toggleUI.SetActive(true);
            listUI.SetActive(false);
        }

        // 버튼에 토글 함수 연결
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
            // 현재 상태를 반대로 전환
            toggleUI.SetActive(!toggleUI.activeSelf);
            listUI.SetActive(!listUI.activeSelf);
        }
    }

    private void OnDestroy()
    {
        // 버튼 리스너 제거
        if (toggleButton != null)
            toggleButton.onClick.RemoveListener(ToggleUI);
        if (listButton != null)
            listButton.onClick.RemoveListener(ToggleUI);
    }
}