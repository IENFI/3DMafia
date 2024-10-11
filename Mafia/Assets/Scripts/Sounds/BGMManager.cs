using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public AudioClip Spooky1; // Spooky 1 오디오 클립
    private AudioSource audioSource;

    void Awake()
    {
        // 씬 전환시에도 오브젝트가 삭제되지 않도록 설정
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;  // 반복 재생 활성화
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름이 ServerScene 또는 Level_0일 경우 Spooky 1을 재생
        if (scene.name == "ServerScene" || scene.name == "Level_0")
        {
            PlayMusic(Spooky1);
        }
        else
        {
            StopMusic();
        }
    }

    void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    void StopMusic()
    {
        audioSource.Stop();
    }
}
