using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class ShopInteraction : MonoBehaviourPun
{

    // 스피드업 아이템 관련
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private float increasedSpeed = 4;
    private float originalSpeed;


    [SerializeField] private Renderer outlineRenderer;
    public GameObject ShopUI; // 상점 UI
    private PlayerCoinController player; // 플레이어의 재화 관리 스크립트
    public Image doubleCoinsImage; // 코인 2배 아이템 활성화 이미지
    public Image speedUpImage; // 코인 2배 아이템 활성화 이미지

    private bool isPlayerInRange = false; // 플레이어가 상점 주변에 있는지 여부

    void Start()
    {
        doubleCoinsImage = GameObject.Find("Canvas/ItemApplyImage/DoubleCoinImage").GetComponent<Image>();
        doubleCoinsImage.enabled = false;
        speedUpImage = GameObject.Find("Canvas/ItemApplyImage/SpeedUpImage").GetComponent<Image>();
        speedUpImage.enabled = false;

        ShopUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = other.GetComponent<PlayerCoinController>();
            outlineRenderer.material.color = Color.blue; // 외곽을 파란색으로 변경
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            outlineRenderer.material.color = Color.white; // 외곽을 기본 색상으로 변경
            player = null;
        }
    }

    private void Update()
    {
        // 플레이어가 상점 주변에 있고 G 키를 누르면 상점 UI 활성화
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.G))
        {
            ShopUI.SetActive(true);
            // 다른 상호작용 방지를 위해 플레이어 조작 비활성화
            // 예: 캐릭터 이동, 카메라 회전 등
        }
    }

    // 더블 코인 구매 버튼 클릭 시 호출될 메서드
    public void BuyDoubleCoin()
    {
        if (player != null)
        {
            int itemCost = 100; // 더블 코인 아이템의 가격

            if (player.coin >= itemCost)
            {
                player.coin -= itemCost;
                player.ActivateDoubleCoin();
                doubleCoinsImage.enabled = true;

                // 10초 후에 player의 DeactivateDoubleCoin 메서드 호출
                StartCoroutine(DeactivateDoubleCoinAfterDelay(player, 10.0f));

                // 상점 UI 비활성화
            }
            else
            {
                Debug.Log("Not enough coins to buy this item.");
            }
        }
    }

    // 스피드업 아이템 구매 버튼 클릭 시 호출될 메서드
    public void BuySpeedUp()
    {
        if (player != null)
        {
            int itemCost = 100; // 스피드업 아이템의 가격
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (player.coin >= itemCost)
            {
                player.coin -= itemCost;
                Debug.Log($"스피드업 구매/적용완료 (잔여 Coins: {player.coin})");
                player.UpdateCoinUI();
                playerController.ChangeMoveSpeed();

                speedUpImage.enabled = true; // 스피드업 이미지 활성화

                // 10초 후에 플레이어의 속도를 원래대로 되돌리는 코루틴 호출
                StartCoroutine(ResetPlayerSpeedAfterDelay(playerController, 10.0f));

            }
            else
            {
                Debug.Log("Not enough coins to buy this item.");
            }
        }
    }

    // 코루틴을 사용하여 일정 시간 후에 메서드를 호출
    private IEnumerator DeactivateDoubleCoinAfterDelay(PlayerCoinController player, float delay)
    {
        yield return new WaitForSeconds(delay);
        player.DeactivateDoubleCoin();
        doubleCoinsImage.enabled = false; // 코인 두배 아이콘 비활성화
    }

    // 10초 후에 플레이어의 속도를 원래대로 되돌리는 코루틴
    private IEnumerator ResetPlayerSpeedAfterDelay(PlayerController playerController, float delay)
    {
        yield return new WaitForSeconds(delay);
        playerController.OriginMoveSpeed(); // 플레이어의 속도를 원래대로 되돌림
        speedUpImage.enabled = false; // 스피드업 이미지 비활성화
    }
}
