using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;
using System.Linq;

public class ShopInteraction : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private float increasedSpeed = 2;
    private float originalSpeed = 2;

    [SerializeField] private Renderer outlineRenderer;
    public GameObject ShopUI;
    private PlayerCoinController player;
    public TMP_Text ShowErrorText;
    private bool isInteracting = false;  // 현재 상호작용 중인지 여부

    [Header("아이템 레이아웃")]
    [SerializeField] private Transform itemLayoutParent; // 아이템 이미지들의 부모 오브젝트

    [Header("아이템 이미지")]
    public Image[] itemImages;  // 아이템 이미지 배열

    [Header("아이템 버튼")]
    public Button[] itemButtons;  // 아이템 버튼 배열

    [Header("아이템 타이머 텍스트")]
    public TMP_Text[] itemTimerTexts;  // 각 아이템 이미지의 자식 TMP_Text

    [Header("승리모금")]
    public TMP_Text SaveCoinText;
    public int SaveCoin = 0;

    private PlayerController playerController;
    private CharacterController characterController;
    private List<ShopInteraction> shopInteractions;
    private XRayVisionItem xRayVisionItem;
    private DeathAnnounce deathAnnounceItem;
    // 아이템 활성화 상태를 추적하는 리스트
    private List<(int originalIndex, GameObject imageObj)> activeItems = new List<(int, GameObject)>();
    // 현재 실행 중인 타이머 코루틴을 저장할 Dictionary 추가
    private Dictionary<int, Coroutine> activeTimers = new Dictionary<int, Coroutine>();
    // 아이템 구매 쿨타임 관련 변수들
    private Dictionary<int, float> lastPurchaseTime = new Dictionary<int, float>();
    private const float PURCHASE_COOLDOWN = 0.5f; // 구매 대기시간 (0.5초)

    private void Awake()
    {
        SaveCoinText.text = "0/1000";
        shopInteractions = new List<ShopInteraction>(FindObjectsOfType<ShopInteraction>(true));
    }

    private void Start()
    {
        InitializeShopItems();
        ShopUI.SetActive(false);
        ShowErrorText.text = "";

        // XRayVisionItem 컴포넌트 초기화
        xRayVisionItem = GetComponent<XRayVisionItem>();
        if (xRayVisionItem == null)
        {
            xRayVisionItem = gameObject.AddComponent<XRayVisionItem>();
        }

        // 버튼 이벤트 설정
        if (itemButtons.Length >= 6)  // 6개 버튼 확인
        {
            itemButtons[0].onClick.AddListener(BuyDoubleCoin);
            itemButtons[1].onClick.AddListener(BuySpeedUp);
            itemButtons[2].onClick.AddListener(BuyXRayVision);  // X-Ray 비전 구매 함수 연결
            itemButtons[3].onClick.AddListener(SaveMoney);
            itemButtons[4].onClick.AddListener(DeleteMoney);
            itemButtons[5].onClick.AddListener(BuyDeathAnnounce);
        }

        StartCoroutine(FindLocalPlayerCoinController());
    }

    private void InitializeShopItems()
    {
        // 모든 아이템 이미지와 타이머 텍스트 비활성화
        for (int i = 0; i < itemImages.Length; i++)
        {
            if (itemImages[i] != null)
            {
                itemImages[i].enabled = false;
                if (itemTimerTexts[i] != null)
                {
                    itemTimerTexts[i].text = "";
                }
            }
        }
    }

    // 아이템 구매 가능 여부 확인 함수
    private bool CanPurchaseItem(int itemIndex)
    {
        if (!lastPurchaseTime.ContainsKey(itemIndex))
        {
            lastPurchaseTime[itemIndex] = -PURCHASE_COOLDOWN;
            return true;
        }

        if (Time.time - lastPurchaseTime[itemIndex] < PURCHASE_COOLDOWN)
        {
            return false;
        }

        return true;
    }
    // Buy 함수들 수정
    public void BuyXRayVision()
    {
        if (player != null && isInteracting && CanPurchaseItem(2))
        {
            int itemCost = 100;

            if (player.coin >= itemCost)
            {
                if (xRayVisionItem == null)
                {
                    xRayVisionItem = GetComponent<XRayVisionItem>();
                    if (xRayVisionItem == null)
                    {
                        xRayVisionItem = gameObject.AddComponent<XRayVisionItem>();
                    }
                }

                player.coin -= itemCost;
                player.UpdateCoinUI();

                if (activeTimers.ContainsKey(2))
                {
                    StopCoroutine(activeTimers[2]);
                    activeTimers.Remove(2);
                }

                itemImages[2].enabled = true;
                ShowError("X-Ray 비전 적용완료!");

                xRayVisionItem.SendMessage("OnActivationButtonClick");

                Coroutine newTimer = StartCoroutine(UpdateItemTimer(2, xRayVisionItem.duration));
                activeTimers[2] = newTimer;

                UpdatePurchaseTime(2);  // 구매 시간 갱신
            }
            else
            {
                ShowError("코인이 부족합니다!");
            }
        }
    }

    public void BuyDeathAnnounce()
    {
        if (player != null && isInteracting && CanPurchaseItem(5)) // 인덱스를 5로 변경
        {
            int itemCost = 100;
            if (player.coin >= itemCost)
            {
                if (deathAnnounceItem == null)
                {
                    deathAnnounceItem = GetComponent<DeathAnnounce>();
                    if (deathAnnounceItem == null)
                    {
                        deathAnnounceItem = gameObject.AddComponent<DeathAnnounce>();
                    }
                }

                player.coin -= itemCost;
                player.UpdateCoinUI();

                if (activeTimers.ContainsKey(3))
                {
                    StopCoroutine(activeTimers[3]);
                    activeTimers.Remove(3);
                    deathAnnounceItem.DeactivateDeathAnnounce();
                }

                itemImages[3].enabled = true;
                ShowError("사망 알림 활성화!");

                deathAnnounceItem.OnActivationButtonClick();

                Coroutine newTimer = StartCoroutine(UpdateItemTimer(3, deathAnnounceItem.duration, () => {
                    deathAnnounceItem.DeactivateDeathAnnounce();
                }));
                activeTimers[3] = newTimer;

                UpdatePurchaseTime(3);
            }
            else
            {
                ShowError("코인이 부족합니다!");
            }
        }
    }

    private IEnumerator DeactivateXRayAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        itemImages[2].enabled = false;
    }

    public void BuyDoubleCoin()
    {
        if (player != null && isInteracting && CanPurchaseItem(0))
        {
            int itemCost = 100;

            if (player.coin >= itemCost)
            {
                player.coin -= itemCost;

                if (activeTimers.ContainsKey(0))
                {
                    StopCoroutine(activeTimers[0]);
                    activeTimers.Remove(0);
                    player.DeactivateDoubleCoin();
                }

                player.ActivateDoubleCoin();
                itemImages[0].enabled = true;
                ShowError("코인두배 적용완료!");

                Coroutine newTimer = StartCoroutine(UpdateItemTimer(0, 60.0f, () => player.DeactivateDoubleCoin()));
                activeTimers[0] = newTimer;

                UpdatePurchaseTime(0);  // 구매 시간 갱신
            }
            else
            {
                ShowError("코인이 부족합니다!");
            }
        }
    }


    public void BuySpeedUp()
    {
        if (player != null && isInteracting && CanPurchaseItem(1))
        {
            int itemCost = 100;
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (player.coin >= itemCost)
            {
                player.coin -= itemCost;
                player.UpdateCoinUI();

                // 이미 스피드업이 활성화되어 있는지 확인
                bool isAlreadyActive = activeTimers.ContainsKey(1);

                if (activeTimers.ContainsKey(1))
                {
                    // 기존 타이머만 중지
                    StopCoroutine(activeTimers[1]);
                    activeTimers.Remove(1);
                    // 이미 활성화된 상태라면 속도는 변경하지 않음
                }
                else
                {
                    // 처음 활성화되는 경우에만 속도 변경
                    playerController.ChangeMoveSpeed(increasedSpeed);
                }

                itemImages[1].enabled = true;
                ShowError("스피드업 적용완료!");

                // 새로운 타이머 시작
                Coroutine newTimer = StartCoroutine(UpdateItemTimer(1, 60.0f, () => {
                    playerController.OriginMoveSpeed(originalSpeed);
                }));
                activeTimers[1] = newTimer;

                UpdatePurchaseTime(1);  // 구매 시간 갱신
            }
            else
            {
                ShowError("코인이 부족합니다!");
            }
        }
    }

    private void ReorderActiveItems()
    {
        // 활성화된 모든 아이템을 순서대로 정렬
        for (int i = 0; i < activeItems.Count; i++)
        {
            if (activeItems[i].imageObj != null)
            {
                activeItems[i].imageObj.transform.SetSiblingIndex(i);
            }
        }
    }
    private void UpdateItemActivation(int itemIndex, bool activate)
    {
        if (activate)
        {
            // 같은 아이템이 이미 활성화되어 있는지 확인
            var existingItemIndex = activeItems.FindIndex(item => item.originalIndex == itemIndex);

            if (existingItemIndex != -1)
            {
                // 기존 아이템을 제거하고
                activeItems.RemoveAt(existingItemIndex);
            }

            // 새로운 아이템을 맨 뒤에 추가
            itemImages[itemIndex].enabled = true;
            activeItems.Add((itemIndex, itemImages[itemIndex].gameObject));

            // 모든 활성화된 아이템 재정렬
            ReorderActiveItems();
        }
        else
        {
            // 아이템 비활성화
            itemImages[itemIndex].enabled = false;
            activeItems.RemoveAll(item => item.originalIndex == itemIndex);
            ReorderActiveItems();
        }
    }



    public void SetShopUIActive(bool isActive, bool isLocalPlayer)
    {
        // 로컬 플레이어의 요청일 때만 UI를 활성화/비활성화
        if (isLocalPlayer)
        {
            ShopUI.SetActive(isActive);
            isInteracting = isActive;
        }
    }

    // 구매 시간 갱신
    private void UpdatePurchaseTime(int itemIndex)
    {
        lastPurchaseTime[itemIndex] = Time.time;
    }

    private IEnumerator UpdateItemTimer(int itemIndex, float duration, System.Action onComplete = null)
    {
        float remainingTime = duration;

        // 아이템 활성화 및 정렬
        UpdateItemActivation(itemIndex, true);

        while (remainingTime > 0)
        {
            if (itemTimerTexts[itemIndex] != null)
            {
                itemTimerTexts[itemIndex].text = Mathf.CeilToInt(remainingTime).ToString();
            }
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        // 타이머 완료 시 Dictionary에서 제거
        if (activeTimers.ContainsKey(itemIndex))
        {
            activeTimers.Remove(itemIndex);
        }

        // 아이템 비활성화 및 정렬
        UpdateItemActivation(itemIndex, false);

        if (itemTimerTexts[itemIndex] != null)
        {
            itemTimerTexts[itemIndex].text = "";
        }

        onComplete?.Invoke();
    }
    public void SaveMoney()
    {
        if (player != null && isInteracting)
        {
            int itemCost = 100;

            if (player.coin >= itemCost)
            {
                player.coin -= itemCost;
                ShowError("모금완료!");
                player.UpdateCoinUI();

                foreach (ShopInteraction shop in shopInteractions)
                {
                    PhotonView photonView = shop.GetComponent<PhotonView>();
                    if (photonView != null)
                    {
                        photonView.RPC("UpdateSaveCoin", RpcTarget.All, 100);
                    }
                }
            }
            else
            {
                ShowError("코인이 부족합니다!");
            }
        }
    }

    public void DeleteMoney()
    {
        if (player != null && isInteracting)
        {
            int itemCost = 100;

            if (player.coin >= itemCost && SaveCoin > 0)
            {
                player.coin -= itemCost;
                player.UpdateCoinUI();

                foreach (ShopInteraction shop in shopInteractions)
                {
                    PhotonView photonView = shop.GetComponent<PhotonView>();
                    if (photonView != null)
                    {
                        photonView.RPC("UpdateSaveCoin", RpcTarget.All, -150);
                    }
                }
            }
            else
            {
                ShowError("코인이 부족합니다!");
            }
        }
    }

    private IEnumerator DeactivateDoubleCoinAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.DeactivateDoubleCoin();
        itemImages[0].enabled = false;
    }

    private IEnumerator ResetPlayerSpeedAfterDelay(PlayerController playerController, float delay)
    {
        yield return new WaitForSeconds(delay);
        playerController.OriginMoveSpeed();
        itemImages[1].enabled = false;
    }

    [PunRPC]
    private void UpdateSaveCoin(int coin)
    {
        SaveCoin += coin;
        if (SaveCoin < 0) { SaveCoin = 0; }
        SaveCoinText.text = SaveCoin.ToString() + "/1000";
    }

    private IEnumerator FindLocalPlayerCoinController()
    {
        while (player == null)
        {
            foreach (var pcc in FindObjectsOfType<PlayerCoinController>())
            {
                if (pcc.photonView.IsMine)
                {
                    player = pcc;
                    break;
                }
            }
            yield return null;
        }
    }

    public void ChangeOutlineRenderer(Color color)
    {
        outlineRenderer.material.color = color;
    }

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