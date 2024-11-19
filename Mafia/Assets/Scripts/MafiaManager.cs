using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MafiaManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public TMP_Text remainingMafiaText;
    public TMP_Text remainingCitizenText;

    [SerializeField]
    public GameObject MafiaWin;
    public GameObject CitizenWin;
    public GameObject RemainingUI;

    [SerializeField]
    public GameObject Merchent;

    private ShopInteraction shopIn;

    public int remainingMafiaNum;
    public int remainingCitizenNum;

    private bool isSynced = false;

    // 테스트시 키고꺼야하는 목록
    private bool gameOver = false;

    [SerializeField]
    public GameObject DeadImage;

    [SerializeField]
    public GameObject choosingMafiaManager;

    void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            remainingMafiaNum = (int)PhotonNetwork.CurrentRoom.CustomProperties["MafiaNum"];
            photonView.RPC("SyncMafiaNum", RpcTarget.All, remainingMafiaNum);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //actually, this one manage the whole level
        GameManager.instance.isConnected = true;
        shopIn = Merchent.GetComponent<ShopInteraction>();
        StartCoroutine(WaitForSync());

        StartCoroutine(WaitForPlayers());        
    }


    private IEnumerator WaitForPlayers()
    {
        Debug.Log("마피아 매니저 WaitForPlayers");
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Player").Length == PhotonNetwork.PlayerList.Length);


       GameObject playerObject = PhotonNetwork.LocalPlayer.TagObject as GameObject;
       PhotonView playerPV = playerObject.GetComponent<PhotonView>();
       playerPV.RPC("Spawn", RpcTarget.All);

    }

    // Update is called once per frame
    void Update()
    {
        remainingCitizenText.text = "남은 시민 수: " + remainingCitizenNum.ToString();
        remainingMafiaText.text = "남은 마피아 수: " + remainingMafiaNum.ToString();

        
        // 테스트시 키고 끌 목록
        
       {
            if (remainingMafiaNum == 0 || shopIn.SaveCoin >= 1000)
            {
                CitizenWin.SetActive(true);
                if (!gameOver)
                {
                    StartCoroutine(BackToHome());
                }
                gameOver = true;
                //gameover
            }
            else
            {
                CitizenWin.SetActive(false);
            }

            if (remainingCitizenNum <= remainingMafiaNum)
            {
                MafiaWin.SetActive(true);
                if (!gameOver)
                {
                    StartCoroutine(BackToHome());
                }
                gameOver = true;
                //gameover
            }
            else
            {
                MafiaWin.SetActive(false);
            }
        }
        

        // if (Input.GetKeyDown(KeyCode.Tab))
        // {
        //     RemainingUI.SetActive(!RemainingUI.activeSelf); // 탭 키로 UI를 켜고 끕니다.
        // } 
    }

    public override void OnPlayerPropertiesUpdate(Player player, ExitGames.Client.Photon.Hashtable props)
    {
        foreach (object key in props.Keys)
        {
            if (key.ToString().Equals("isDead"))
            {
                if ((bool)player.CustomProperties["isDead"])
                {
                    if(player == PhotonNetwork.LocalPlayer)
                    {
                        DeadImage.SetActive(true);
                        Debug.Log("DeadImage활성화 여부 " );
                    }
                    if ((bool)player.CustomProperties["isMafia"])
                    {
                        remainingMafiaNum--;
                    }
                    else
                    {
                        remainingCitizenNum--;
                    }
                }
            }
        }
    }

    [PunRPC]
    void SyncMafiaNum(int num)
    {
        remainingMafiaNum = num;
        isSynced = true;
    }

    IEnumerator WaitForSync()
    {
        while (!isSynced)
        {
            yield return null;
        }

        int playerNum;
        playerNum = PhotonNetwork.PlayerList.Length;
        if (remainingMafiaNum > playerNum)
            remainingMafiaNum = playerNum;
        remainingCitizenNum = playerNum - remainingMafiaNum;

        remainingCitizenText.text = "남은 시민 수: " + remainingCitizenNum.ToString();
        remainingMafiaText.text = "남은 마피아 수: " + remainingMafiaNum.ToString();

    }


    IEnumerator BackToHome()
    {
        yield return new WaitForSeconds(10f);

        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyAll();
            choosingMafiaManager.GetComponent<ChoosingMafiaManager>().Initializing();
            PhotonNetwork.LoadLevel("Level_0");
        }
    }
}