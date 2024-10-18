using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class WriteLetter : MinigameBase
{
    public TextMeshProUGUI[] letters;
    private bool[] letterCompleted;

    [SerializeField]
    private GameObject minigameManager;

    [SerializeField]
    private MinigameInteraction minigame;
    private int index = 3;
    private bool active = false;

    private int currentLetterIndex = 0; // 현재 칠할 글자의 인덱스

    void Start()
    {
        letterCompleted = new bool[letters.Length];
    }

    void Update()
    {
        if (!active) return;

        // 마우스 위치 가져오기
        Vector3 mousePosition = Input.mousePosition;

        // 현재 순서에 맞는 글자만 색칠할 수 있게 처리
        if (!letterCompleted[currentLetterIndex])
        {
            RectTransform rectTransform = letters[currentLetterIndex].GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition))
            {
                // 마우스가 글자 위에 있으면 색을 검은색으로 바꾼다
                letters[currentLetterIndex].color = Color.black;
                letterCompleted[currentLetterIndex] = true;

                // 다음 글자로 넘어감
                currentLetterIndex++;
            }
        }

        // 모든 글자가 검은색으로 되었는지 체크
        if (AllLettersCompleted())
        {
            CloseUI(); // UI를 닫는 함수 호출
        }
    }

    bool AllLettersCompleted()
    {
        return currentLetterIndex >= letters.Length;
    }

    public override void ReceiveToken()
    {
        Debug.Log("편지 쓰기 미니게임이 시작되었습니다.");
        active = true;
    }

    public override void Deactivation()
    {
        active = false;
    }

    public override bool GetActive()
    {
        return active;
    }

    public override MinigameManager GetMinigameManager()
    {
        return minigameManager.GetComponent<MinigameManager>();
    }

    void CloseUI()
    {
        minigame.ExitCode = true;
        active = false;
        GetMinigameManager().SuccessMission(index);
        this.gameObject.SetActive(false);
    }
}
