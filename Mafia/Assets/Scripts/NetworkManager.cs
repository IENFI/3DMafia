using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks // 안현석 똑바로해라
{
    [Header("DisconnectPanel")]
    public GameObject DisconnectPanel;
    public TMP_InputField NickNameInput;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public TMP_InputField RoomInput;
    public TextMeshProUGUI WelcomeText;
    public TextMeshProUGUI LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public TextMeshProUGUI ListText;
    public TextMeshProUGUI RoomInfoText;
    public TextMeshProUGUI[] ChatText;
    public TMP_InputField ChatInput;

    [Header("CreateRoomUI")]
    public GameObject CreateRoomUI;

    [Header("ETC")]
    public TextMeshProUGUI StatusText;
    public PhotonView PV;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    [SerializeField]
    private CreateRoomUI createRoomUI;


    #region 방리스트 갱신
    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else
        {
            GameManager.instance.isConnected = true;
            PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        }
        MyListRenewal();
    }

    void MyListRenewal()
    {
        // 최대페이지
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음버튼
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            if (myList.Count > 0)
            {
                CellBtn[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
                CellBtn[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";

            }
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion

    #region 서버연결
    void Awake()
    {
        Screen.SetResolution(1500, 1080, false);
        // 방장이 혼자 씬을 로딩하면, 나머지 사람들은 자동으로 싱크가 됨
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
        
    }



    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfoText.text = (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "Lobby / " + PhotonNetwork.CountOfPlayers + "Connected";
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        DisconnectPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        RoomPanel.SetActive(false);
        CreateRoomUI.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        WelcomeText.text = "Welcome " + PhotonNetwork.LocalPlayer.NickName ;
        myList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (DisconnectPanel != null)
        {
            DisconnectPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("disconnectPanel has already been destroyed.");
        }

        DisconnectPanel.SetActive(true);
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        CreateRoomUI.SetActive(false);
    }
    #endregion

    #region 방 설정
    public void SettingRoom()
    {
        DisconnectPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        RoomPanel.SetActive(false);
        CreateRoomUI.SetActive(true);
    }
    #endregion

    #region 방
    public void CreateRoom()
    {
        string roomName = RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text;
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = createRoomUI.roomData.maxPlayerCount };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    public void JoinRandomRoom()
    {
        GameManager.instance.isConnected = true;
        PhotonNetwork.JoinRandomRoom();
    }

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnJoinedRoom()
    {
        DisconnectPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        CreateRoomUI.SetActive(false);

        RoomRenewal();
        ChatInput.text = "";

        for (int i = 0; i < ChatText.Length; i++)
        {
            if (ChatText[i] == null)
            {
                Debug.LogError($"ChatText[{i}] is not assigned.");
                continue;
            }
            ChatText[i].text = "";
        }

        /*
        GameManager.instance.isConnected = true;
        PhotonNetwork.LoadLevel("Level_1");
        Debug.Log("04. 방 입장 완료");
        */
        
        
        if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.isConnected = true;
            PhotonNetwork.LoadLevel("Level_1");
            Debug.Log("04. 방 입장 완료");
            
        }
        
        //GameManager.instance.StartCoroutine(GameManager.instance.CreatePlayer());

    }

    public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

    public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + newPlayer.NickName + "has joined</color>");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RoomRenewal();
        ChatRPC("<color=yellow>" + otherPlayer.NickName + " has left</color>");
    }

    void RoomRenewal()
    {
        ListText.text = "";
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "people / " + PhotonNetwork.CurrentRoom.MaxPlayers + "max";
    }
    #endregion


    #region 채팅
    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
    #endregion
}