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

    // Start is called before the first frame update
    void Start()
    {
        SpeedUpImage = GameObject.Find("Canvas/CoinTest/ItemApplyImage/SpeedUpImage").GetComponent<Image>(); // 코인 2배 아이템 활성화 이미지
        SpeedUpImage.enabled = false; // 스피드업 아이템 활성화 이미지 비활성화
    }

    public void ActivateSpeedUp()
    {
        Player.GetComponent<PlayerController>().playerMoveSpeedUnit = increasedSpeed;
        SpeedUpImage.enabled = true;
    }

    public void DeactivateSpeedUp()
    {
        Player.GetComponent<PlayerController>().playerMoveSpeedUnit = 1;
        SpeedUpImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (criterion)
        {
            count++;
        }

        /*
        if (count > 0 && Input.GetKeyDown(KeyCode.P))
        {
            //이속 증가 함수 구현
            SpeedUp();
            count--;
        }
        */
    }
}
