
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

    private void Awake()
    {
        Debug.Log("게임매니저 awake");
        if (instance == null)
        {
            Debug.Log("게임매니저 1");
            instance = this;
            DontDestroyOnLoad(gameObject); // GameManager 객체 유지 설정
        }
        else if (instance != this)
        {
            Debug.Log("게임매니저 2");
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("게임매니저 start");
        StartCoroutine(WaitForConnectionAndCreatePlayer());
    }

    private IEnumerator WaitForConnectionAndCreatePlayer()
    {
        Debug.Log("게임매니저 WaitForConnectionAndCreatePlayer");
        yield return new WaitUntil(() => isConnected && PhotonNetwork.InRoom);
        StartCoroutine(CreatePlayer());
    }

    public IEnumerator CreatePlayer()
    {
        Debug.Log("플레이어 생성 시작해볼까?");
        yield return new WaitForSeconds(1); // Adding a small delay to ensure the scene is loaded
        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        Debug.Log("플레이어 생성");
        isConnected = false;
    }
}