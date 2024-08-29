using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class SceneInitializer : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text coinTextPrefab;
    [SerializeField] private GameObject loadingImage;
    [SerializeField] private CanvasGroup citizenUI;
    [SerializeField] private CanvasGroup mafiaUI;

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == CustomEventCodes.GameSceneLoaded)
        {
            StartCoroutine(InitializeSceneWithDelay());
        }
    }

    private IEnumerator InitializeSceneWithDelay()
    {
        yield return new WaitForSeconds(1);
        InitializeScene();
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeInRoleUI());
    }

    private void InitializeScene()
    {
        Debug.Log("Initializing Scene...");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log($"Number of players found: {players.Length}");

        foreach (GameObject player in players)
        {
            Debug.Log($"Player Found: {player.name}");
            InitializePlayerComponents(player);
        }

        InitializeTimer();
    }

    private void InitializePlayerComponents(GameObject player)
    {
        InitializePlayerCoinController(player);
        InitializePlayerAnimator(player);
    }

    private void InitializePlayerCoinController(GameObject player)
    {
        PlayerCoinController playerCoinController = player.GetComponent<PlayerCoinController>();
        if (playerCoinController != null)
        {
            Debug.Log($"PlayerCoinController found on player: {player.name}");
            if (playerCoinController.coinText == null)
            {
                Transform coinTextLocation = GameObject.FindWithTag("CoinText")?.transform;
                if (coinTextLocation != null)
                {
                    playerCoinController.coinTextPrefab = coinTextPrefab;
                    playerCoinController.coinText = Instantiate(coinTextPrefab, coinTextLocation);
                    playerCoinController.coinText.transform.SetParent(coinTextLocation, false);
                    Debug.Log($"PlayerCoinController properties assigned for player: {player.name}");
                }
                else
                {
                    Debug.LogWarning("CoinText location not found!");
                }
            }
        }
        else
        {
            Debug.LogWarning($"PlayerCoinController not found on player: {player.name}");
        }
    }


    private void InitializePlayerAnimator(GameObject player)
    {
        Transform builder = player.transform.Find("Avatar");
        if (builder != null)
        {
            PlayerAnimator playerAnimator = builder.GetComponent<PlayerAnimator>();
            if (playerAnimator != null)
            {
                GameObject killTimeObject = GameObject.FindWithTag("KillTime");
                if (killTimeObject != null)
                {
                    KillTimer killTimer = killTimeObject.GetComponent<KillTimer>();
                    if (killTimer != null)
                    {
                        playerAnimator.killTimer = killTimer;
                        Debug.Log($"KillTimer assigned for player: {player.name}");
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
                Debug.LogWarning($"PlayerAnimator not found on Builder for player: {player.name}");
            }
        }
        else
        {
            Debug.LogWarning($"Builder not found for player: {player.name}");
        }
    }

    private void InitializeTimer()
    {
        Timer timer = FindObjectOfType<Timer>();
        if (timer == null)
        {
            Debug.LogWarning("Timer script not found!");
        }
    }

    private IEnumerator FadeInRoleUI()
    {
        CanvasGroup uiToActivate = (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMafia"] ? mafiaUI : citizenUI;
        uiToActivate.gameObject.SetActive(true);

        Timer timer = FindObjectOfType<Timer>();
        if (timer != null)
        {
            timer.StartTimer();
            Debug.Log("Timer started.");
        }

        yield return StartCoroutine(FadeCanvasGroup(uiToActivate, 0, 1, 2));
        loadingImage.SetActive(false);

        yield return new WaitForSeconds(2);

        yield return StartCoroutine(FadeCanvasGroup(uiToActivate, 1, 0, 1));
        uiToActivate.gameObject.SetActive(false);
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
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
}