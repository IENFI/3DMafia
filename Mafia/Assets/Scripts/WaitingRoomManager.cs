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
    public Button ReadyBtn;
    public Button StartBtn;

    public GameObject LoadingUI; // 로딩 화면 UI
    public GameObject SettingUI;
    private CanvasGroup loadingCanvasGroup; // 로딩 화면의 CanvasGroup

    private bool isReady = false;
    static bool gameReady = false;

    public TMP_Text readyBtnText;
    public TMP_Text startBtnText;
    public TMP_Text ConfirmText;

    private static VoiceConnection voiceConnection;
    private static bool voiceConnectionInitialized = false;

    public int mafiaNum = 1;
    public int maxPlayerNum = 10;

    private bool hasGameStarted = false; // 게임 시작 여부를 추적하는 변수

    public TMP_Text playerCountText;


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
            PhotonNetwork.CurrentRoom.IsOpen = true;
            mafiaNum = GameManager.instance.mafiaNum;
            maxPlayerNum = GameManager.instance.maxPlayerNum;

            ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable
            {
            { "MafiaNum", mafiaNum }, { "MaxPlayerNum", maxPlayerNum }
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

        // TextLoadingAnimation 컴포넌트를 가져옵니다.
        TextLoadingAnimation loadingAnimation = LoadingUI.GetComponentInChildren<TextLoadingAnimation>();
        if (loadingAnimation != null)
        {
            // 3초 동안 애니메이션을 실행합니다.
            yield return StartCoroutine(loadingAnimation.AnimateForDuration(3f));
        }
        else
        {
            // TextLoadingAnimation 컴포넌트가 없는 경우 2초 대기
            yield return new WaitForSeconds(2f);
        }

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

        /*if (PhotonNetwork.IsMasterClient)
        {
            // 게임이 시작되지 않았을 때만 버튼 상태를 업데이트
            if (!hasGameStarted)
            {
                StartBtn.GetComponent<Button>().interactable = gameReady;
            }
            
            if(PhotonNetwork.PlayerList.Length >= (int)PhotonNetwork.CurrentRoom.CustomProperties["MaxPlayerNum"])
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            else
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                GameStart();
            }
        }*/

        if (PhotonNetwork.IsMasterClient)
        {
            // 방에 있는 플레이어 수가 4명 이상이어야 함
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
            }
            else
            {
                // 플레이어 수가 부족할 때 버튼 비활성화
                StartBtn.GetComponent<Button>().interactable = false;
            }
        }

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

        if (gameReady && PhotonNetwork.PlayerList.Length >= 4)
        {
            startBtnText.text = "게임 시작";
        }
        else if (gameReady && PhotonNetwork.PlayerList.Length < 4)
        {
            startBtnText.text = "최소 인원이 부족합니다.";
        }
        else
        {
            startBtnText.text = "모두 준비해야 합니다";
        }
    }

    void UpdatePlayerCount()
    {
        if (PhotonNetwork.InRoom)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
            int mafiaCount = 0;

            // 방의 CustomProperties에서 마피아 수 가져오기
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("MafiaNum"))
            {
                mafiaCount = (int)PhotonNetwork.CurrentRoom.CustomProperties["MafiaNum"];
            }

            maxPlayers = (int)PhotonNetwork.CurrentRoom.CustomProperties["MaxPlayerNum"];

            // Rich Text를 사용하여 마피아 수를 빨간색으로 표시
            string playerCountText = $"인원수: {playerCount} / {maxPlayers} <color=red>({mafiaCount})</color>";
            this.playerCountText.text = playerCountText;
        }
    }

    // Handle player entering or leaving the room
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        int criterion = 0;
        criterion = (int)PhotonNetwork.CurrentRoom.CustomProperties["MaxPlayerNum"];
        if (PhotonNetwork.PlayerList.Length > criterion)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CloseConnection(newPlayer);
            }
            Debug.Log($"{newPlayer.NickName} has been kicked!");
        }

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

            PhotonNetwork.CurrentRoom.IsOpen = false;

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
        // GameManager.instance = null;
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

        // 방 고유 ID 가져오기
        string roomID = PhotonNetwork.CurrentRoom.CustomProperties["RoomID"].ToString();
        int clientID = PhotonNetwork.LocalPlayer.ActorNumber;

        // 방에 남아 있는 플레이어가 없는지 확인하고 마지막 플레이어인 경우 방 정보 삭제
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            // 데이터베이스에서 방 정보 삭제
            DBInteraction.RemoveRoomAppearance(roomID);
            Debug.Log("Room data removed from database as no players are left.");
        }
        // 외형 정보 삭제
        DBInteraction.ResetAppearanceByClientID(roomID, clientID);
        Debug.Log("Room data removed from database.");


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
        float duration = 2f; // 페이드 아웃 지속 시간
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
        //Debug.Log("MafiaNum 버튼이 클릭되었습니다: " + mafiaNum);
    }

    public void OnButtonClickMaxPlayer(int selectedMaxPlayerNum)
    {
        maxPlayerNum = selectedMaxPlayerNum;
    }

    public void UpdateMafiaNum()
    {
        //this code also updates the max player setting
        if (PhotonNetwork.InRoom)
        {
            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable()
        {
            { "MafiaNum", mafiaNum }, { "MaxPlayerNum",  maxPlayerNum }
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

    public void CloseUI()
    {
        SettingUI.SetActive(false);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        UpdatePlayerCount();
        if (propertiesThatChanged.ContainsKey("MaxPlayerNum"))
        {
            KickPlayers();
        }
    }

    public void KickPlayers()
    {
        int criterion = 0;
        criterion = (int)PhotonNetwork.CurrentRoom.CustomProperties["MaxPlayerNum"];

        if (PhotonNetwork.PlayerList.Length > criterion)
        {
            int myIndex = -1;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                {
                    myIndex = i; break;
                }
                //Debug.Log($"{PhotonNetwork.PlayerList[i].NickName} has been kicked!");
            }
            if (myIndex >= criterion)
            {
                LeaveRoom();
            }
        }
    }
}

public class CustomEventCodes
{
    public const byte GameSceneLoaded = 1; // 사용자 정의 이벤트 코드
}