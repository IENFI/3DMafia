using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinScript : MonoBehaviourPun
{
    public int CoinInt; // 코인이 저장되는 변수
    public TMP_Text CoinText; // 코인을 표시할 오브젝트
    public GameObject Message; // 메시지 오브젝트
    public TMP_Text MSG; // 메시지 내용
    public Image doubleCoinsImage; // 코인 2배 아이템 활성화 이미지

    private float Timer; // 타이머
    private bool TimeSet; // 타이머 작동여부
    private bool IsDoubleCoins; // 코인 2배 여부

    [SerializeField]
    private GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        CoinText = GameObject.FindWithTag("CoinText").GetComponent<TMP_Text>();
        Message = GameObject.Find("Canvas/CoinTest/Message").gameObject; // Message는 플레이어 객체 내부에 있는 Message 오브젝트입니다.
        MSG = Message.transform.Find("Text").GetComponent<TMP_Text>();
        doubleCoinsImage = GameObject.Find("Canvas/CoinTest/ItemApplyImage/DoubleCoinImage").GetComponent<Image>(); // 코인 2배 아이템 활성화 이미지

        CoinInt = PlayerPrefs.GetInt("Coin_" + gameObject.name, 0); // PlayerPrefs 내에 저장되어있는 'Coin'을 불러와 CoinInt에 저장합니다. 만약에 저장된 정보가 없다면 0을 저장합니다.
        Message.SetActive(false);
        IsDoubleCoins = false; // 초기 상태에서는 코인 2배가 아님
        doubleCoinsImage.enabled = false; // 코인 2배 아이템 활성화 이미지 비활성화
    }

    // Update is called once per frame
    void Update()
    {
        //if (!Player.GetComponent<PhotonView>().IsMine) return;

        PlayerPrefs.SetInt("Coin_" + gameObject.name, CoinInt); // CoinInt를 PlayerPrefs 내에 저장되어있는 'Coin'에 저장합니다.
        CoinText.text = CoinInt.ToString(); // CoinText의 Text에 CoinInt를 출력합니다.

        if (TimeSet == true) // TimeSet이 True면
        {
            Timer += Time.deltaTime; // 타이머가 작동합니다.
            if (Timer > 2.0f) // 2초가 지나면
            {
                Message.SetActive(false);
                MSG.text = null;
                Timer = 0;
                TimeSet = false;
            }
        }
    }

    public void GetMoney() // 돈을 얻습니다.
    {
        int amount = IsDoubleCoins ? 80 : 40; // 코인 2배일 경우 80, 아니면 40
        CoinInt += amount;
        Debug.Log("Get coin");
    }

    public void lostMoney() // 돈을 잃습니다.
    {
        if (CoinInt >= 40) // CoinInt가 40이상이라면
        {
            CoinInt -= 40; // CoinInt가 40 줄어듭니다.
            Debug.Log("lost coin");
        }
        else // 만약에 부족하다면
        {
            Message.SetActive(true); // 메시지 오브젝트를 활성화합니다.
            MSG.text = "돈이 부족합니다!!".ToString(); // MSG의 Text를 "돈이 부족합니다"로 출력합니다.
            TimeSet = true; // TimeSet를 true로 합니다.
        }
    }

    public void ActivateDoubleCoins() // 코인 2배 아이템 사용
    {
        if (CoinInt >= 40) // CoinInt가 40이상이라면
        {
            lostMoney();
            IsDoubleCoins = true;
            doubleCoinsImage.enabled = true; // 코인 2배 아이템 활성화 이미지 활성화
        }
        else // 만약에 부족하다면
        {
            Message.SetActive(true); // 메시지 오브젝트를 활성화합니다.
            MSG.text = "돈이 부족합니다!!".ToString(); // MSG의 Text를 "돈이 부족합니다"로 출력합니다.
            TimeSet = true; // TimeSet를 true로 합니다.
        }
        
    }

    public void DeactivateDoubleCoins() // 코인 2배 효과 해제
    {
        IsDoubleCoins = false;
        doubleCoinsImage.enabled = false; // 코인 2배 아이템 활성화 이미지 비활성화
    }
}
