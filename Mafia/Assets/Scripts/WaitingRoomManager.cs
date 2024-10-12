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

    public GameObject LoadingUI; // 로딩 화면 UI
    private CanvasGroup loadingCanvasGroup; // 로딩 화면의 CanvasGroup

    private bool isReady = false;
    static bool gameReady = false;

    public TMP_Text readyBtnText;
    public TMP_Text startBtnText;
    public TMP_Text ConfirmText;

    private static VoiceConnection voiceConnection;
    private static bool voiceConnectionInitialized = false;

    public int mafiaNum = 1;

    private bool hasGameStarted = false; // 게임 시작 여부를 추적하는 변수

    public TMP_Text playerCountText;

    public GameObject CreateRoomUI;

    void Awake()
    {
        // 방장이 혼자 씬을 로딩하면, 나머지 사람들은 자동으로 싱크가 됨
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("isGameStarted"))
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
            { "isGameStarted", false }
        };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        if (PhotonNetwork.IsMasterClient)
        {
            mafiaNum = GameManager.instance.mafiaNum;

            ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable
            {
            { "MafiaNum", mafiaNum }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        }

        GameManager.instance.isConnected = true;
        //GameManager.instance.is

        readyBtnText = ReadyBtn.GetComponentInChildren<TMP_Text>();
        startBtnText = StartBtn.GetComponentInChildren<TMP_Text>();
        loadingCanvasGroup = LoadingUI.GetComponent<CanvasGroup>();

        InitializeVoiceConnection();
        StartCoroutine(InitialState());

        // Update the player count at the start
        UpdatePlayerCount();
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
        // 로딩 UI를 활성화
        LoadingUI.SetActive(true);

        yield return new WaitForSeconds(2); // 4초 대기

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
            // 게임이 시작되지 않았을 때만 버튼 상태를 업데이트
            if (!hasGameStarted)
            {
                StartBtn.GetComponent<Button>().interactable = gameReady;
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                GameStart();
            }
        }

        /*if (PhotonNetwork.IsMasterClient)
        {
            // 최소 4명의 플레이어가 있는지 확인
            if (PhotonNetwork.PlayerList.Length >= 4)
            {
                // 게임이 시작되지 않았을 때만 버튼 상태를 업데이트
                if (!hasGameStarted)
                {
                    StartBtn.GetComponent<Button>().interactable = gameReady;
                }

                // F5 키를 눌렀을 때 게임 시작
                if (Input.GetKeyDown(KeyCode.F5))
                {
                    GameStart();
                }

                // 게임 준비가 완료되었을 때 텍스트 변경
                if (gameReady)
                {
                    startBtnText.text = "게임 시작";
                }
                else
                {
                    startBtnText.text = "모두 준비해야 합니다";
                }
            }
            else
            {
                // 플레이어 수가 4명 미만일 경우 버튼 비활성화 및 텍스트 변경
                StartBtn.GetComponent<Button>().interactable = false;
                startBtnText.text = "최소 4명이 있어야 합니다.";
            }
        }*/

        if (!PhotonNetwork.IsMasterClient && Input.GetKeyDown(KeyCode.F5))
        {
            ClickReadyBtn();
        }

        UpdatePlayerCount(); // Update player count continuously

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

        // ESC 누르면 방 설정 변경 UI 띄우기
        if (!CreateRoomUI.activeSelf && Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsMasterClient && !GameManager.instance.IsAnyUIOpen())
        {
            CreateRoomUI.SetActive(true);
        }
        else if (CreateRoomUI.activeSelf && Input.GetKeyDown(KeyCode.Escape) && PhotonNetwork.IsMasterClient && GameManager.instance.IsAnyUIOpen())
        {
            CreateRoomUI.SetActive(false);            
        }
    }

    void UpdatePlayerCount()
    {
        if (PhotonNetwork.InRoom)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            playerCountText.text = $"Players: {playerCount}"; // Update the TMP_Text UI element
        }
    }

    // Handle player entering or leaving the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCount();
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
                StartBtn.interactable = false;
            }
            else
            {
                Debug.LogError("PhotonNetwork.CurrentRoom is null.");
                return;
            }

            // StartBtn 비활성화
            StartBtn.GetComponent<Button>().interactable = false;
            hasGameStarted = true; // 게임 시작 상태로 설정

            PhotonNetwork.DestroyAll();
            PhotonNetwork.LoadLevel("Level_1");
            Debug.Log("Level_1 입장 완료");

            // 커스텀 이벤트 전송
            RaiseEventOptions options = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            PhotonNetwork.RaiseEvent(CustomEventCodes.GameSceneLoaded, null, options, SendOptions.SendReliable);

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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
            isReady = true;
            SetReadyState(isReady);
            StartBtn.gameObject.SetActive(true);
            ReadyBtn.GetComponent<Button>().interactable = false;
            // 레디 버튼 숨기기
            ReadyBtn.gameObject.SetActive(false);
        }
        Debug.Log("New MasterClient: " + newMasterClient.NickName);
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

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room.");
    }

    public void OnButtonClick(int selectedMafiaNum)
    {
        mafiaNum = selectedMafiaNum;  // 선택된 MafiaNum 저장

        // 필요 시 UI 업데이트나 피드백 추가
        Debug.Log("MafiaNum 버튼이 클릭되었습니다: " + mafiaNum);
    }

    public void UpdateMafiaNum()
    {
        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable()
        {
            { "MafiaNum", mafiaNum }
        };
            PhotonNetwork.CurrentRoom.SetCustomProperties(newProperties);

            ConfirmText.text = "적용되었습니다.";
            StartCoroutine(HideConfirmTextAfterDelay(0.3f));
        }

        IEnumerator HideConfirmTextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            ConfirmText.text = ""; // 메시지 삭제
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // MafiaNum이 업데이트되었는지 확인
        if (propertiesThatChanged.ContainsKey("MafiaNum"))
        {
            int updatedMafiaNum = (int)propertiesThatChanged["MafiaNum"];
            Debug.Log("MafiaNum updated to: " + updatedMafiaNum);
        }
    }
}

public class CustomEventCodes
{
    public const byte GameSceneLoaded = 1; // 사용자 정의 이벤트 코드
}