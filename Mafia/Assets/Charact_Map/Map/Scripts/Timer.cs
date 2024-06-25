using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviourPun
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float time = 300f;  // 타이머 기본 시간
    private float curTime;  // 현재 남은 시간

    private Light directionalLight;
    private Coroutine blinkCoroutine;
    private Coroutine timerCoroutine;  // 타이머 코루틴 참조 변수
    private bool isBlinking;
    private fillAmountController fillController;
    public GameObject ShopUI; // 상점 UI

    private int Start_count = -1;
    private int Pause_count = -1;

    private bool isDaytime = true; // 낮/밤 상태를 추적하는 변수

    [SerializeField] private List<GameObject> Merchants; // 상인 GameObject 리스트

    private void Awake()
    {
        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
        fillController = FindObjectOfType<fillAmountController>(); // fillAmountController 찾기
        Debug.Log("Timer Awake: directionalLight and fillController initialized.");
    }

    [PunRPC]
    public void RPC_StartTimer()
    {
        if (Start_count == -1)
        {
            Start_count--;
            StartTimer();
        }
        else
        {
            return;
        }
    }

    [PunRPC]
    public void RPC_PauseTimer()
    {
        if (Pause_count == -1)
        {
            Pause_count--;
            PauseTimer();
        }
        else
        {
            return;
        }
    }


    public void StartTimer()
    {
        Start_count = -1;
        curTime = time+5;
        Debug.Log("타이머시간" + curTime);
        // 낮/밤 상태 전환
        isDaytime = !isDaytime;
        Debug.Log("낮밤" + isDaytime);
        // 자연광 전환
        directionalLight.enabled = !directionalLight.enabled;
        Debug.Log("자연광" + directionalLight);
        // 밤으로 전환될 때만 상인 활성화
        if (!isDaytime)
        {
            ActivateRandomMerchants();
        }
        else
        {
            DeactivateAllMerchants();


        }
        timerCoroutine = StartCoroutine(TimerCoroutine());  // 타이머 코루틴 시작
    }
    public void StartTimer2()
    {
        curTime = time;
        timerCoroutine = StartCoroutine(TimerCoroutine());  // 타이머 코루틴 시작
    }


    public void PauseTimer()
    {
        Pause_count = -1;
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);  // 타이머 코루틴 중지
            timerCoroutine = null;
        }

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);  // 반짝임 코루틴 중지
            text.color = Color.black;  // 텍스트 색상 초기화
        }

        Debug.Log("타이머가 멈췄습니다.");
    }

    public void RestartTimer()
    {
        PauseTimer();  // 타이머 중지
        curTime = time;  // 타이머 초기화
        /*photonView.RPC("RPC_StartTimer", RpcTarget.All);*/
    }

    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            GameObject playerObject = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            PhotonView playerPhotonView = playerObject.GetComponent<PhotonView>();

            curTime = time + 5;
            while (playerPhotonView == null)
            {
                // 1초 기다림
                yield return new WaitForSeconds(1);
            }


            playerPhotonView.RPC("DisableAllCorpses", RpcTarget.All);

            isBlinking = false;

            if (fillController != null)
            {
                fillController.ResetFillAmount(); // 각 사이클 시작 시 fill amount 초기화
            }

            text.color = Color.black;  // 텍스트 색상 초기화

            while (curTime > 0)
            {
                curTime -= Time.deltaTime;
                int minute = (int)curTime / 60;
                int second = (int)curTime % 60;
                text.text = minute.ToString("00") + ":" + second.ToString("00");

                if (curTime <= 10f && !isBlinking)  // 10초 이하로 남았을 때
                {
                    text.color = Color.red;
                    blinkCoroutine = StartCoroutine(BlinkText());
                    isBlinking = true;
                }

                yield return null;
            }

            // 타이머가 종료되었을 때
            Debug.Log("시간 종료");
            curTime = 0;
            // 자연광 전환
            directionalLight.enabled = !directionalLight.enabled;

            // 낮/밤 상태 전환
            isDaytime = !isDaytime;

            // 밤으로 전환될 때만 상인 활성화
            if (!isDaytime)
            {
                ActivateRandomMerchants();
            }
            else
            {
                DeactivateAllMerchants();

            }

            // 반짝임 코루틴 멈춤
            if (blinkCoroutine != null)
            {
                StopCoroutine(blinkCoroutine);
                text.color = Color.black;  // 텍스트 색상 초기화
            }

            // fillAmount 초기화
            if (fillController != null)
            {
                fillController.ResetFillAmount();
            }

            // 다음 사이클을 위해 잠시 대기
            yield return new WaitForSeconds(1f);  // 전환 후 1초 대기 (선택 사항)
        }
    }

    private void ActivateRandomMerchants()
    {
        // 모든 상인 비활성화
        foreach (var merchant in Merchants)
        {
            merchant.SetActive(false);
        }

        // 상인 중 2명을 무작위로 선택하여 활성화
        List<int> selectedIndices = new List<int>();
        while (selectedIndices.Count < 2)
        {
            int randomIndex = Random.Range(0, Merchants.Count);
            if (!selectedIndices.Contains(randomIndex))
            {
                selectedIndices.Add(randomIndex);
                Merchants[randomIndex].SetActive(true);
            }
        }

    }

    private void DeactivateAllMerchants()
    {
        foreach (var merchant in Merchants)
        {
            merchant.SetActive(false);
        }
        ShopUI.SetActive(false);
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            text.enabled = !text.enabled;  // 텍스트의 활성화 상태를 토글
            yield return new WaitForSeconds(0.5f);  // 반짝임 속도 조절
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // 필요한 경우 Start에서 초기화할 수 있습니다.
    }

    // Update is called once per frame
    void Update()
    {

    }
}