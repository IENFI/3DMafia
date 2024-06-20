
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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForConnectionAndCreatePlayer());
    }

    private IEnumerator WaitForConnectionAndCreatePlayer()
    {
        yield return new WaitUntil(() => isConnected && PhotonNetwork.InRoom);
        StartCoroutine(CreatePlayer());
    }

    public IEnumerator CreatePlayer()
    {
        yield return new WaitForSeconds(1); // Adding a small delay to ensure the scene is loaded
        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        Debug.Log("플레이어 생성");
        isConnected = false;
    }
}