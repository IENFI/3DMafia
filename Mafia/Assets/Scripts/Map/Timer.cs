using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Timer Settings")]
    [SerializeField] private TMP_Text text;
    [SerializeField] private float time = 300f;
    private float curTime;
    private float syncInterval = 1f;
    private float nextSyncTime = 0f;
    private double startTime;

    [Header("UI References")]
    [SerializeField] private Image Night;
    [SerializeField] private Image Day;
    public GameObject ShopUI;

    [Header("Scene References")]
    [SerializeField] private List<GameObject> Merchants;
    [SerializeField] private GameObject reportObject;
    public Material daySkybox;
    public Material nightSkybox;

    [Header("Components")]
    private Light directionalLight;
    private Light reportLight;
    private fillAmountController fillController;
    public MinigameManager minigameManager;

    // Private variables
    private Coroutine blinkCoroutine;
    private Coroutine timerCoroutine;
    private bool isBlinking;
    private bool isPaused = false;
    private int startCount = -1;
    private bool isDaytime = true;
    private PhotonView photonView;

    private string ghostTag = "Ghost";
    private GameObject[] GhostList;
    private Dictionary<GameObject, Light> ghostLights = new Dictionary<GameObject, Light>();

    private void Awake()
    {
        // Component 캐싱
        photonView = GetComponent<PhotonView>();
        if (!photonView.ObservedComponents.Contains(this))
        {
            photonView.ObservedComponents.Add(this);
        }

        directionalLight = GameObject.Find("Directional Light")?.GetComponent<Light>();
        if (directionalLight == null) Debug.LogError("Directional Light not found!");

        reportLight = reportObject?.GetComponent<Light>();
        if (reportLight == null) Debug.LogError("Report Light not found!");

        fillController = FindObjectOfType<fillAmountController>();
        if (fillController != null)
        {
            fillController.totalTime = time;
        }
        else Debug.LogError("fillAmountController not found!");

        minigameManager = FindObjectOfType<MinigameManager>();
        if (minigameManager == null)
        {
            Debug.LogError("MinigameManager not found in scene!");
        }

        // 초기 설정
        InitializeUI();
        CacheGhostLights();
    }

    private void Start()
    {
        // 모든 컴포넌트가 준비될 때까지 약간의 딜레이를 줍니다
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        // 플레이어 초기화를 위해 잠시 대기
        yield return new WaitForSeconds(3f);

        if (PhotonNetwork.IsMasterClient)
        {
            StartTimer();
        }
    }

    private void InitializeUI()
    {
        if (text != null)
        {
            text.enabled = true;
            text.color = Color.black;
        }
        if (Night != null) Night.enabled = false;
        if (Day != null) Day.enabled = true;
    }

    private void CacheGhostLights()
    {
        ghostLights.Clear();
        GhostList = GameObject.FindGameObjectsWithTag(ghostTag);
        foreach (GameObject ghost in GhostList)
        {
            Transform lightTransform = ghost.transform.Find("FPCamera/PointLight");
            if (lightTransform != null)
            {
                Light pointLight = lightTransform.GetComponent<Light>();
                if (pointLight != null)
                {
                    ghostLights[ghost] = pointLight;
                }
            }
        }
    }

    public void StartTimer()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        photonView.RPC("InitializeAndStartTimer", RpcTarget.All);
    }

    [PunRPC]
    private void InitializeAndStartTimer()
    {
        startTime = PhotonNetwork.Time;
        curTime = time;
        isDaytime = true;
        isBlinking = false;

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        // 초기화는 여기서 한 번만
        if (text != null)
        {
            text.enabled = true;
            text.color = Color.black;
        }

        if (fillController != null)
        {
            fillController.totalTime = time;
            fillController.ResetFillAmount();
        }

        // 최초 한 번만 실행되어야 하는 로직
        if (PhotonNetwork.LocalPlayer?.TagObject != null)
        {
            SyncPlayerState();
        }

        if (minigameManager != null)
        {
            minigameManager.AssignRandomMinigame();
        }

        UpdatePhaseVisuals();
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    [PunRPC]
    private void SyncTimer(double networkTime, bool isDayTime, float currentTime)
    {
        startTime = networkTime;
        this.isDaytime = isDayTime;
        this.curTime = currentTime;

    }

    private void UpdatePhaseVisuals()
    {
        if (!isDaytime)
        {
            ActivateRandomMerchants();
            Night.enabled = true;
            Day.enabled = false;
        }
        else
        {
            DeactivateAllMerchants();
            Night.enabled = false;
            Day.enabled = true;
        }
    }

    // IPunObservable 인터페이스 구현
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 마스터 클라이언트에서 다른 클라이언트로 데이터 전송
            stream.SendNext(curTime);
            stream.SendNext(isDaytime);
            stream.SendNext(startTime);
        }
        else
        {
            // 다른 클라이언트에서 데이터 수신
            curTime = (float)stream.ReceiveNext();
            isDaytime = (bool)stream.ReceiveNext();
            startTime = (double)stream.ReceiveNext();
        }
    }

    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            // 최초 한 번만 초기화
            if (PhotonNetwork.LocalPlayer?.TagObject != null)
            {
                InitializeTimerCycle();
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            while (curTime > 0)
            {
                if (isPaused)
                {
                    yield return null;
                    continue;
                }

                if (PhotonNetwork.IsMasterClient)
                {
                    curTime -= Time.deltaTime;
                    if (Time.time >= nextSyncTime)
                    {
                        photonView.RPC("SyncTimer", RpcTarget.All, PhotonNetwork.Time, isDaytime, Mathf.Max(0, curTime));
                        nextSyncTime = Time.time + syncInterval;
                    }
                }

                UpdateTimer();
                yield return null;
            }

            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("HandleTimerEndRPC", RpcTarget.All);
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private void InitializeTimerCycle()
    {
        if (startCount == -1)
        {
            curTime = time + 5;
            startCount++;
        }
        else
        {
            curTime = time;
        }

        if (fillController != null)
        {
            fillController.totalTime = curTime;
            fillController.ResetFillAmount();
        }

        if (text != null)
        {
            text.enabled = true;
            text.color = Color.black;
        }

        SyncPlayerState();
    }

    private void UpdateTimer()
    {
        if (text != null)
        {
            int minute = (int)curTime / 60;
            int second = (int)curTime % 60;
            text.text = $"{minute:00}:{second:00}";

            if (curTime <= 10f && !isBlinking)
            {
                text.color = Color.red;
                blinkCoroutine = StartCoroutine(BlinkText());
                isBlinking = true;
            }
        }
    }
    [PunRPC]
    private void HandleTimerEndRPC()
    {
        curTime = 0;
        isDaytime = !isDaytime;
        isBlinking = false;

        if (minigameManager != null)
        {
            minigameManager.AssignRandomMinigame();
        }

        UpdateEnvironment();
        StopBlinking();

        if (fillController != null)
        {
            fillController.ResetFillAmount();
        }
    }

    private void UpdateEnvironment()
    {
        ToggleAllLights(isDaytime);
        ChangeSkybox(isDaytime);
        SetFarValue(isDaytime);
        SwitchOnFewLights(isDaytime);
        UpdatePhaseVisuals();
    }

    private void SyncPlayerState()
    {
        if (PhotonNetwork.LocalPlayer?.TagObject is GameObject playerObject)
        {
            PhotonView playerPhotonView = playerObject.GetComponent<PhotonView>();
            if (playerPhotonView != null)
            {
                playerPhotonView.RPC("Spawn", PhotonNetwork.LocalPlayer);
                playerPhotonView.RPC("DisableAllCorpses", RpcTarget.All);
                // PlayerReportRadius 컴포넌트 찾아서 DestoryCorpse 호출
                PlayerReportRadius reportRadius = playerObject.GetComponent<PlayerReportRadius>();
                if (reportRadius != null)
                {
                    reportRadius.DestoryCorpse();
                }
            }
        }
    }

    private void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            if (text != null)
            {
                text.enabled = true;
                text.color = Color.black;
            }
        }
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            if (text != null)
            {
                text.enabled = !text.enabled;
                yield return new WaitForSeconds(0.5f);
            }
            else yield break;
        }
    }

    [PunRPC]
    public void PauseTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
        isPaused = true;
    }

    [PunRPC]
    public void ForceRestartTimer()
    {
        StopAllCoroutines();
        isPaused = false;

        if (text != null)
        {
            text.enabled = true;
            text.color = Color.black;
        }

        if (directionalLight != null)
        {
            directionalLight.enabled = !directionalLight.enabled;
        }

        isDaytime = !isDaytime;


        if (minigameManager != null)
        {
            minigameManager.AssignRandomMinigame();
        }

        UpdateEnvironment();
        StopBlinking();

        if (fillController != null)
        {
            fillController.ResetFillAmount();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncTimer", RpcTarget.All, PhotonNetwork.Time, isDaytime, time);
        }

        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private void ActivateRandomMerchants()
    {
        if (Merchants == null || Merchants.Count == 0) return;

        foreach (var merchant in Merchants)
        {
            if (merchant != null) merchant.SetActive(false);
        }

        List<int> selectedIndices = new List<int>();
        while (selectedIndices.Count < Mathf.Min(2, Merchants.Count))
        {
            int randomIndex = Random.Range(0, Merchants.Count);
            if (!selectedIndices.Contains(randomIndex))
            {
                selectedIndices.Add(randomIndex);
                if (Merchants[randomIndex] != null)
                {
                    Merchants[randomIndex].SetActive(true);
                }
            }
        }
    }

    private void DeactivateAllMerchants()
    {
        if (Merchants != null)
        {
            foreach (var merchant in Merchants)
            {
                if (merchant != null) merchant.SetActive(false);
            }
        }

        if (ShopUI != null)
        {
            ShopUI.SetActive(false);
        }
    }

    public void ToggleAllLights(bool state)
    {
        if (state)
        {
            Light[] allLights = FindObjectsOfType<Light>();
            GameObject[] PlayerList = GameObject.FindGameObjectsWithTag("Player");
            List<Light> FlashLightList = new List<Light>();
            foreach (GameObject player in PlayerList)
            {
                Transform FPCamera = player.transform.Find("FPCamera");
                Transform FlashLight = FPCamera.transform.Find("FlashLight");
                Light flashLight = FlashLight.GetComponent<Light>();
                FlashLightList.Add(flashLight);
            }
            foreach (Light light in allLights)
            {
                if (!FlashLightList.Contains(light)) light.enabled = state;
            }
        }
        else
        {
            Light[] allLights = FindObjectsOfType<Light>();
            foreach (Light light in allLights)
            {
                light.enabled = state;
            }
        }
    }

    public void ChangeSkybox(bool state)
    {
        RenderSettings.skybox = state ? daySkybox : nightSkybox;
    }

    public void SetFarValue(bool state)
    {
        if (Camera.main != null)
        {
            Camera.main.farClipPlane = state ? 1000f : 1000f;
        }
    }

    public void SwitchOnFewLights(bool state)
    {
        if (state)
        {
            reportLight.enabled = true;
        }
        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"])
        {
            ghostTag = "Ghost";
            GhostList = GameObject.FindGameObjectsWithTag(ghostTag);

            foreach (GameObject ghost in GhostList)
            {
                Transform FPCamera = ghost.transform.Find("FPCamera");
                Transform PointLight = FPCamera.transform.Find("PointLight");
                Light pointLight = PointLight.GetComponent<Light>();
                pointLight.enabled = true;
            }
        }
        else
        {
            GameObject Player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            Transform FPCamera = Player.transform.Find("FPCamera");
            Transform PointLight = FPCamera.transform.Find("PointLight");
            Light pointLight = PointLight.GetComponent<Light>();
            pointLight.enabled = !state;
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncTimer", RpcTarget.All, PhotonNetwork.Time, isDaytime);
        }
    }
}