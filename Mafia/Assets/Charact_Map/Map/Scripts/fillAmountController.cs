using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class fillAmountController : MonoBehaviour
{
    public float totalTime;
    private float fillAmount = 0; // 0부터 시작
    private Image myImage;

    private void Awake()
    {
        myImage = GetComponent<Image>();
    }

    void Update()
    {
        if (fillAmount < 1)
        {
            fillAmount += Time.deltaTime / totalTime;
            // 최대값이 1이 되도록 제한합니다.
            fillAmount = Mathf.Clamp01(fillAmount);
            myImage.fillAmount = fillAmount;
        }
    }

    // fillAmount를 초기화하는 메서드
    public void ResetFillAmount()
    {
        fillAmount = 0;
        myImage.fillAmount = fillAmount;
    }
}
