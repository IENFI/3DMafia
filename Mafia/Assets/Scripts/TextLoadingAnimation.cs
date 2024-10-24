using UnityEngine;
using TMPro;
using System.Collections;

public class TextLoadingAnimation : MonoBehaviour
{
    public TMP_Text loadingText;
    public float dotInterval = 0.5f; // ���� �߰��Ǵ� ���� (��)
    private string baseText = "�ε���";
    private int maxDots = 3;
    private Coroutine animationCoroutine;

    void Awake()
    {
        if (loadingText == null)
        {
            loadingText = GetComponent<TMP_Text>();
            if (loadingText == null)
            {
                Debug.LogError("TMP_Text ������Ʈ�� ã�� �� �����ϴ�!");
            }
        }
        loadingText.text = baseText;
    }

    void Start()
    {
        // Start���� �ִϸ��̼��� �������� �ʽ��ϴ�. �ܺο��� ȣ���ϵ��� ����
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
            Debug.LogError("loadingText�� null�Դϴ�. �ִϸ��̼��� ������ �� �����ϴ�.");
            return;
        }
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
        animationCoroutine = StartCoroutine(AnimateLoadingText());
        Debug.Log("�ִϸ��̼� ���۵�");
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
            Debug.Log("�ִϸ��̼� ������, �ؽ�Ʈ ����: " + loadingText.text);
        }
    }

    private IEnumerator AnimateLoadingText()
    {
        int dotCount = 1;
        while (true)
        {
            string newText = baseText + new string('.', dotCount);
            loadingText.text = newText;
            Debug.Log("�ؽ�Ʈ ������Ʈ: " + newText);
            dotCount++;
            if (dotCount > maxDots)
            {
                dotCount = 1;
            }
            yield return new WaitForSeconds(dotInterval);
        }
    }
}