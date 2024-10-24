using UnityEngine;
using TMPro;
using System.Collections;

public class TextLoadingAnimation : MonoBehaviour
{
    public TMP_Text loadingText;
    public float dotInterval = 0.5f; // 점이 추가되는 간격 (초)
    private string baseText = "로딩중";
    private int maxDots = 3;
    private Coroutine animationCoroutine;

    void Awake()
    {
        if (loadingText == null)
        {
            loadingText = GetComponent<TMP_Text>();
            if (loadingText == null)
            {
                Debug.LogError("TMP_Text 컴포넌트를 찾을 수 없습니다!");
            }
        }
        loadingText.text = baseText;
    }

    void Start()
    {
        // Start에서 애니메이션을 시작하지 않습니다. 외부에서 호출하도록 변경
    }

    private void OnDisable()
    {
        StopAnimation();
    }

    public IEnumerator AnimateForDuration(float duration)
    {
        StartAnimation();
        yield return new WaitForSeconds(duration);
        StopAnimation();
    }

    public void StartAnimation()
    {
        if (loadingText == null)
        {
            Debug.LogError("loadingText가 null입니다. 애니메이션을 시작할 수 없습니다.");
            return;
        }
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateLoadingText());
        Debug.Log("애니메이션 시작됨");
    }

    public void StopAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        if (loadingText != null)
        {
            loadingText.text = baseText;
            Debug.Log("애니메이션 중지됨, 텍스트 리셋: " + loadingText.text);
        }
    }

    private IEnumerator AnimateLoadingText()
    {
        int dotCount = 1;
        while (true)
        {
            string newText = baseText + new string('.', dotCount);
            loadingText.text = newText;
            Debug.Log("텍스트 업데이트: " + newText);
            dotCount++;
            if (dotCount > maxDots)
            {
                dotCount = 1;
            }
            yield return new WaitForSeconds(dotInterval);
        }
    }
}