using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeController : MonoBehaviour, IEndDragHandler
{
    [SerializeField] int maxPage;
    int currentPage;
    Vector3 targetPos;
    [SerializeField] Vector3 pageStep;
    [SerializeField] RectTransform levelPagesRect;
    [SerializeField] float tweenTime;
    [SerializeField] LeanTweenType tweenType;
    float dragThreshould;
    [SerializeField] Image[] barImage;
    [SerializeField] Sprite barClosed, barOpen;
    [SerializeField] Button previousBtn, nextBtn;

    public void Awake()
    {
        currentPage = 1;
        targetPos = levelPagesRect.localPosition;
        dragThreshould = Screen.width / 15;
        SetupBarButtons(); // 바 이미지에 버튼 기능 추가
        UpdateBar();
        UpdateArrowButton();
    }

    // 바 이미지에 버튼 기능을 추가하는 메서드
    private void SetupBarButtons()
    {
        for (int i = 0; i < barImage.Length; i++)
        {
            int pageIndex = i + 1; // 현재 바의 페이지 번호 (1부터 시작)
            Button button = barImage[i].gameObject.GetComponent<Button>();

            // 버튼 컴포넌트가 없다면 추가
            if (button == null)
            {
                button = barImage[i].gameObject.AddComponent<Button>();
            }

            // 클릭 이벤트 설정
            button.onClick.AddListener(() => MoveToPage(pageIndex));
        }
    }

    // 특정 페이지로 이동하는 메서드
    public void MoveToPage(int pageNumber)
    {
        if (pageNumber < 1 || pageNumber > maxPage || pageNumber == currentPage)
            return;

        // 현재 위치에서 목표 페이지까지의 이동량 계산
        int pagesDiff = pageNumber - currentPage;
        targetPos += pageStep * pagesDiff;
        currentPage = pageNumber;

        MovePage();
    }

    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            targetPos += pageStep;
            MovePage();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.x - eventData.pressPosition.x) > dragThreshould)
        {
            if (eventData.position.x > eventData.pressPosition.x) Previous();
            else Next();
        }
        else
        {
            MovePage();
        }
    }

    void UpdateBar()
    {
        foreach (var item in barImage)
        {
            item.sprite = barClosed;
        }
        barImage[currentPage - 1].sprite = barOpen;
    }

    public void Previous()
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPos -= pageStep;
            MovePage();
        }
    }

    void MovePage()
    {
        levelPagesRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
        UpdateBar();
        UpdateArrowButton();
    }

    void UpdateArrowButton()
    {
        nextBtn.interactable = true;
        previousBtn.interactable = true;
        if (currentPage == 1) previousBtn.interactable = false;
        else if (currentPage == maxPage) nextBtn.interactable = false;
    }
}