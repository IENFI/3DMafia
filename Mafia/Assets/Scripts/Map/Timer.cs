using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviourPun
{
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float cycleDuration = 300f;
    [SerializeField] private List<GameObject> merchants;
    [SerializeField] private Image nightImage;
    [SerializeField] private Image dayImage;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private CanvasGroup phaseChangeUI;
    [SerializeField] private TMP_Text phaseChangeText;

    private float currentTime;
    private bool isDaytime = true;
    private bool isBlinking;
    private int cycleCount = -1;

    private Light directionalLight;
    private fillAmountController fillController;
    private Coroutine blinkCoroutine;
    private Coroutine timerCoroutine;

    private void Awake()
    {
        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
        fillController = FindObjectOfType<fillAmountController>();
        fillController.totalTime = cycleDuration;
        
    }

    [PunRPC]
    public void PauseTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
    }

    [PunRPC]
    public void ResumeTimer()
    {
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    public void StartTimer()
    {
        UpdateDayNightState();
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        while (true)
        {
            InitializeNewCycle();
            yield return RunTimerCycle();
            yield return StartCoroutine(ShowPhaseChangeUI());

           // yield return new WaitForSeconds(1f);
        }
    }

    private void InitializeNewCycle()
    {
        if (cycleCount == -1) 
        { 
            currentTime = cycleDuration;
            GameObject playerObject = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            PhotonView playerPhotonView = playerObject.GetComponent<PhotonView>();
            playerPhotonView.RPC("Spawn", PhotonNetwork.LocalPlayer);
            playerPhotonView.RPC("DisableAllCorpses", RpcTarget.All);
            fillController.totalTime = currentTime;
            fillController.ResetFillAmount();
        }
        cycleCount++;

        isBlinking = false;
        timerText.color = Color.black;
    }

    private IEnumerator RunTimerCycle()
    {
        fillController.totalTime = currentTime;
        fillController.ResetFillAmount(); // 여기로 이동합니다
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();
            CheckBlinkingCondition();
            yield return null;
        }
    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void CheckBlinkingCondition()
    {
        if (currentTime <= 10f && !isBlinking)
        {
            timerText.color = Color.red;
            blinkCoroutine = StartCoroutine(BlinkText());
            isBlinking = true;
        }
    }

    private void EndCycle()
    {
        currentTime = cycleDuration;  // 여기서 타이머를 초기화합니다
        directionalLight.enabled = !directionalLight.enabled;
        isDaytime = !isDaytime;
        UpdateDayNightState();
        GameObject playerObject = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        PhotonView playerPhotonView = playerObject.GetComponent<PhotonView>();
        playerPhotonView.RPC("Spawn", PhotonNetwork.LocalPlayer);
        playerPhotonView.RPC("DisableAllCorpses", RpcTarget.All);

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            timerText.color = Color.black;
        }

        UpdateTimerDisplay();  // 타이머 표시를 즉시 업데이트합니다
    }

    private void UpdateDayNightState()
    {
        if (isDaytime)
        {
            DeactivateAllMerchants();
            nightImage.enabled = false;
            dayImage.enabled = true;
        }
        else
        {
            ActivateRandomMerchants();
            nightImage.enabled = true;
            dayImage.enabled = false;
        }
    }

    private void ActivateRandomMerchants()
    {
        DeactivateAllMerchants();
        List<int> selectedIndices = new List<int>();
        while (selectedIndices.Count < 2)
        {
            int randomIndex = Random.Range(0, merchants.Count);
            if (!selectedIndices.Contains(randomIndex))
            {
                selectedIndices.Add(randomIndex);
                merchants[randomIndex].SetActive(true);
            }
        }
    }

    private void DeactivateAllMerchants()
    {
        foreach (var merchant in merchants)
        {
            merchant.SetActive(false);
        }
        shopUI.SetActive(false);
    }

    private IEnumerator BlinkText()
    {
        while (true)
        {
            timerText.enabled = !timerText.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public IEnumerator ShowPhaseChangeUI()
    {
        CanvasGroup uiToActivate1 = null;
        EndCycle();
        phaseChangeText.text = isDaytime ? "밤이 되었습니다" : "낮이 되었습니다";
        phaseChangeText.color = isDaytime ? Color.red : Color.blue;
        phaseChangeUI.gameObject.SetActive(true);
        uiToActivate1 = phaseChangeUI;
        if(uiToActivate1  != null) { 
            
            yield return StartCoroutine(FadeCanvasGroup(phaseChangeUI, 0, 1, 3f));
            Debug.Log("페이드인 호출완료");
            yield return new WaitForSeconds(2f);
            Debug.Log("2초대기 호출완료");
            yield return StartCoroutine(FadeCanvasGroup(phaseChangeUI, 1, 0, 2f));
            Debug.Log("페이드아웃 호출완료");
            uiToActivate1.gameObject.SetActive(false);
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
        Debug.Log("빠져나왔음");
    }
}