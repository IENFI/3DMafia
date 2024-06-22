using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Photon.Voice.Unity;
using Photon.Voice;

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
        PhotonNetwork.AutomaticallySyncScene = true;
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
        if (PhotonNetwork.IsConnectedAndReady)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "isReady", isReady }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        else
        {
            Debug.LogWarning("Photon client is not connected and ready. Cannot set custom properties.");
        }
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
        if (PhotonNetwork.IsMasterClient && gameReady)
        {
            if (PhotonNetwork.CurrentRoom != null)
            {
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
                { "isGameStarted", true }
            };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
            else
            {
                Debug.LogError("PhotonNetwork.CurrentRoom is null.");
                return;
            }

            

            PhotonNetwork.LoadLevel("Level_1");
            Debug.Log("Level_1 입장 완료");
        }
    }


    public void LeaveRoom()
    {


        // 모든 플레이어의 준비 상태를 초기화
        ResetAllPlayerReadyStates();

        // 방 떠나기
        PhotonNetwork.LeaveRoom();
    }

    void ResetAllPlayerReadyStates()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "isReady", false }
        };

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            player.SetCustomProperties(props);
        }
    }

    public override void OnLeftRoom()
    {
        // 방을 떠나면서 이전 씬으로 전환
        PhotonNetwork.LoadLevel("ServerScene");
    }
}
