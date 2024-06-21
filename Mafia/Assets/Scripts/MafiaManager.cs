using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MafiaManager : MonoBehaviour
{
    [SerializeField]
    public TMP_Text remainingMafiaText;
    public TMP_Text remainingCitizenText;

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
        // if(leftedMafiaNum == 0)
    }
}
