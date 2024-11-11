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

    [Header("ProfileImage")]
    public Image[] profileImage;

    [Header("AvatarImage")]
    public Sprite[] avatarSpriteImages;

    [Header("Die(X)Image")]
    public Image[] xImage;

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

    public Button btn_skip;

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

    private TMP_Text btn_skipText;

    [SerializeField]
    public GameObject voteManager;

    [Header("VoteReuslt")]
    public CanvasGroup VoteResultUI; // VoteResultUI의 CanvasGroup을 설정합니다.
    public TMP_Text resultMessage;


    private Dictionary<int, TMP_Text> btnTexts;
    private Dictionary<int, Button> btns;
    private Dictionary<string, int> avatarNameToSpriteIndex;

    private Timer gameTimer;
    // VoteManager   Ȱ  ȭ Ǹ  Start      Ϸ                ϴٰ                θ    Ȱ  ȭ.

    public bool isMeetingActivated = false;

    //voteList       client 鿡       .
    private int[] voteList = new int[10]; //10    ִ   ÷  ̾    ,     list  ƴϰ  array  , 0      ʱ ȭ
    private int criterion; // ÷  ̾       ݼ 
    private int remainingPlayerNum;
    private int skiped;

    private bool VoteResultBool;
    private int VoteSelectPlayerNum = -1;

    private void Awake()
    {
        Debug.Log("[VoteManager] Initializing VoteManager...");
        try
        {
            // 아바타 이름과 스프라이트 인덱스 매핑 초기화
            avatarNameToSpriteIndex = new Dictionary<string, int>()
            {
                { "naked", 0 },
                { "builder", 1 },
                { "businessWoman", 2 },
                { "cashier", 3 },
                { "chef", 4 },
                { "fisherman", 5 },
                { "miner", 6 },
                { "nurse", 7 },
                { "police", 8 },
                { "security", 9 },
                { "worker", 10 }
            };
            Debug.Log("[VoteManager] Avatar name to sprite index mapping initialized successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"[VoteManager] Error during initialization: {e.Message}");
        }
    }
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

        btn_skipText = btn_skip.GetComponentInChildren<TMP_Text>();


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

        gameTimer = FindObjectOfType<Timer>();
        if (gameTimer != null && PhotonNetwork.IsMasterClient) // 마스터 클라이언트만 RPC 호출
        {
            gameTimer.photonView.RPC("PauseTimer", RpcTarget.All);
        }

        Debug.Log("[VoteManager] Starting meeting...");

        // 필수 컴포넌트들 검증
        if (profileImage == null || profileImage.Length == 0)
        {
            Debug.LogError("[VoteManager] Profile image array is not assigned!");
            return;
        }

        if (avatarSpriteImages == null || avatarSpriteImages.Length == 0)
        {
            Debug.LogError("[VoteManager] Avatar sprite images array is not assigned!");
            return;
        }

        if (VoteUI == null)
        {
            Debug.LogError("[VoteManager] VoteUI is not assigned!");
            return;
        }

        // 모든 검증이 통과되면 실행
        Initialize();
        UpdateProfileImages();
        VoteUI.SetActive(true);
        Debug.Log("[VoteManager] Meeting started successfully");

    }

    /*
    public void MeetingStart()
    {
        //UI   Ȱ  ȭ ->   ư       ǥ
        VoteUI.SetActive(true);
    }
    */

    private void UpdateProfileImages()
    {
        Debug.Log("[VoteManager] 프로필 이미지 업데이트 시작...");

        // 배열 유효성 검사
        if (profileImage == null || avatarSpriteImages == null)
        {
            Debug.LogError("[VoteManager] 프로필 이미지 또는 아바타 스프라이트 배열이 null입니다!");
            return;
        }

        try
        {
            // 플레이어 수와 프로필 이미지 슬롯 수 중 작은 값까지만 반복
            for (int i = 0; i < PhotonNetwork.PlayerList.Length && i < profileImage.Length; i++)
            {
                Player player = PhotonNetwork.PlayerList[i];
                Debug.Log($"[VoteManager] 플레이어 {i} 처리 중: {player.NickName}");

                // 프로필 이미지 슬롯 확인
                if (profileImage[i] == null)
                {
                    Debug.LogError($"[VoteManager] 인덱스 {i}의 프로필 이미지가 null입니다!");
                    continue;
                }

                // 플레이어의 아바타 이름 가져오기
                if (!player.CustomProperties.TryGetValue("AvatarName", out object avatarNameObj))
                {
                    Debug.LogWarning($"[VoteManager] 플레이어의 아바타 이름을 찾을 수 없습니다: {player.NickName}");
                    continue;
                }

                string avatarName = (string)avatarNameObj;
                Debug.Log($"[VoteManager] 플레이어 {player.NickName}의 아바타: {avatarName}");

                // 아바타에 해당하는 스프라이트 인덱스 가져오기
                if (!avatarNameToSpriteIndex.TryGetValue(avatarName, out int spriteIndex))
                {
                    Debug.LogWarning($"[VoteManager] 아바타 이름에 해당하는 스프라이트 인덱스를 찾을 수 없습니다: {avatarName}");
                    continue;
                }

                // 스프라이트 인덱스 범위 확인
                if (spriteIndex >= avatarSpriteImages.Length)
                {
                    Debug.LogError($"[VoteManager] 스프라이트 인덱스 {spriteIndex}가 범위를 벗어났습니다. 사용 가능한 스프라이트 수: {avatarSpriteImages.Length}");
                    continue;
                }

                // 해당 인덱스의 스프라이트 유효성 확인
                if (avatarSpriteImages[spriteIndex] == null)
                {
                    Debug.LogError($"[VoteManager] 인덱스 {spriteIndex}의 아바타 스프라이트가 null입니다!");
                    continue;
                }

                // 프로필 이미지 업데이트
                profileImage[i].sprite = avatarSpriteImages[spriteIndex];
                profileImage[i].gameObject.SetActive(true);
                Debug.Log($"[VoteManager] 플레이어 {player.NickName}의 프로필 이미지가 성공적으로 업데이트되었습니다");
            }

            // 사용하지 않는 프로필 슬롯 비활성화
            for (int i = PhotonNetwork.PlayerList.Length; i < profileImage.Length; i++)
            {
                if (profileImage[i] != null)
                {
                    profileImage[i].gameObject.SetActive(false);
                    Debug.Log($"[VoteManager] 사용하지 않는 프로필 슬롯 {i} 비활성화");
                }
            }

            Debug.Log("[VoteManager] 프로필 이미지 업데이트 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"[VoteManager] 프로필 이미지 업데이트 중 오류 발생: {e.Message}\n스택 트레이스: {e.StackTrace}");
        }
    }

    void Update()
    {
        //if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"])
        //    return;

        if (isMeetingActivated)
        {
            MeetingStart();
            isMeetingActivated = false;
            VoteResultBool = true;
            VoteSelectPlayerNum = -1;
        }

        //           ÿ ,
        // ð             ߰   ʿ 
        if ((AllVoted()) && VoteResultBool)
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
        btn_skip.GetComponent<Button>().interactable = true;

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

        skiped = 0;

        for (int i = 0; i < 10; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length)
            {
                if ((bool)PhotonNetwork.PlayerList[i].CustomProperties["isDead"])
                {
                    btns[i].GetComponent<Button>().interactable = false;
                    xImage[i].gameObject.SetActive(true);
                }
                else
                    btns[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                btns[i].GetComponent<Button>().interactable = false;
            }
        }

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"])
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                btns[i].GetComponent<Button>().interactable = false;
            }
            btn_skip.GetComponent<Button>().interactable = false;
        }

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            SendText(i, 0);
        }

        SendSkip(skiped);
    }

    public override void OnPlayerPropertiesUpdate(Player player, ExitGames.Client.Photon.Hashtable props)
    {
        try
        {
            base.OnPlayerPropertiesUpdate(player, props);

            // 플레이어나 props가 null인지 체크
            if (player == null || props == null)
            {
                Debug.LogWarning("[VoteManager] Player or props is null in OnPlayerPropertiesUpdate");
                return;
            }

            // btns Dictionary가 초기화되었는지 확인
            if (btns == null)
            {
                Debug.LogWarning("[VoteManager] Buttons dictionary is not initialized");
                return;
            }

            foreach (object key in props.Keys)
            {
                if (key.ToString().Equals("voted"))
                {
                    if (!player.CustomProperties.ContainsKey("voted"))
                    {
                        Debug.LogWarning("[VoteManager] Player does not have 'voted' property");
                        continue;
                    }

                    int num = (int)player.CustomProperties["voted"];

                    if (num >= 0)
                    {
                        voteList[num]++;

                        if (PhotonNetwork.LocalPlayer == player)
                        {
                            // 버튼 비활성화 전에 null 체크
                            for (int i = 0; i < 10; i++)
                            {
                                if (btns.ContainsKey(i) && btns[i] != null)
                                {
                                    btns[i].interactable = false;
                                }
                            }

                            if (btn_skip != null)
                            {
                                btn_skip.interactable = false;
                            }
                        }

                        if (btnTexts != null && btnTexts.ContainsKey(num))
                        {
                            SendText(num, voteList[num]);
                        }
                    }
                    else if (num == -13)  // Skip vote
                    {
                        skiped++;

                        if (PhotonNetwork.LocalPlayer == player)
                        {
                            // 버튼 비활성화 전에 null 체크
                            for (int i = 0; i < 10; i++)
                            {
                                if (btns.ContainsKey(i) && btns[i] != null)
                                {
                                    btns[i].interactable = false;
                                }
                            }

                            if (btn_skip != null)
                            {
                                btn_skip.interactable = false;
                            }
                        }

                        SendSkip(skiped);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[VoteManager] Error in OnPlayerPropertiesUpdate: {e.Message}\nStack trace: {e.StackTrace}");
        }
    }

    private void SendText(int i, int voteCount)
    {
        btnTexts[i].text = PhotonNetwork.PlayerList[i].NickName + "\n" + voteCount.ToString();
    }

    private void SendSkip(int num)
    {
        btn_skipText.text = "SKIP!!" + "\n" + num.ToString();
    }

    #region Voting functions

    public void Skip()
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "voted" , -13 }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

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

                    if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                    {
                        GameObject playerGameObject = PhotonNetwork.PlayerList[i].TagObject as GameObject;
                        playerGameObject.GetComponent<PhotonView>().RPC("Death", RpcTarget.All);
                        Debug.Log(PhotonNetwork.PlayerList[i].NickName);
                    }
                    break;
                }
            }
            StartCoroutine(FadeInRoleUI());

            int LivingMan = -1;
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (!(bool)PhotonNetwork.PlayerList[i].CustomProperties["isDead"])
                {
                    LivingMan = i;
                    break;
                }
            }


        }

    }

    private IEnumerator FadeInRoleUI()
    {
        CanvasGroup uiToActivate1 = null;
        if (VoteSelectPlayerNum != -1)
        {
            if ((bool)PhotonNetwork.PlayerList[VoteSelectPlayerNum].CustomProperties["isMafia"])
            {
                resultMessage.text = "마피아를 찾아냈습니다!!";
                resultMessage.color = Color.red;
            }
            else
            {
                resultMessage.text = "무고한 시민이 죽었습니다..";
                resultMessage.color = Color.green;
            }
        }
        else
        {
            resultMessage.text = "아무도 투표로 지목되지 않았습니다";
            resultMessage.color = Color.yellow;
        }

        VoteResultUI.gameObject.SetActive(true);
        uiToActivate1 = VoteResultUI;
        if (uiToActivate1 != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(uiToActivate1, 0, 1, 3)); // 페이드 인을 3초 동안 수행
            yield return new WaitForSeconds(2); // 2초 대기
            VoteUI.SetActive(false);

            // 모든 클라이언트에게 시체 제거 RPC 호출
            GameObject playerObject = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            PhotonView playerPhotonView = playerObject.GetComponent<PhotonView>();
            playerPhotonView.RPC("DisableAllCorpses", RpcTarget.All);

            // 마스터 클라이언트만 타이머 재시작 RPC 호출
            if (PhotonNetwork.IsMasterClient && gameTimer != null)
            {
                gameTimer.photonView.RPC("ForceRestartTimer", RpcTarget.All);
            }

            yield return StartCoroutine(FadeCanvasGroup(uiToActivate1, 1, 0, 1)); // 페이드 아웃을 1초 동안 수행
            uiToActivate1.gameObject.SetActive(false);

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
        count += skiped;
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