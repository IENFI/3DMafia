using System;
using System.Collections;
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
    [SerializeField]
    public GameObject mafiaManager;

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

    [Header("VoteReuslt")]
    public CanvasGroup VoteResultUI; // VoteResultUI의 CanvasGroup을 설정합니다.
    public TMP_Text resultMessage;


    private Dictionary<int, TMP_Text> btnTexts;
    private Dictionary<int, Button> btns;

    // VoteManager   Ȱ  ȭ Ǹ  Start      Ϸ                ϴٰ                θ    Ȱ  ȭ.

    public bool isMeetingActivated = true;

    //voteList       client 鿡       .
    private int[] voteList = new int[10]; //10    ִ   ÷  ̾    ,     list  ƴϰ  array  , 0      ʱ ȭ
    private int criterion; // ÷  ̾       ݼ 
    private int remainingPlayerNum;

    private bool VoteResultBool;
    private int VoteSelectPlayerNum = -1;

    // Start is called before the first frame update
    void MeetingStart()
    {
        //calculate criterion from remainig users
        remainingPlayerNum = mafiaManager.GetComponent<MafiaManager>().remainingMafiaNum + mafiaManager.GetComponent<MafiaManager>().remainingCitizenNum;
        criterion = (int)((double)remainingPlayerNum / 2 + 1);

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
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("isDead"))
            return;

        if (isMeetingActivated)
        {
            MeetingStart();
            isMeetingActivated = false;
            VoteResultBool = true;
            VoteSelectPlayerNum = -1;
        }

        //           ÿ ,
        // ð             ߰   ʿ 
        if ((IsElected() || AllVoted())&&VoteResultBool)
        {
            VoteResultBool = false;
            VotingResult();
            StartCoroutine(DelayActions());
        }


    }

    private IEnumerator DelayActions()
    {
        // 3초 대기
        yield return new WaitForSeconds(10f);

       
        isMeetingActivated = true;
        voteManager.SetActive(false);
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
            if (i < PhotonNetwork.PlayerList.Length)
            {
                if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("isDead"))
                    btns[i].GetComponent<Button>().interactable = false;
                else
                    btns[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                btns[i].GetComponent<Button>().interactable = false;
            }
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            SendText(i, 0);
        }
    }

    // ÷  ̾     °        Ʈ  Ƿ    OnPlayerPropertiesUpdate       ٷ     
    //         ǥ         ¸     ⼭     ȭ
    public override void OnPlayerPropertiesUpdate(Player player, ExitGames.Client.Photon.Hashtable props)
    {
        // ÷  ̾      ȣ   Ž   ϰ    ǥ     Ʈ    ݿ 
        foreach (object key in props.Keys)
        {
            if (key.ToString().Equals("voted"))
            {
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
            }
        }
    }

    private void SendText(int i, int voteCount)
    {
        btnTexts[i].text = PhotonNetwork.PlayerList[i].NickName + "\n" + voteCount.ToString();
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
        GameObject timerObject = GameObject.FindWithTag("Timer");
        if (timerObject != null)
        {
            Timer timer = timerObject.GetComponent<Timer>();

            for (int i = 0; i < 10; i++)
            {
                if (voteList[i] >= criterion)
                {
                    Debug.Log(i);
                    VoteSelectPlayerNum = i;
                    StartCoroutine(FadeButton(btns[i], Color.red, 2.0f, 2.0f));

                    if (PhotonNetwork.PlayerList[i].CustomProperties.ContainsKey("isMafia"))
                    {
                        if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                        {
                            GameObject playerGameObject = PhotonNetwork.PlayerList[i].TagObject as GameObject;
                            playerGameObject.GetComponent<PhotonView>().RPC("Death", RpcTarget.All);
                            Debug.Log(PhotonNetwork.PlayerList[i].NickName);
                        }
                    }
                    else
                    {
                        if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                        {
                            GameObject playerGameObject = PhotonNetwork.PlayerList[i].TagObject as GameObject;
                            playerGameObject.GetComponent<PhotonView>().RPC("Death", RpcTarget.All);
                            Debug.Log(PhotonNetwork.PlayerList[i].NickName);

                        }
                    }
                    break;
                }
            }
                    StartCoroutine(FadeInRoleUI());

                            // 타이머를 0으로 설정하고 다시 시작
                            if (timer != null)
                            {
                                timer.photonView.RPC("RPC_PauseTimer", RpcTarget.All);
                                timer.photonView.RPC("RPC_StartTimer", RpcTarget.All);
                            }
        }
        else
        {
            Debug.LogError("GameTimer 태그를 가진 타이머 오브젝트를 찾을 수 없습니다.");
        }
    }

    private IEnumerator FadeInRoleUI()
    {
        CanvasGroup uiToActivate1 = null;
        if (VoteSelectPlayerNum != -1) { 
            if (PhotonNetwork.PlayerList[VoteSelectPlayerNum].CustomProperties.ContainsKey("isMafia"))
            {
                if ((bool)PhotonNetwork.PlayerList[VoteSelectPlayerNum].CustomProperties["isMafia"])
                {
                    // 닉네임과함께 마피아인지, 시민인지표시 (닉네임 인자를 받아야함.)
                    // resultMessage = PhotonNetwork.PlayerList[i].NickName+"님은 마피아 였습니다!!";
                    Debug.Log("이건됨");
                    resultMessage.text = "마피아를 찾아냈습니다!!";
                    VoteResultUI.gameObject.SetActive(true);
                    uiToActivate1 = VoteResultUI;
                }
            }
            else
            {
                resultMessage.text = "무고한 시민이 죽었습니다..";
                VoteResultUI.gameObject.SetActive(true);
                uiToActivate1 = VoteResultUI;
            }

            if (uiToActivate1 != null)
            {
                yield return StartCoroutine(FadeCanvasGroup(uiToActivate1, 0, 1, 3)); // 페이드 인을 3초 동안 수행
                    Debug.Log("요것도됨");   

                yield return new WaitForSeconds(2); // 2초 대기
                VoteUI.SetActive(false);

                yield return StartCoroutine(FadeCanvasGroup(uiToActivate1, 1, 0, 1)); // 페이드 아웃을 1초 동안 수행
                uiToActivate1.gameObject.SetActive(false);

            }
        }
        else
        {
            VoteUI.SetActive(false);
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = end;
    }

    private IEnumerator WaitAndDisableVoteUI(float delay)
    {
        yield return new WaitForSeconds(delay);
        VoteUI.SetActive(false);
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
        if (count == remainingPlayerNum)
        {
            result = true;
        }

        return result;
    }

    private IEnumerator FadeButton(Button button, Color targetColor, float fadeInDuration, float holdDuration)
    {
        Color originalColor = button.image.color;
        float timer = 0f;

        // Fade in
        while (timer < fadeInDuration)
        {
            button.image.color = Color.Lerp(originalColor, targetColor, timer / fadeInDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Hold
        button.image.color = targetColor;
        yield return new WaitForSeconds(holdDuration);

        // Fade out
        timer = 0f;
        while (timer < fadeInDuration)
        {
            button.image.color = Color.Lerp(targetColor, originalColor, timer / fadeInDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Reset color to original
        button.image.color = originalColor;
    }


}
