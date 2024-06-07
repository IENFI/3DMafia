// SceneInitializer.cs
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using System.Collections;
using TMPro;

public class SceneInitializer : MonoBehaviourPunCallbacks
{
    public TMP_Text coinTextPrefab; // CoinText 프리팹을 여기서 설정합니다.

    public override void OnEnable()
    {
        base.OnEnable();
        // 씬이 로드될 때 이벤트를 등록합니다.
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == CustomEventCodes.GameSceneLoaded) // 커스텀 이벤트 코드 사용
        {
            StartCoroutine(InitializeSceneWithDelay());
        }
    }

    private IEnumerator InitializeSceneWithDelay()
    {
        yield return new WaitForSeconds(1); // 필요한 시간만큼 대기
        InitializeScene();
    }

    private void InitializeScene()
    {
        // 로그 추가로 객체 찾기 확인
        Debug.Log("Initializing Scene...");

        // 모든 플레이어의 프리팹을 찾습니다.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            Debug.Log("Player Found: " + player.name); // 플레이어 객체 확인

            PlayerCoinController playerCoinController = player.GetComponent<PlayerCoinController>();
            if (playerCoinController != null)
            {
                // CoinText 프리팹을 할당합니다.
                // 이전에는 코인 UI를 Canvas의 자식으로 설정했지만, 이제는 원하는 위치에 설정합니다.
                Transform coinTextLocation = GameObject.FindWithTag("CoinText").transform;
                playerCoinController.coinText.transform.SetParent(coinTextLocation, false);

                Debug.Log("PlayerCoinController properties assigned for player: " + player.name);
            }
            else
            {
                Debug.Log("PlayerCoinController not found on player: " + player.name);
            }
        }

        // 타이머 초기화 및 실행
        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.StartTimer(); // 타이머 시작
            Debug.Log("Timer started.");
        }
        else
        {
            Debug.LogWarning("Timer script not found!");
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();
        // 씬이 언로드될 때 이벤트를 해제합니다.
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
}
