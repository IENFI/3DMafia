using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

public class VoteManager : MonoBehaviourPunCallbacks
{
    [Header("VoteUI")]
    public GameObject VoteUI;
    public Button btn0;
    public Button btn1;
    public Button btn2;
    public Button btn3;
    public Button btn4;
    public Button btn5;
    public Button btn6;
    public Button btn7;
    public Button btn8;
    public Button btn9;

    private TMP_Text btnText0;
    private TMP_Text btnText1;
    private TMP_Text btnText2;
    private TMP_Text btnText3;
    private TMP_Text btnText4;
    private TMP_Text btnText5;
    private TMP_Text btnText6;
    private TMP_Text btnText7;
    private TMP_Text btnText8;
    private TMP_Text btnText9;

    [SerializeField]
    public GameObject voteManager;

    private Dictionary<int, TMP_Text> btnTexts;
    private Dictionary<int, Button> btns;

    // VoteManager   Ȱ  ȭ Ǹ  Start      Ϸ                ϴٰ                θ    Ȱ  ȭ.

    public bool isMeetingActivated = true;

    //voteList       client 鿡       .
    private int[] voteList = new int[10]; //10    ִ   ÷  ̾    ,     list  ƴϰ  array  , 0      ʱ ȭ
    private int criterion; // ÷  ̾       ݼ 

    // Start is called before the first frame update
    void MeetingStart()
    {
        criterion = (int)((double)PhotonNetwork.PlayerList.Length / 2 + 1);

        btnText0 = btn0.GetComponentInChildren<TMP_Text>();
        btnText1 = btn1.GetComponentInChildren<TMP_Text>();
        btnText2 = btn2.GetComponentInChildren<TMP_Text>();
        btnText3 = btn3.GetComponentInChildren<TMP_Text>();
        btnText4 = btn4.GetComponentInChildren<TMP_Text>();
        btnText5 = btn5.GetComponentInChildren<TMP_Text>();
        btnText6 = btn6.GetComponentInChildren<TMP_Text>();
        btnText7 = btn7.GetComponentInChildren<TMP_Text>();
        btnText8 = btn8.GetComponentInChildren<TMP_Text>();
        btnText9 = btn9.GetComponentInChildren<TMP_Text>();


        // Dictionary  ʱ ȭ
        btns = new Dictionary<int, Button>
        {
            { 0, btn0 },
            { 1, btn1 },
            { 2, btn2 },
            { 3, btn3 },
            { 4, btn4 },
            { 5, btn5 },
            { 6, btn6 },
            { 7, btn7 },
            { 8, btn8 },
            { 9, btn9 },
        };

        btnTexts = new Dictionary<int, TMP_Text>
        {
            { 0, btnText0 },
            { 1, btnText1 },
            { 2, btnText2 },
            { 3, btnText3 },
            { 4, btnText4 },
            { 5, btnText5 },
            { 6, btnText6 },
            { 7, btnText7 },
            { 8, btnText8 },
            { 9, btnText9 }
        };


        Initialize();
        VoteUI.SetActive(true);
    }

    /*
    public void MeetingStart()
    {
        //UI   Ȱ  ȭ ->   ư       ǥ
        VoteUI.SetActive(true);
    }
    */

    void Update()
    {
        if (isMeetingActivated)
        {
            MeetingStart();
            isMeetingActivated = false;
        }

        //           ÿ ,
        // ð             ߰   ʿ 
        if (IsElected() || AllVoted())
        {
            VotingResult();
            VoteUI.SetActive(false);
            // ȸ       .      θ        .
            isMeetingActivated = true;
            voteManager.SetActive(false);
        }

    }

    //   ึ    ʱ ȭ
    private void Initialize()
    {
        for (int i = 0; i < 10; i++)
        {
            btnTexts[i].text = " ";
        }

        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "voted" , -1 }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        for (int i = 0; i < 10; i++)
        {
            voteList[i] = 0;
        }

        for (int i = 0; i < 10; i++)
        {
            btns[i].GetComponent<Button>().interactable = true;
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            SendText(i, 0);
        }
    }

    // ÷  ̾     °        Ʈ  Ƿ    OnPlayerPropertiesUpdate       ٷ     
    //         ǥ         ¸     ⼭     ȭ
    public override void OnPlayerPropertiesUpdate(Player player, Hashtable props)
    {
        // ÷  ̾      ȣ   Ž   ϰ    ǥ     Ʈ    ݿ 
        int num;
        num = (int)player.CustomProperties["voted"];
        if (num >= 0)
        {
            voteList[num]++;
            //  ǥ  Ϸ             ̻    ǥ     
            if (PhotonNetwork.LocalPlayer == player)
            {
                //  ǥ  Ϸ             ̻    ǥ     
                for (int i = 0; i < 10; i++)
                {
                    btns[i].GetComponent<Button>().interactable = false;
                }
            }
            SendText(num, voteList[num]);
        }
        /*
        else
        {
            int playerNum = 0;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if(player == PhotonNetwork.PlayerList[i])
                {
                    playerNum = i;
                    break;
                }
            }
            SendText(playerNum, 0);
        }
        */
    }

    private void SendText(int i, int voteCount)
    {
        btnTexts[i].text = "Name: " + PhotonNetwork.PlayerList[i].NickName + ", vote count: " + voteCount.ToString();
    }

    #region Voting functions
    //  ư            
    public void Vote0()
    {
        int btnNum;
        btnNum = 0;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "voted" , btnNum }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void Vote1()
    {
        int btnNum;
        btnNum = 1;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "voted" , btnNum }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void Vote2()
    {
        int btnNum;
        btnNum = 2;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "voted" , btnNum }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void Vote3()
    {
        int btnNum;
        btnNum = 3;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "voted" , btnNum }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void Vote4()
    {
        int btnNum;
        btnNum = 4;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "voted" , btnNum }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void Vote5()
    {
        int btnNum;
        btnNum = 5;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "voted" , btnNum }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void Vote6()
    {
        int btnNum;
        btnNum = 6;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "voted" , btnNum }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void Vote7()
    {
        int btnNum;
        btnNum = 7;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "voted" , btnNum }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void Vote8()
    {
        int btnNum;
        btnNum = 8;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "voted" , btnNum }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void Vote9()
    {
        int btnNum;
        btnNum = 9;
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "voted" , btnNum }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    #endregion

    private void VotingResult()
    {
        for (int i = 0; i < 10; i++)
        {
            if (voteList[i] > criterion)
            {
                //PhotonNetwork.PlayerList[i]    ̴   ̺ Ʈ  ߻ 
                break;
            }
        }
    }

    private bool IsElected()
    {
        bool result = false;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (voteList[i] >= criterion)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    private bool AllVoted()
    {
        bool result = false;
        int count = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            count += voteList[i];
        }
        if (count == PhotonNetwork.PlayerList.Length)
        {
            result = true;
        }

        return result;
    }
}
