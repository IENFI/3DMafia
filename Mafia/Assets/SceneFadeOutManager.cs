using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFadeOutManager : MonoBehaviour
{
    public CanvasGroup fadeCanvasGroup;
    public float initialDelay = 2f;
    public float fadeDuration = 2f;

    private void Start()
    {
        // �� �ε� �̺�Ʈ�� ������ �߰�
        SceneManager.sceneLoaded += OnSceneLoaded;

        // ���� ���� Server_Scene�� ��� ���̵� �ƿ� ����
        if (SceneManager.GetActiveScene().name == "Server_Scene")
        {
            StartFadeOut();
        }
    }

    private void OnDestroy()
    {
        // ��ũ��Ʈ�� �ı��� �� �̺�Ʈ ������ ����
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Server_Scene�� �ε�� ������ ���̵� �ƿ� ����
        if (scene.name == "Server_Scene")
        {
            StartFadeOut();
        }
    }

    private void StartFadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        // �ʱ� ���� ����
        fadeCanvasGroup.alpha = 1f;

        // �ʱ� ��� �ð�
        yield return new WaitForSeconds(initialDelay);

        // ���̵� �ƿ�
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        // ������ �����ϰ� ����
        fadeCanvasGroup.alpha = 0f;
    }
}