using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class SceneInitializer : MonoBehaviourPunCallbacks
{
    public TMP_Text coinTextPrefab; // CoinText 프리팹을 여기서 설정합니다.
    /*public Image DeadImagePrefab;*/
    public GameObject LoadingImage; // 로딩 이미지를 설정합니다.
    public CanvasGroup CitizenUI; // CitizenUI의 CanvasGroup을 설정합니다.
    public CanvasGroup MafiaUI; // MafiaUI의 CanvasGroup을 설정합니다.

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
        yield return new WaitForSeconds(1); // 역할 배정 후 잠시 대기
        StartCoroutine(FadeInRoleUI());
    }

    private void InitializeScene()
    {
        // 로그 추가로 객체 찾기 확인
        // Debug.Log("Initializing Scene...");

        // 모든 플레이어의 프리팹을 찾습니다.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("Number of players found: " + players.Length);

        foreach (GameObject player in players)
        {
            Debug.Log("Player Found: " + player.name); // 플레이어 객체 확인

            /*PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                if (playerController.DeadImage == null)
                {
                    Transform DeadImageLocation = GameObject.FindWithTag("DeadImage")?.transform;
                    if (DeadImageLocation != null)
                    {
                        playerController.DeadImagePrefab = DeadImagePrefab;
                        playerController.DeadImage = Instantiate(DeadImagePrefab, DeadImageLocation);
                        playerController.DeadImage.transform.SetParent(DeadImageLocation, false);
                        Debug.Log("PlayerController properties assigned for player: " + player.name);
                    }
                    else
                    {
                        Debug.LogWarning("DeadImage location not found!");
                    }
                }
            }*/

            PlayerCoinController playerCoinController = player.GetComponent<PlayerCoinController>();
            if (playerCoinController != null)
            {
                Debug.Log("PlayerCoinController found on player: " + player.name); // PlayerCoinController 확인

                // CoinText 프리팹을 할당합니다.
                if (playerCoinController.coinText == null)
                {
                    Transform coinTextLocation = GameObject.FindWithTag("CoinText")?.transform;
                    if (coinTextLocation != null)
                    {
                        playerCoinController.coinTextPrefab = coinTextPrefab;
                        playerCoinController.coinText = Instantiate(coinTextPrefab, coinTextLocation);
                        playerCoinController.coinText.transform.SetParent(coinTextLocation, false);
                        Debug.Log("PlayerCoinController properties assigned for player: " + player.name);
                    }
                    else
                    {
                        Debug.LogWarning("CoinText location not found!");
                    }
                }
            }
            else
            {
                Debug.LogWarning("PlayerCoinController not found on player: " + player.name);
            }

            // Builder 하위 오브젝트에 있는 PlayerAnimator 스크립트를 찾습니다.
            Transform builder = player.transform.Find("Avatar");
            if (builder != null)
            {
                PlayerAnimator playerAnimator = builder.GetComponent<PlayerAnimator>();
                if (playerAnimator != null)
                {
                    // KillTime 태그가 부착된 오브젝트를 찾습니다.
                    GameObject killTimeObject = GameObject.FindWithTag("KillTime");
                    if (killTimeObject != null)
                    {
                        KillTimer killTimer = killTimeObject.GetComponent<KillTimer>();
                        if (killTimer != null)
                        {
                            playerAnimator.killTimer = killTimer;
                            Debug.Log("KillTimer assigned for player: " + player.name);
                        }
                        else
                        {
                            Debug.LogWarning("KillTimer component not found on KillTime object!");
                        }
                    }
                    else
                    {
                        Debug.LogWarning("KillTime object not found!");
                    }
                }
                else
                {
                    Debug.LogWarning("PlayerAnimator not found on Builder for player: " + player.name);
                }
            }
            else
            {
                Debug.LogWarning("Builder not found for player: " + player.name);
            }
        }

        // 타이머 초기화
        Timer timer = FindObjectOfType<Timer>();
        if (timer == null)
        {
            Debug.LogWarning("Timer script not found!");
        }
    }

    private IEnumerator FadeInRoleUI()
    {
        CanvasGroup uiToActivate = null;

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isMafia"])
        {
            MafiaUI.gameObject.SetActive(true);
            uiToActivate = MafiaUI;
        }
        else
        {
            CitizenUI.gameObject.SetActive(true);
            uiToActivate = CitizenUI;
        }

        if (uiToActivate != null)
        {
            // 타이머 시작
            Timer timer = FindObjectOfType<Timer>();
            if (timer != null)
            {
                timer.StartTimer();
                Debug.Log("Timer started.");
            }
            yield return StartCoroutine(FadeCanvasGroup(uiToActivate, 0, 1, 2)); // 페이드 인을 2초 동안 수행
            LoadingImage.SetActive(false); // LoadingImage 비활성화

            yield return new WaitForSeconds(2); // 2초 대기

            yield return StartCoroutine(FadeCanvasGroup(uiToActivate, 1, 0, 1)); // 페이드 아웃을 1초 동안 수행
            uiToActivate.gameObject.SetActive(false);

        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = end;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        // 씬이 언로드될 때 이벤트를 해제합니다.
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

}
