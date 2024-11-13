
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject playerPrefab;
    public bool isConnected = false;
    
    //save initial room settings
    public int mafiaNum = 1;
    public int maxPlayerNum = 10; //initialize

    [SerializeField]
    private bool isAnyUIOpen = false;
    [SerializeField]
    private List<GameObject> uiWindows = new List<GameObject>();

    public GameObject LocalPlayer { get; private set; }  // 로컬 플레이어를 저장하기 위한 변수

    private PlayerController playerController;

    private void Awake()
    {

        if (instance == null)
        {

            instance = this;
            DontDestroyOnLoad(this.gameObject); // GameManager 객체 유지 설정
        }
        else if (instance != this)
        {

            Destroy(this.gameObject);
        }

        foreach (var pcc in FindObjectsOfType<PlayerController>())
        {
            if (pcc.photonView.IsMine)
            {
                playerController = pcc;
                break;
            }
        }
    }

    void Update()
    {
        if (isConnected)
        {
            isConnected = false;
            StartCoroutine(WaitForConnectionAndCreatePlayer());
        }

        if (isAnyUIOpen)
        {
            //if(! (playerController == null))
            //playerController.EnableControl(false);
        }
        //if (!(playerController == null))
            //playerController.EnableControl(true);

    }

    public bool CheckRoomPanel(){
        if (GameObject.Find("Canvas/RoomPanel") == null) return false;
        return GameObject.Find("Canvas/RoomPanel")? uiWindows.Contains(GameObject.Find("Canvas/RoomPanel")) : false;
    }

    public int CheckUiList()
    {
        GameObject roomPanel = GameObject.Find("Canvas/RoomPanel");

        // RoomPanel이 uiWindows에 포함되어 있고, 다른 UI 요소도 존재하는지 확인
        if (roomPanel != null && uiWindows.Contains(roomPanel))
        {
            return uiWindows.Count; // RoomPanel 외에 다른 UI가 존재하면 true 반환
        }
        return 0; // RoomPanel이 없거나, 다른 UI가 없는 경우 false 반환
    }


    private IEnumerator WaitForConnectionAndCreatePlayer()
    {
        //Debug.Log("게임매니저 WaitForConnectionAndCreatePlayer");
        yield return new WaitUntil(() => PhotonNetwork.InRoom);
        CreatePlayer();
    }

    public void CreatePlayer()
    {
        Debug.Log("플레이어 생성 시작");
        GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        LocalPlayer = player; // 로컬 플레이어 저장
        Debug.Log("플레이어 생성 완료");
    }
    public void RegisterUIWindow(GameObject uiWindow)
    {
        if (!uiWindows.Contains(uiWindow))
        {
            uiWindows.Add(uiWindow);
        }
    }

    public void UnregisterUIWindow(GameObject uiWindow)
    {
        if (uiWindows.Contains(uiWindow))
        {
            uiWindows.Remove(uiWindow);
        }
    }

    public void SetUIOpenState(bool isOpen)
    {
        isAnyUIOpen = isOpen;
    }

    public bool IsAnyUIOpen()
    {
        CheckUIState();
        return isAnyUIOpen;
    }

    public void CheckUIState()
    {
        foreach (var uiWindow in uiWindows)
        {
            if (uiWindow.activeSelf)
            {
                SetUIOpenState(true);
                return;
            }
        }
        SetUIOpenState(false);
    }
}