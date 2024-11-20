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
    public TextMeshProUGUI NickNameError;
    public Button ConnectBtn;

    [Header("LobbyPanel")]
    public GameObject LobbyPanel;
    public TextMeshProUGUI WelcomeText;
    public TextMeshProUGUI LobbyInfoText;
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;

    [Header("CreateRoomUI")]
    public GameObject CreateRoomUI;
    public TMP_InputField RoomInput;

    [Header("ETC")]
    public TextMeshProUGUI StatusText;
    public PhotonView PV;
    public TextMeshProUGUI RandomError;

    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;

    [SerializeField]
    private CreateRoomUI createRoomUI;

    private int selectedMafiaNum = 1;
    public int selectedMaxPlayerNum = 10;

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





    //void MyListRenewal()
    //{


    //    // 최대페이지
    //    maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

    //    // 이전, 다음버튼
    //    PreviousBtn.interactable = (currentPage <= 1) ? false : true;
    //    NextBtn.interactable = (currentPage >= maxPage) ? false : true;

    //    // 페이지에 맞는 리스트 대입
    //    multiple = (currentPage - 1) * CellBtn.Length;
    //    for (int i = 0; i < CellBtn.Length; i++)
    //    {
    //        Debug.Log("for문 시작");
    //        CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
    //        if (myList.Count > 0)
    //        {
    //            Debug.Log("if실행");
    //            CellBtn[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
    //            CellBtn[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
    //            CellBtn[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = (multiple + i < myList.Count) ? "Mafia 수 : " + MafiaCnt : "";
    //            Debug.Log("UI 갱신됨: " + "Mafia 수 : " + MafiaCnt);
    //            CellBtn[i].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = (multiple + i < myList.Count) ? "MaxPlayer 수 : " + myList[multiple + i].MaxPlayers : "";
    //        }
    //    }
    //}

    void MyListRenewal()
    {
        // 최대페이지 계산
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음 버튼 상태 설정
        PreviousBtn.interactable = (currentPage <= 1)? false : true;
        NextBtn.interactable = (currentPage >= maxPage)? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * CellBtn.Length;

        for (int i = 0; i < CellBtn.Length; i++)
        {
            // 리스트의 크기를 넘지 않도록 범위 체크
            if (multiple + i < myList.Count)
            {
                CellBtn[i].interactable = true;
                var roomInfo = myList[multiple + i];

                CellBtn[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = roomInfo.Name;

                int maxPlayerNum = 0;
                if (roomInfo.CustomProperties.ContainsKey("MaxPlayerNum"))
                {
                    maxPlayerNum = (int)roomInfo.CustomProperties["MaxPlayerNum"];
                }
                CellBtn[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = roomInfo.PlayerCount + "/" + maxPlayerNum;

                // 각 방의 MafiaNum을 가져와서 UI에 표시
                int mafiaNum = 0;
                if (roomInfo.CustomProperties.ContainsKey("MafiaNum"))
                {
                    mafiaNum = (int)roomInfo.CustomProperties["MafiaNum"];
                }
                CellBtn[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "마피아 수 : " + mafiaNum;

                if (roomInfo.IsOpen)
                {
                    CellBtn[i].interactable = true;
                }
                else
                {
                    CellBtn[i].interactable = false;
                }
            }
            else
            {
                // 유효하지 않은 인덱스일 경우 버튼 비활성화 및 텍스트 초기화
                CellBtn[i].interactable = false;
                CellBtn[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
                CellBtn[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                CellBtn[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "";
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

    //public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //{
    //    // Create a new list to store rooms that should be displayed
    //    List<RoomInfo> roomsToShow = new List<RoomInfo>();

    //    // Iterate through the updated room list
    //    foreach (RoomInfo room in roomList)
    //    {
    //        // Check if the room is removed from the list
    //        if (room.RemovedFromList)
    //        {
    //            // Remove the room from myList if it's in the list
    //            if (myList.Contains(room))
    //            {
    //                myList.Remove(room);
    //            }
    //        }
    //        else
    //        {
    //            // Check if the room has started the game
    //            if (room.CustomProperties.ContainsKey("isGameStarted") && (bool)room.CustomProperties["isGameStarted"])
    //            {
    //                // If the game has started, we should not add it to myList
    //                if (myList.Contains(room))
    //                {
    //                    myList.Remove(room);
    //                }
    //            }
    //            else
    //            {
    //                // Add the room to myList if it's not already in the list
    //                if (!myList.Contains(room))
    //                {
    //                    myList.Add(room);
    //                }
    //            }
    //        }
    //    }

    //    // Update the UI to display the updated room list
    //    MyListRenewal();
    //}
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

        // PhotonNetwork.ConnectToRegion("kr");

        // 게임 버전 지정

        // 게임을 키면 디비에서 닉네임 정리
        PlayerDBController playerDBController = FindObjectOfType<PlayerDBController>();
        playerDBController.DeleteInactivePlayers();

    }

    void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
        LobbyInfoText.text = "전체: " + PhotonNetwork.CountOfPlayers + "      로비: " + (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms);
        MyListRenewal();
    }

    public void Connect()
    {
        PhotonNetwork.NickName = NickNameInput.text;
        if (string.IsNullOrEmpty(PhotonNetwork.NickName))
        {
            NickNameError.text = "닉네임을 입력하세요.";
            return;
        }
        else if (DBInteraction.Login(PhotonNetwork.NickName))
        {
            NickNameError.text = "";
            PhotonNetwork.ConnectUsingSettings();
            ConnectBtn.interactable = false;
        }
        else
        {
            NickNameError.text = "이미 사용 중인 닉네임입니다.";
            return;
        }
    }

    public override void OnConnectedToMaster() => PhotonNetwork.JoinLobby();

    public override void OnJoinedLobby()
    {
        DisconnectPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        CreateRoomUI.SetActive(false);
        if (PhotonNetwork.NickName == "")
        {
            PhotonNetwork.NickName = NickNameInput.text;
            WelcomeText.text = "닉네임: " + PhotonNetwork.NickName + "";
            myList.Clear();
        }
        WelcomeText.text = "닉네임: " + PhotonNetwork.NickName + "";
        myList.Clear();
    }

    public void Disconnect() => PhotonNetwork.Disconnect();

    void OnApplicationQuit()
    {
        // 창을 닫을 때 실행할 코드 작성
        Debug.Log("애플리케이션이 종료됩니다.");
        // 예를 들어, 데이터를 저장하거나, 서버에 연결을 종료하는 등의 작업을 수행할 수 있습니다.
        DBInteraction.DeletePlayer(PhotonNetwork.NickName);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        DBInteraction.DeletePlayer(PhotonNetwork.NickName);

        if (DisconnectPanel != null)
        {
            DisconnectPanel.SetActive(true);
            NickNameError.text = "";
            ConnectBtn.interactable = true;
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
    //public void CreateRoom()
    //{

    //    string roomName = RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text;

    //    //RoomOptions roomOptions = new RoomOptions { MaxPlayers = createRoomUI.roomData.maxPlayerCount };
    //    //roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "isGameStarted", false } };
    //    //roomOptions.CustomRoomPropertiesForLobby = new string[] { "isGameStarted" };
    //    RoomOptions roomOptions = new RoomOptions
    //    {
    //        MaxPlayers = createRoomUI.roomData.maxPlayerCount,
    //        CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
    //    {
    //        { "isGameStarted", false },
    //        { "MafiaNum", selectedMafiaNum }  // 저장된 MafiaNum을 포함시킵니다.
    //    },
    //        CustomRoomPropertiesForLobby = new string[] { "isGameStarted", "MafiaNum" }  // 로비에서 사용할 속성 목록
    //    };
    //    PhotonNetwork.CreateRoom(roomName, roomOptions);
    //}

    public void OnButtonClick(int mafiaNum)
    {
        selectedMafiaNum = mafiaNum;
        // 선택된 옵션에 대해 UI를 업데이트하거나 피드백을 제공할 수 있습니다.
    }

    public void OnButtonClickMaxPlayer(int maxPlayerNum)
    {
        selectedMaxPlayerNum = maxPlayerNum;
    }

    public void CreateRoom()
    {
        string roomName = RoomInput.text == "" ? "Room" + Random.Range(0, 100) : RoomInput.text;
        string roomID = System.Guid.NewGuid().ToString();  // 고유한 Room ID 생성

        RoomOptions roomOptions = new RoomOptions
        {
            //MaxPlayers = createRoomUI.roomData.maxPlayerCount,
            MaxPlayers = 12,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable()
            {
                { "isGameStarted", false },
                { "MafiaNum", selectedMafiaNum }, // 저장된 MafiaNum을 포함시킵니다.
                { "MaxPlayerNum", selectedMaxPlayerNum },
                { "RoomID", roomID }  // 고유 Room ID를 추가
            },
            CustomRoomPropertiesForLobby = new string[] { "isGameStarted", "MafiaNum", "MaxPlayerNum", "RoomID" }  // 로비에서 사용할 속성 목록
        };

        PhotonNetwork.CreateRoom(roomName, roomOptions);
        // 방 생성 후 DB에 roomID와 초기 외형 데이터 추가
        DBInteraction.AddRoomAppearance(roomID);
    }


    public void JoinRandomRoomExcluding(string excludedRoomName)
    {
        // myList에서 제외할 방을 제외한 나머지 방을 필터링
        List<RoomInfo> filteredRooms = new List<RoomInfo>();

        foreach (var room in myList)
        {
            if (room.CustomProperties.ContainsKey("isGameStarted"))
            {
                if ((bool)room.CustomProperties["isGameStarted"])
                {
                    return;
                }
                else
                {
                    filteredRooms.Add(room);
                }
            }
        }

        if (filteredRooms.Count > 0)
        {
            // 필터링된 방에서 랜덤으로 방을 선택하여 참가
            int randomIndex = Random.Range(0, filteredRooms.Count);
            PhotonNetwork.JoinRoom(filteredRooms[randomIndex].Name);
        }
        else
        {
            RandomError.text = "입장할 수 있는 방이 없습니다.";
            StartCoroutine(HideRandomErrorAfterDelay(3f));
        }

        IEnumerator HideRandomErrorAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            RandomError.text = ""; // 메시지 삭제
        }
    }

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    //public override void OnJoinedRoom()
    //{
    //    DisconnectPanel.SetActive(false);
    //    LobbyPanel.SetActive(false);
    //    CreateRoomUI.SetActive(false);

    //    /*
    //    PhotonNetwork.LoadLevel("Level_1");
    //    Debug.Log("04. 방 입장 완료");
    //    */


    //    if (PhotonNetwork.IsMasterClient)
    //    {
    //        Debug.Log("마스터클라이언트임");
    //        PhotonNetwork.LoadLevel("Level_0");
    //        Debug.Log("04. 방 입장 완료");


    //    }
    //    else
    //    {
    //        Debug.Log("마스터 클라이언트 아님");
    //    }


    //}
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
            PhotonNetwork.LoadLevel("Level_0");
            Debug.Log("04. 방 입장 완료");


        }
        else
        {
            Debug.Log("마스터 클라이언트 아님");
        }


    }

    //public override void OnCreateRoomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }

    //public override void OnJoinRandomFailed(short returnCode, string message) { RoomInput.text = ""; CreateRoom(); }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"방 생성 실패: {message}. 재시도합니다.");
        RoomInput.text = "";
        // 방 생성 실패 시 UI 상태를 명확히 설정
        CreateRoomUI.SetActive(true);
        LobbyPanel.SetActive(false);
        DisconnectPanel.SetActive(false);
        // 재시도 로직 필요 시 추가
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"무작위 방 참가 실패: {message}. 방 생성 시도합니다.");
        RoomInput.text = "";
        // 무작위 방 참가 실패 시 UI 상태를 명확히 설정
        CreateRoomUI.SetActive(true);
        LobbyPanel.SetActive(false);
        DisconnectPanel.SetActive(false);
        // 방 생성 로직을 호출하거나 UI 상태를 적절히 설정
    }

    public void Refresh()
    {
        StartCoroutine(WaitForDisconnectionAndConnect());
    }

    private IEnumerator WaitForDisconnectionAndConnect()
    {
        Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        Connect();
    }

    #endregion
}