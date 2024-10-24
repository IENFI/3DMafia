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
        // 씬 로드 이벤트에 리스너 추가
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 현재 씬이 Server_Scene인 경우 페이드 아웃 시작
        if (SceneManager.GetActiveScene().name == "Server_Scene")
        {
            StartFadeOut();
        }
    }

    private void OnDestroy()
    {
        // 스크립트가 파괴될 때 이벤트 리스너 제거
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Server_Scene이 로드될 때마다 페이드 아웃 시작
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
        // 초기 상태 설정
        fadeCanvasGroup.alpha = 1f;

        // 초기 대기 시간
        yield return new WaitForSeconds(initialDelay);

        // 페이드 아웃
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        // 완전히 투명하게 설정
        fadeCanvasGroup.alpha = 0f;
    }
}