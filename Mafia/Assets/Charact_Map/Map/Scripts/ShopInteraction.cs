using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ShopInteraction : MonoBehaviour
{
    [SerializeField] private Renderer outlineRenderer;
    public GameObject ShopUI; // 상점 UI
    private PlayerCoinController player; // 플레이어의 재화 관리 스크립트

    private bool isPlayerInRange = false; // 플레이어가 상점 주변에 있는지 여부

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
                Debug.Log("Double Coin purchased!");
                // 상점 UI 비활성화
                ShopUI.SetActive(false);
            }
            else
            {
                Debug.Log("Not enough coins to buy this item.");
            }
        }
    }
}
