using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class ChoosingMafiaManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public TMP_Text mafiaText;
    // Start is called before the first frame update
    public bool isMafia = false;
    public int mafiaNum = 1;

    void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int playerNum;
            List<int> MafiaList;
            playerNum = PhotonNetwork.PlayerList.Length;
            mafiaNum = GameManager.instance.mafiaNum;
            if (mafiaNum > playerNum)
                mafiaNum = playerNum;
            //mafiaNum = (int)(Math.Sqrt((double)playerNum) + 0.5);
            MafiaList = SelectRandomNumbers(playerNum, mafiaNum);
            //Initializing();
            MafiaSelecting(mafiaNum, MafiaList);
            Debug.Log("MafiaList: " + string.Join(", ", MafiaList));
            //PrintPlayerList();
        }
    }

    void Start()
    {

        mafiaText.text = " ";

        /*
        if (PhotonNetwork.IsMasterClient)
        {
            int playerNum;
            int mafiaNum;
            List<int> MafiaList;
            playerNum = PhotonNetwork.PlayerList.Length;
            mafiaNum = (int)(Math.Sqrt((double)playerNum)+0.5);
            MafiaList = SelectRandomNumbers(playerNum, mafiaNum);
            //Initializing();
            MafiaSelecting(playerNum, MafiaList);
        }
        */
        //Player localPlayer = PhotonNetwork.LocalPlayer;

        /*
        isMafia = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("isMafia") && (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMafia"];
        */

        SendMafiaText();
    }

        public void SendMafiaText()
        {
            if (isMafia)
            {
                mafiaText.text = "Mafia";
            }
            else
            {
                mafiaText.text = "Citizen";
            }
        }
    /*
    // Update is called once per frame
    void Update()
    {

    }
    */

    public override void OnPlayerPropertiesUpdate(Player player, Hashtable props)
    {
        if (player.CustomProperties.ContainsKey("isMafia") && player == PhotonNetwork.LocalPlayer)
        {
            isMafia = true;
            SendMafiaText();
        }
    }

    static List<int> SelectRandomNumbers(int N, int n)
    {
        System.Random random = new System.Random();
        List<int> numbers = Enumerable.Range(1, N).ToList();
        List<int> result = new List<int>();

        for (int i = 0; i < n; i++)
        {
            int index = random.Next(numbers.Count);
            result.Add(numbers[index]);
            numbers.RemoveAt(index);
        }
        return result;
    }

    /*
    void Initializing()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
            { "isMafia" , false }
            };
            player.SetCustomProperties(props);
        }
    }
    */

    void MafiaSelecting(int mafiaNum, List<int> MafiaList)
    {
        for(int i = 0; i < mafiaNum; i++)
        {
            ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
            {
            { "isMafia" , true }
            };
            PhotonNetwork.PlayerList[MafiaList[i]-1].SetCustomProperties(props);
        }
    }

    void PrintPlayerList()
    {
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Player player in players)
        {
            Debug.Log("Player ID: " + player.ActorNumber + ", Player Name: " + player.NickName + ", isMafia = " + (player.CustomProperties.ContainsKey("isMafia") && (bool)player.CustomProperties["isMafia"]).ToString());
        }
    }
}
