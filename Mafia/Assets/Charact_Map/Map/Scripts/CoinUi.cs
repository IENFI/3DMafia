using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class CoinUi : MonoBehaviourPun
{
    public static TMP_Text coinText;

    private PlayerCoinController playerCoinController;

    private void Start()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            // 다른 플레이어의 경우 해당 UI를 비활성화합니다.
            gameObject.SetActive(false);
            return;
        }

        playerCoinController = GetComponent<PlayerCoinController>();
        UpdateCoinUI(playerCoinController.coin); // 코인 잔액으로 업데이트합니다.
    }

    private void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
            return;

        // 코인 잔액이 변경될 때마다 UI를 업데이트합니다.
        UpdateCoinUI(playerCoinController.coin); // 코인 잔액으로 업데이트합니다.
    }

    public static void UpdateCoinUI(int coin)
    {
        if (coinText != null)
        {
            coinText.text = "Coins: " + coin.ToString();
        }
    }
}
