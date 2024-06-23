using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Photon.Voice.Unity;

public class WaitingRoomManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance = null;
    public Button ReadyBtn;
    public Button StartBtn;

    private bool isReady = false;
    static bool gameReady = false;

    public TMP_Text readyBtnText;
    public TMP_Text startBtnText;

    private static VoiceConnection voiceConnection;
    private static bool voiceConnectionInitialized = false;

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
        InitializeVoiceConnection();
        StartCoroutine(InitialState());
    }
    private void InitializeVoiceConnection()
    {
        // 모든 VoiceConnection 인스턴스 가져오기
        var voiceConnections = FindObjectsOfType<VoiceConnection>();

        // 여러 인스턴스가 있을 경우 하나만 유지하고 나머지는 제거
        if (voiceConnections.Length > 1)
        {
            Debug.LogWarning("Multiple VoiceConnection instances found. Keeping only one and destroying others.");
            for (int i = 1; i < voiceConnections.Length; i++)
            {
                Destroy(voiceConnections[i].gameObject);
            }
        }

        // VoiceConnection 인스턴스가 있는 경우 재사용
        if (voiceConnections.Length > 0)
        {
            voiceConnection = voiceConnections[0];
            if (!voiceConnection.gameObject.scene.IsValid())
            {
                DontDestroyOnLoad(voiceConnection.gameObject);
            }
            Debug.Log("Using existing VoiceConnection instance.");
        }
        else if (!voiceConnectionInitialized)
        {
            // VoiceConnection 인스턴스가 없는 경우 새로 생성
            GameObject voiceConnectionObject = new GameObject("VoiceConnection");
            voiceConnection = voiceConnectionObject.AddComponent<VoiceConnection>();
            DontDestroyOnLoad(voiceConnectionObject);
            voiceConnectionInitialized = true;
            Debug.Log("VoiceConnection component created and added to the scene.");
        }
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
        GameManager.instance = null;
        if (voiceConnection != null)
        {
            // 음성 채팅 연결 해제
            if (voiceConnection.Client.InRoom)
            {
                voiceConnection.Client.OpLeaveRoom(false);
            }
        }
        else
        {
            Debug.LogError("VoiceConnection is null. Cannot leave the voice room.");
        }

        // Photon 룸 떠나기
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        if (voiceConnection != null && voiceConnection.Client.LoadBalancingPeer != null)
        {
            voiceConnection.Client.LoadBalancingPeer.StopThread();
        }
        else
        {
            Debug.LogError("VoiceConnection or LoadBalancingPeer is null. Cannot clean up voice resources.");
        }

        // 서버 씬으로 전환
        PhotonNetwork.LoadLevel("ServerScene");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room.");
    }
}

public class CustomEventCodes
{
    public const byte GameSceneLoaded = 1; // 사용자 정의 이벤트 코드
}