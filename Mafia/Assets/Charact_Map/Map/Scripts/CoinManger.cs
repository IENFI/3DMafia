using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }
    public int SaveCoin = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddSaveCoin(int amount)
    {
        SaveCoin += amount;
        if (SaveCoin < 0) SaveCoin = 0;

        // 승리 모금 금액이 변경될 때 이벤트 발생
        RaiseSaveCoinEvent();
    }

    private void RaiseSaveCoinEvent()
    {
        object[] content = new object[] { SaveCoin }; // 전송할 데이터
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // 모든 클라이언트에게 전송
        SendOptions sendOptions = new SendOptions { Reliability = true };

        PhotonNetwork.RaiseEvent(0, content, raiseEventOptions, sendOptions);
    }
}
