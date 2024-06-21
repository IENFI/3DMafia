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

        StartCoroutine(InitialState());
    }

    public IEnumerator InitialState()
    {
        yield return new WaitForSeconds(2);
        if (PhotonNetwork.IsMasterClient)
        {
            isReady = true;
            SetReadyState(isReady);
            ReadyBtn.GetComponent<Button>().interactable = false;
        }
        else
        {
            SetReadyState(isReady);
            StartBtn.GetComponent<Button>().interactable = false;
        }
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
            readyBtnText.text = "Unready";
        }
        else
        {
            readyBtnText.text = "Ready";
        }

        if (gameReady)
        {
            startBtnText.text = "Game Start";
        }
        else
        {
            startBtnText.text = "Waiting for Players";
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
}

public class CustomEventCodes
{
    public const byte GameSceneLoaded = 1; // 사용자 정의 이벤트 코드
}