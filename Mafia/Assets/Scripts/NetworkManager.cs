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
    public TextMeshProUGUI ListText;
    public TextMeshProUGUI RoomInfoText;

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

    /*public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])){ 
                    myList.Add(roomList[i]);}
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }*/
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // Create a new list to store rooms that should be displayed
        List<RoomInfo> roomsToShow = new List<RoomInfo>();

        // Iterate through the updated room list
        foreach (RoomInfo room in roomList)
        {
            // Check if the room is removed from the list
            if (room.RemovedFromList)
            {
                // Remove the room from myList if it's in the list
                if (myList.Contains(room))
                {
                    myList.Remove(room);
                }
            }
            else
            {
                // Check if the room has started the game
                if (room.CustomProperties.ContainsKey("isGameStarted") && (bool)room.CustomProperties["isGameStarted"])
                {
                    // If the game has started, we should not add it to myList
                    if (myList.Contains(room))
                    {
                        myList.Remove(room);
                    }
                }
                else
                {
                    // Add the room to myList if it's not already in the list
                    if (!myList.Contains(room))
                    {
                        myList.Add(room);
                    }
                }
            }
        }

        // Update the UI to display the updated room list
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
        LobbyInfoText.text = "전체: " + PhotonNetwork.CountOfPlayers + ",     로비: " + (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms);
    }

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        DisconnectPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        CreateRoomUI.SetActive(false);
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        WelcomeText.text = "닉네임: " + PhotonNetwork.LocalPlayer.NickName + "";
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

        //DisconnectPanel.SetActive(true); hs수정
        LobbyPanel.SetActive(false);
        CreateRoomUI.SetActive(false);
    }
    #endregion

    #region 방 설정
    public void SettingRoom()
    {
        DisconnectPanel.SetActive(false);
        // LobbyPanel.SetActive(false);
        CreateRoomUI.SetActive(true);
    }
    #endregion

    #region 방
    /*public void CreateRoom()
    {
        string roomName = RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text;
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = createRoomUI.roomData.maxPlayerCount };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }*/
    public void CreateRoom()
    {
        string roomName = RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text;
        RoomOptions roomOptions = new RoomOptions { MaxPlayers = createRoomUI.roomData.maxPlayerCount };
        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "isGameStarted", false } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "isGameStarted" };
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }


    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public override void OnJoinedRoom()
    {
        DisconnectPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        CreateRoomUI.SetActive(false);

        /*
        PhotonNetwork.LoadLevel("Level_1");
        Debug.Log("04. 방 입장 완료");
        */


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("마스터클라이언트임");
            PhotonNetwork.LoadLevel("Level_0");
            Debug.Log("04. 방 입장 완료");


        }
        else
        {
            Debug.Log("마스터 클라이언트 아님");
        }


    }

    public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

    public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }
    #endregion
}