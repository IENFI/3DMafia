using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCoinController : MonoBehaviourPunCallbacks, IPunObservable
{
    public int coin = 0;
    private bool doubleCoinActive = false;
    public TMP_Text coinText; // UI Text 요소를 참조할 변수
    public TMP_Text coinTextPrefab; // CoinText 프리팹을 참조할 변수

    void Start()
    {
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            // PhotonView가 현재 플레이어의 것이거나 오프라인 상태일 때만 코인 UI를 생성합니다.
            if (coinText == null && coinTextPrefab != null)
            {
                // CoinText 프리팹을 인스턴스화하고 할당합니다.
                coinText = Instantiate(coinTextPrefab);
            }
            if (coinText != null)
            {
                coinText.text = " " + coin;
            }
        }

    }

    void Update()
    {
        /*if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                GetCoin(60);
            }
        }*/
    }

    public void GetCoin(int amount)
    {
        if (doubleCoinActive)
        {
            amount *= 2;
        }
        coin += amount;
        Debug.Log("Coins: " + coin);

        // 코인 UI 업데이트
        UpdateCoinUI();
    }

    // 코인 UI 업데이트 메서드
    public void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = "" + coin;
        }
    }

    public void ActivateDoubleCoin()
    {
        doubleCoinActive = true;
        if (doubleCoinActive)
        {
            Debug.Log($"코인 2배 구매/적용완료 (잔여 Coins: {coin})"); 
        }
        UpdateCoinUI();
    }
    public void DeactivateDoubleCoin()
    {
        doubleCoinActive = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(coin);
        }
        else
        {
            coin = (int)stream.ReceiveNext();
        }
    }
}
