using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoinScript : MonoBehaviour
{
    public static int CoinInt; // 코인이 저장되는 변수
    public static TMP_Text CoinText; // 코인을 표시할 오브젝트
    public static GameObject Message; // 메시지 오브젝트
    public static TMP_Text MSG; // 메시지 내용

    public static float Timer; // 타이머
    public static bool TimeSet; // 타이머 작동여부
    // Start is called before the first frame update
    void Start()
    {
        CoinText = GameObject.Find("Canvas").transform.Find("CoinTest").transform.Find("Coin").transform.Find("Text").gameObject.GetComponent<TMP_Text>(); // CoinText는 Canvas/Coin/Text에 있는 Text입니다.

        Message = GameObject.Find("Canvas").transform.Find("CoinTest").transform.Find("Message").gameObject; // Message는 Canvas안에 있는 Message 란 오브젝트입니다.
        MSG = GameObject.Find("Canvas").transform.Find("CoinTest").transform.Find("Message").transform.Find("Text").gameObject.GetComponent<TMP_Text>();


        CoinInt = PlayerPrefs.GetInt("Coin", 0); // PlayerPrefs 내에 저장되어있는 'Coin'을 불러와 CoinInt에 저장합니다. 만약에 저장된 정보가 없다면 0을 저장합니다.

        Message.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.SetInt("Coin", CoinInt); // CoinInt를 PlayerPrefs 내에  저장되어있는 'Coin'에 저장합니다.

        CoinText.text = CoinInt.ToString(); //CoinText의 Text에 CoinInt를 출력합니다.

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

    public void GetMoney() //돈을 얻습니다.
    {
        CoinInt += 40;
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
}
