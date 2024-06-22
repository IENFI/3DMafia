using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class MafiaManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public TMP_Text remainingMafiaText;
    public TMP_Text remainingCitizenText;

    [SerializeField]
    public GameObject MafiaWin;
    public GameObject CitizenWin;

    public int remainingMafiaNum;
    public int remainingCitizenNum;

    private bool isSynced = false;

    void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            remainingMafiaNum = GameManager.instance.mafiaNum;
            photonView.RPC("SyncMafiaNum", RpcTarget.All, remainingMafiaNum);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForSync());
    }

    // Update is called once per frame
    void Update()
    {
        remainingCitizenText.text = "Remaining Citizens: " + remainingCitizenNum.ToString();
        remainingMafiaText.text = "Remaining Mafias: " + remainingMafiaNum.ToString();

        if (remainingMafiaNum == 0)
        {
            CitizenWin.SetActive(true);
            //gameover
        }
        else
        {
            CitizenWin.SetActive(false);
        }

        if (remainingCitizenNum == 0)
        {
            MafiaWin.SetActive(true);
            //gameover
        }
        else
        {
            MafiaWin.SetActive(false);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player player, ExitGames.Client.Photon.Hashtable props)
    {
        foreach (object key in props.Keys)
        {
            if (key.ToString().Equals("isDead"))
            {
                if (player.CustomProperties.ContainsKey("isDead"))
                {
                    if (player.CustomProperties.ContainsKey("isMafia"))
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

        remainingCitizenText.text = "Remaining Citizens: " + remainingCitizenNum.ToString();
        remainingMafiaText.text = "Remaining Mafias: " + remainingMafiaNum.ToString();

    }
}
