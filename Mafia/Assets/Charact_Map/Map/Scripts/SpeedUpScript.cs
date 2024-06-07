/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class SpeedUpScript : MonoBehaviourPun
{
    // 남은 이속 증가 아이템 개수
    [SerializeField]
    public int count = 0;

    [SerializeField]
    private float increasedSpeed = 4;

    // 아이템 얻는 기준
    private bool criterion = false;

    [SerializeField]
    private GameObject Player;

    public Image SpeedUpImage; // 스피드업 아이템 활성화 이미지

    private CoinScript coinScript;

    // Start is called before the first frame update
    void Start()
    {
        SpeedUpImage = GameObject.Find("Canvas/ItemApplyImage/SpeedUpImage").GetComponent<Image>(); // 스피드업 아이템 활성화 이미지
        SpeedUpImage.enabled = false; // 스피드업 아이템 활성화 이미지 비활성화

        // Player 오브젝트가 제대로 설정되어 있는지 확인합니다.
        if (Player == null)
        {
            Player = gameObject;
        }

        coinScript = Player.GetComponent<CoinScript>(); // CoinScript 참조 가져오기
        if (coinScript == null)
        {
            Debug.LogError("CoinScript not found on Player object");
        }
        else
        {
            Debug.Log("CoinScript successfully found on Player object");
        }
    }

    public void ActivateSpeedUp()
    {
        // 스피드업 활성화 조건 확인
        if (coinScript != null)
        {
            Debug.Log($"Current Coins: {coinScript.CoinInt}");

            if (coinScript.CoinInt >= 40)
            {
                coinScript.LostMoney(); // LostMoney 메서드 호출

                // LostMoney 호출 후 다시 코인 양을 확인
                Debug.Log($"Coins after LostMoney: {coinScript.CoinInt}");

                if (coinScript.CoinInt >= 0)
                {
                    Player.GetComponent<PlayerController>().playerMoveSpeedUnit = increasedSpeed;
                    SpeedUpImage.enabled = true;
                }
                else
                {
                    Debug.Log("Not enough coins after LostMoney");
                }
            }
            else
            {
                Debug.Log("Not enough coins to activate speed up");
            }
        }
        else
        {
            Debug.LogError("CoinScript is null");
        }
    }

    public void DeactivateSpeedUp()
    {
        if (coinScript != null)
        {
            Player.GetComponent<PlayerController>().playerMoveSpeedUnit = 1;
            SpeedUpImage.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (criterion)
        {
            count++;
        }

        *//*
        if (count > 0 && Input.GetKeyDown(KeyCode.P))
        {
            //이속 증가 함수 구현
            SpeedUp();
            count--;
        }
        *//*
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class SpeedUpScript : MonoBehaviourPun
{
    // 남은 이속 증가 아이템 개수
    [SerializeField]
    public int count = 0;

    [SerializeField]
    private float increasedSpeed = 4;

    // 아이템 얻는 기준
    private bool criterion = false;

    [SerializeField]
    private GameObject Player;

    public Image SpeedUpImage; // 스피드업 아이템 활성화 이미지

    private CoinScript coinScript;

    // Start is called before the first frame update
    void Start()
    {
        SpeedUpImage = GameObject.Find("Canvas/ItemApplyImage/SpeedUpImage").GetComponent<Image>(); // 스피드업 아이템 활성화 이미지
        SpeedUpImage.enabled = false; // 스피드업 아이템 활성화 이미지 비활성화

        // Player 오브젝트가 제대로 설정되어 있는지 확인합니다.
        if (Player == null)
        {
            Player = gameObject;
        }

        coinScript = Player.GetComponent<CoinScript>(); // CoinScript 참조 가져오기
        if (coinScript == null)
        {
            Debug.LogError("CoinScript not found on Player object");
        }
        else
        {
            Debug.Log("CoinScript successfully found on Player object");
        }
    }

    public void ActivateSpeedUp()
    {
        // 스피드업 활성화 조건 확인
        if (coinScript != null)
        {
            Debug.Log($"Current Coins: {coinScript.CoinInt}");

            if (coinScript.CoinInt >= 40)
            {
                coinScript.LostMoney(); // LostMoney 메서드 호출

                // LostMoney 호출 후 다시 코인 양을 확인
                Debug.Log($"Coins after LostMoney: {coinScript.CoinInt}");

                if (coinScript.CoinInt >= 0)
                {
                    Player.GetComponent<PlayerController>().playerMoveSpeedUnit = increasedSpeed;
                    SpeedUpImage.enabled = true;
                }
                else
                {
                    Debug.Log("Not enough coins after LostMoney");
                }
            }
            else
            {
                Debug.Log("Not enough coins to activate speed up");
            }
        }
        else
        {
            Debug.LogError("CoinScript is null");
        }
    }

    public void DeactivateSpeedUp()
    {
        if (coinScript != null)
        {
            Player.GetComponent<PlayerController>().playerMoveSpeedUnit = 1;
            SpeedUpImage.enabled = false;
        }
    }

   
}
