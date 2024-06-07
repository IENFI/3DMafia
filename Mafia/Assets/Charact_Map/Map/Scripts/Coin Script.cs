using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinScript : MonoBehaviourPunCallbacks
{
    public int CoinInt; // 코인이 저장되는 변수
    public TMP_Text CoinTextPrefab; // 코인을 표시할 오브젝트 프리팹
    public TMP_Text CoinTextInstance; // 코인을 표시할 오브젝트 인스턴스
    public GameObject Message; // 메시지 오브젝트
    public TMP_Text MSG; // 메시지 내용
    public Image doubleCoinsImage; // 코인 2배 아이템 활성화 이미지

    private float Timer; // 타이머
    private bool TimeSet; // 타이머 작동여부
    private bool IsDoubleCoins; // 코인 2배 여부

    [SerializeField]
    private GameObject Player;

    private PhotonView photonView;

    void Awake()
    {
        // 필요한 오브젝트들을 초기화
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        // 필요한 오브젝트들을 초기화
        Message = GameObject.Find("Canvas/CoinTest/Message").gameObject;
        MSG = Message.transform.Find("Text").GetComponent<TMP_Text>();
        doubleCoinsImage = GameObject.Find("Canvas/ItemApplyImage/DoubleCoinImage").GetComponent<Image>();

        Message.SetActive(false);
        IsDoubleCoins = false;
        doubleCoinsImage.enabled = false;

        if (photonView == null)
        {
            Debug.LogError("PhotonView is null");
            return;
        }

        if (photonView.IsMine)
        {
            // 자신의 코인 상태를 로드
            CoinInt = PlayerPrefs.GetInt("Coin_" + PhotonNetwork.LocalPlayer.UserId, 0);
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "CoinInt", CoinInt } });
            Debug.Log("CoinInt loaded and set: " + CoinInt);
        }
        else
        {
            CoinInt = (int)PhotonNetwork.LocalPlayer.CustomProperties["CoinInt"];
            Debug.Log("CoinInt from other player: " + CoinInt);
        }
        UpdateCoinText();
    }

    void Update()
    {
        if (photonView == null || !photonView.IsMine) return;

        PlayerPrefs.SetInt("Coin_" + PhotonNetwork.LocalPlayer.UserId, CoinInt);
        UpdateCoinText();

        if (TimeSet)
        {
            Timer += Time.deltaTime;
            if (Timer > 2.0f)
            {
                Message.SetActive(false);
                MSG.text = null;
                Timer = 0;
                TimeSet = false;
            }
        }
    }

    private void UpdateCoinText()
    {
        if (CoinTextInstance != null)
        {
            CoinTextInstance.text = CoinInt.ToString();
        }
    }

    public void GetMoney()
    {
        if (photonView == null || !photonView.IsMine) return;

        int amount = IsDoubleCoins ? 80 : 40;
        CoinInt += amount;
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "CoinInt", CoinInt } });
        Debug.Log("Get coin. New CoinInt: " + CoinInt);
    }

    public void LostMoney()
    {
        if (photonView == null || !photonView.IsMine) return;

        if (CoinInt >= 40)
        {
            CoinInt -= 40;
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "CoinInt", CoinInt } });
            Debug.Log("Lost coin. New CoinInt: " + CoinInt);
        }
        else
        {
            ShowMessage("돈이 부족합니다!!");
        }
    }

    public void ActivateDoubleCoins()
    {
        if (photonView == null || !photonView.IsMine) return;

        if (CoinInt >= 40)
        {
            IsDoubleCoins = true;
            doubleCoinsImage.enabled = true;
            LostMoney();
            Debug.Log("Double coins activated.");
        }
        else
        {
            Debug.Log("Not enough coins to activate double coins.");
        }
    }

    public void DeactivateDoubleCoins()
    {
        if (photonView == null || !photonView.IsMine) return;

        IsDoubleCoins = false;
        doubleCoinsImage.enabled = false;
        Debug.Log("Double coins deactivated.");
    }

    private void ShowMessage(string message)
    {
        Message.SetActive(true);
        MSG.text = message;
        TimeSet = true;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey("CoinInt"))
        {
            int newCoinInt = (int)changedProps["CoinInt"];
            if (targetPlayer == PhotonNetwork.LocalPlayer)
            {
                CoinInt = newCoinInt;
                UpdateCoinText();
                Debug.Log("CoinInt updated for local player: " + CoinInt);
            }
            else
            {
                // 다른 플레이어의 코인 상태를 업데이트
                TMP_Text otherPlayerCoinText = Instantiate(CoinTextPrefab, GameObject.FindWithTag("Canvas").transform);
                otherPlayerCoinText.text = newCoinInt.ToString();
                // 위치나 스타일을 조정하는 코드 추가 가능
                Debug.Log("CoinInt updated for other player: " + newCoinInt);
            }
        }
    }
}
