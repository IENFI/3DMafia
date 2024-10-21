using UnityEngine;
using UnityEngine.UI;

public class PanelExpander : MonoBehaviour
{
    public GameObject detailsPanel; // 세부 내용을 담고 있는 패널
    public Button toggleButton;      // 버튼 (화살표)
    public RectTransform panelRect;  // 패널의 RectTransform
    public float expandedHeight = 300f; // 확장된 높이
    public float collapsedHeight = 100f; // 축소된 높이
    private bool isExpanded = false;

    void Start()
    {
        // 버튼 클릭 이벤트 연결
        toggleButton.onClick.AddListener(TogglePanel);
        // 초기 상태: 세부 내용 비활성화 및 패널 높이 설정
        ClosePanel();
    }

    void TogglePanel()
    {
        isExpanded = !isExpanded;
        if (isExpanded)
        {
            OpenPanel();
        }
        else
        {
            ClosePanel();
        }
    }

    void OpenPanel()
    {
        detailsPanel.SetActive(true); // 세부 내용 활성화
        panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, expandedHeight); // 패널 높이 증가
    }

    void ClosePanel()
    {
        detailsPanel.SetActive(false); // 세부 내용 비활성화
        panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, collapsedHeight); // 패널 높이 감소
        isExpanded = false; // 상태 업데이트
    }

    void OnDisable()
    {
        // UI가 비활성화될 때 (즉, 닫힐 때) 호출됩니다.
        ClosePanel();
    }
}