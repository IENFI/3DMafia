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

    // Start is called before the first frame update
    void Start()
    {
        int playerNum;
        playerNum = PhotonNetwork.PlayerList.Length;
        remainingMafiaNum = GameManager.instance.mafiaNum;
        if (remainingMafiaNum > playerNum)
            remainingMafiaNum = playerNum;
        remainingCitizenNum = playerNum - remainingMafiaNum;

        remainingCitizenText.text = "Remaining Citizens: " + remainingCitizenNum.ToString();
        remainingMafiaText.text = "Remaining Mafias: " + remainingMafiaNum.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        remainingCitizenText.text = "Remaining Citizens: " + remainingCitizenNum.ToString();
        remainingMafiaText.text = "Remaining Mafias: " + remainingMafiaNum.ToString();

        if(remainingMafiaNum == 0)
        {
            CitizenWin.SetActive(true);
            //gameover
        }

        if (remainingCitizenNum == 0)
        {
            MafiaWin.SetActive(true);
            //gameover
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

}
