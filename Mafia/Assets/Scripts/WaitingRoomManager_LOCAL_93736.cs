using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public Button ReadyBtn;
    public Button StartBtn;

    public GameObject LoadingUI; // 로딩 화면 UI
    private CanvasGroup loadingCanvasGroup; // 로딩 화면의 CanvasGroup

    private bool isReady = false;
    static bool gameReady = false;

    public TMP_Text readyBtnText;
    public TMP_Text startBtnText;

    void Awake()
    {
        // 방장이 혼자 씬을 로딩하면, 나머지 사람들은 자동으로 싱크가 됨
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
    }

    void Start()
    {
        readyBtnText = ReadyBtn.GetComponentInChildren<TMP_Text>();
        startBtnText = StartBtn.GetComponentInChildren<TMP_Text>();

        loadingCanvasGroup = LoadingUI.GetComponent<CanvasGroup>();

        StartCoroutine(InitialState());
    }

    public IEnumerator InitialState()
    {
        // 로딩 UI를 활성화
        LoadingUI.SetActive(true);

        yield return new WaitForSeconds(4); // 4초 대기

        if (PhotonNetwork.IsMasterClient)
        {
            isReady = true;
            SetReadyState(isReady);

            ReadyBtn.GetComponent<Button>().interactable = false;
            // 레디 버튼 숨기기
            ReadyBtn.gameObject.SetActive(false);
        }
        else
        {
            SetReadyState(isReady);
            StartBtn.GetComponent<Button>().interactable = false;
            // 시작 버튼 숨기기
            StartBtn.gameObject.SetActive(false);
        }

        // 로딩 UI를 페이드 아웃
        yield return StartCoroutine(FadeOutLoadingUI());
    }

    void Update()
    {
        ReadAllReadyStates();

        if (PhotonNetwork.IsMasterClient)
        {
            StartBtn.GetComponent<Button>().interactable = gameReady;
        }

        // 버튼 텍스트 변경
        if (isReady)
        {
            readyBtnText.text = "준비완료";
            ReadyBtn.GetComponent<Image>().color = Color.green;
        }
        else
        {
            readyBtnText.text = "준비하기";
            ReadyBtn.GetComponent<Image>().color = Color.white;
        }

        if (gameReady)
        {
            startBtnText.text = "게임 시작";
        }
        else
        {
            startBtnText.text = "모두 준비해야 합니다";
        }
    }

    void SetReadyState(bool isReady)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "isReady", isReady }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void ClickReadyBtn()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            isReady = !isReady;
            SetReadyState(isReady);
        }
    }

    void ReadAllReadyStates()
    {
        int count = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("isReady")) continue;

            if ((bool)player.CustomProperties["isReady"] == false)
            {
                count++;
            }
        }
        if (count == 0)
        {
            gameReady = true;
        }
        else
        {
            gameReady = false;
        }
    }

    public void GameStart()
    {
        if(PhotonNetwork.IsMasterClient && gameReady)
        {
            PhotonNetwork.LoadLevel("Level_1");
            Debug.Log("Level_1 입장 완료");

            // 커스텀 이벤트 전송
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(CustomEventCodes.GameSceneLoaded, null, options, SendOptions.SendReliable);
        }
    }

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnLeftRoom()
    {
        // 서버 씬으로 전환
        PhotonNetwork.LoadLevel("ServerScene");
    }
    private IEnumerator FadeOutLoadingUI()
    {
        float duration = 0.5f; // 페이드 아웃 지속 시간
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            loadingCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / duration);
            yield return null;
        }

        loadingCanvasGroup.alpha = 0f;
        LoadingUI.SetActive(false);
    }
}

public class CustomEventCodes
{
    public const byte GameSceneLoaded = 1; // 사용자 정의 이벤트 코드
}