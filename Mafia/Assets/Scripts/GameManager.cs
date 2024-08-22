
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject playerPrefab;
    public bool isConnected = false;
    public int mafiaNum = 1;

    private bool isAnyUIOpen = false;
    [SerializeField]
    private List<GameObject> uiWindows = new List<GameObject>();

    public GameObject LocalPlayer { get; private set; }  // 로컬 플레이어를 저장하기 위한 변수

    private PlayerController playerController;

    private void Awake()
    {
        Debug.Log("게임매니저 awake");
        if (instance == null)
        {
            Debug.Log("게임매니저 1");
            instance = this;
            DontDestroyOnLoad(this.gameObject); // GameManager 객체 유지 설정
        }
        else if (instance != this)
        {
            Debug.Log("게임매니저 2");
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

    private IEnumerator WaitForConnectionAndCreatePlayer()
    {
        Debug.Log("게임매니저 WaitForConnectionAndCreatePlayer");
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