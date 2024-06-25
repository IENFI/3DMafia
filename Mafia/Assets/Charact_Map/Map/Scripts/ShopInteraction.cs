using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;

public class ShopInteraction : MonoBehaviourPunCallbacks
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
    public TMP_Text ShowErrorText;

    [Header("아이템목록 이미지")]
    public Image doubleCoinsImage; // 코인 2배 아이템 활성화 이미지
    public Image speedUpImage; // 스피드업 아이템 활성화 이미지

    [Header("승리모금")]
    public TMP_Text SaveCoinText; // 승리모금금액을 표시할 텍스트
    public int SaveCoin = 0;

    private bool isPlayerInRange = false; // 플레이어가 상점 주변에 있는지 여부
    private PlayerController playerController;
    

    // 버튼 변수 선언
    public Button buyDoubleCoinButton;
    public Button buySpeedUpButton;
    public Button saveMoneyButton;
    public Button deleteMoneyButton;

    private List<ShopInteraction> shopInteractions;

    private void Awake()
    {
        SaveCoinText.text = "0/1000";

        // 모든 ShopInteraction 컴포넌트를 수집
        shopInteractions = new List<ShopInteraction>(FindObjectsOfType<ShopInteraction>(true));
    }

    private void Start()
    {
        doubleCoinsImage = GameObject.Find("Canvas/ItemApplyImage/DoubleCoinImage").GetComponent<Image>();
        doubleCoinsImage.enabled = false;
        speedUpImage = GameObject.Find("Canvas/ItemApplyImage/SpeedUpImage").GetComponent<Image>();
        speedUpImage.enabled = false;

        ShopUI.SetActive(false);
        ShowErrorText.text = ""; // 에러 텍스트 초기화

        // 버튼 클릭 이벤트 설정
        buyDoubleCoinButton.onClick.AddListener(BuyDoubleCoin);
        buySpeedUpButton.onClick.AddListener(BuySpeedUp);
        saveMoneyButton.onClick.AddListener(SaveMoney);
        deleteMoneyButton.onClick.AddListener(DeleteMoney);

        /*// 이벤트 등록
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;*/
    }

    /*private void OnDestroy()
    {
        // 이벤트 등록 해제
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            player = other.GetComponent<PlayerCoinController>();
            playerController = player.GetComponent<PlayerController>();
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
            playerController = null;
            ShopUI.SetActive(false); // 플레이어가 범위를 벗어나면 ShopUI를 비활성화
        }
    }

    private void Update()
    {
        // 플레이어가 상점 주변에 있고 E 키를 누르면 상점 UI 활성화
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ShopUI.SetActive(true);
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
                ShowError("코인두배 적용완료!");

                // 60초 후에 player의 DeactivateDoubleCoin 메서드 호출
                StartCoroutine(DeactivateDoubleCoinAfterDelay(player, 60.0f));
            }
            else
            {
                ShowError("코인이 부족합니다!");
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
                ShowError("스피드업 적용완료!");

                // 60초 후에 플레이어의 속도를 원래대로 되돌리는 코루틴 호출
                StartCoroutine(ResetPlayerSpeedAfterDelay(playerController, 60.0f));
            }
            else
            {
                ShowError("코인이 부족합니다!");
                Debug.Log("Not enough coins to buy this item.");
            }
        }
    }

    public void SaveMoney()
    {
        if (player != null)
        {
            int itemCost = 100; // 모금단위

            if (player.coin >= itemCost)
            {
                player.coin -= itemCost;
                ShowError("모금완료!");
                Debug.Log($"모금완료 (잔여 Coins: {player.coin})");
                player.UpdateCoinUI();

                foreach (ShopInteraction shopInteraction in shopInteractions)
                {
                    PhotonView photonView = shopInteraction.GetComponent<PhotonView>();
                    if (photonView != null)
                    {
                        // 모든 클라이언트에서 UpdateSaveCoin RPC 호출
                        photonView.RPC("UpdateSaveCoin", RpcTarget.All, 100);
                    }
                    else
                    {
                        Debug.LogWarning("PhotonView가 ShopInteraction 오브젝트에 없습니다: " + shopInteraction.gameObject.name);
                    }
                }
            }
            else
            {
                ShowError("코인이 부족합니다!");
                Debug.Log("Not enough coins to buy this item.");
            }
        }
    }

    public void DeleteMoney()
    {
        if (player != null)
        {
            int itemCost = 100; // 모금단위

            if (player.coin >= itemCost && SaveCoin > 0)
            {
                player.coin -= itemCost;
                Debug.Log($"모금방해완료 (잔여 Coins: {player.coin})");
                player.UpdateCoinUI();

                // 각 ShopInteraction의 PhotonView를 사용하여 RPC를 호출
                foreach (ShopInteraction shopInteraction in shopInteractions)
                {
                    PhotonView photonView = shopInteraction.GetComponent<PhotonView>();
                    if (photonView != null)
                    {
                        // 모든 클라이언트에서 UpdateSaveCoin RPC 호출
                        photonView.RPC("UpdateSaveCoin", RpcTarget.All, -150);
                    }
                    else
                    {
                        Debug.LogWarning("PhotonView가 ShopInteraction 오브젝트에 없습니다: " + shopInteraction.gameObject.name);
                    }
                }
            }
            else
            {
                ShowError("코인이 부족합니다!");
                Debug.Log("Not enough coins to buy this item.");
            }
        }
    }

    // 이벤트를 수신하여 SaveCoinText를 업데이트하는 메서드
    /*private void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 0)
        {
            object[] data = (object[])photonEvent.CustomData;
            int newSaveCoin = (int)data[0];
            SaveCoinText.text = newSaveCoin.ToString() + "/1000";
        }
    }*/

    [PunRPC]
    private void UpdateSaveCoin(int coin)
    {
        Debug.Log("UpdateSaveCoin()");
        SaveCoin += coin;
        if (SaveCoin < 0) { SaveCoin = 0; }
        Debug.Log("SaveCoin : "+SaveCoin +"SaveCoin.ToString() :"+SaveCoin.ToString());
        SaveCoinText.text = SaveCoin.ToString() + "/1000";

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

    // 에러 메시지를 표시하고 일정 시간 후에 사라지게 하는 코루틴
    private void ShowError(string message)
    {
        StartCoroutine(DisplayErrorMessage(message, 2.0f));
    }

    private IEnumerator DisplayErrorMessage(string message, float delay)
    {
        ShowErrorText.text = message;
        yield return new WaitForSeconds(delay);
        ShowErrorText.text = "";
    }
}
