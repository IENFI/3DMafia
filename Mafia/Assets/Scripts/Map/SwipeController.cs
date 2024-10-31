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
        SetupBarButtons(); // �� �̹����� ��ư ��� �߰�
        UpdateBar();
        UpdateArrowButton();
    }

    // �� �̹����� ��ư ����� �߰��ϴ� �޼���
    private void SetupBarButtons()
    {
        for (int i = 0; i < barImage.Length; i++)
        {
            int pageIndex = i + 1; // ���� ���� ������ ��ȣ (1���� ����)
            Button button = barImage[i].gameObject.GetComponent<Button>();

            // ��ư ������Ʈ�� ���ٸ� �߰�
            if (button == null)
            {
                button = barImage[i].gameObject.AddComponent<Button>();
            }

            // Ŭ�� �̺�Ʈ ����
            button.onClick.AddListener(() => MoveToPage(pageIndex));
        }
    }

    // Ư�� �������� �̵��ϴ� �޼���
    public void MoveToPage(int pageNumber)
    {
        if (pageNumber < 1 || pageNumber > maxPage || pageNumber == currentPage)
            return;

        // ���� ��ġ���� ��ǥ ������������ �̵��� ���
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