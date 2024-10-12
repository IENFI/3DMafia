using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Book : MonoBehaviour
{
    public int Number { get; private set; }
    public bool IsPlaced { get; set; }
    private Vector2 initialPosition;

    private TextMeshProUGUI numberText;

    public void Initialize(int number, Vector2 position)
    {
        Number = number;
        IsPlaced = false;
        initialPosition = position;

        GetComponent<Image>().color = Random.ColorHSV();

        numberText = GetComponentInChildren<TextMeshProUGUI>();

        if (numberText != null)
        {
            numberText.text = number.ToString();
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI component is missing in children of Book object!");
        }

        ResetToInitialPosition();  // 초기 위치로 설정
    }

    public void UpdateInitialPosition(Vector2 newPosition)
    {
        initialPosition = newPosition;
    }

    public void ResetPosition(Vector2 position)
    {
        initialPosition = position;
        ResetToInitialPosition();
    }

    public void ResetToInitialPosition()  // 새로 추가된 메서드
    {
        GetComponent<RectTransform>().anchoredPosition = initialPosition;
    }
}