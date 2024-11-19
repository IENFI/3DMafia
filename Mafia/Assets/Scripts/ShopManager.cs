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

public class ShopManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private float increasedSpeed = 1.5f;
    private float originalSpeed = 1;

    public GameObject ShopUI;
    private PlayerCoinController player;
    public TMP_Text ShowErrorText;

    [Header("������ ���̾ƿ�")]
    [SerializeField] private Transform itemLayoutParent; // ������ �̹������� �θ� ������Ʈ

    [Header("������ �̹���")]
    public Image[] itemImages;  // ������ �̹��� �迭

    [Header("������ ��ư")]
    public Button[] itemButtons;  // ������ ��ư �迭

    [Header("������ Ÿ�̸� �ؽ�Ʈ")]
    public TMP_Text[] itemTimerTexts;  // �� ������ �̹����� �ڽ� TMP_Text

    [Header("�¸����")]
    public TMP_Text SaveCoinText;
    public int SaveCoin = 0;

    private PlayerController playerController;
    private CharacterController characterController;
    private List<ShopInteraction> shopInteractions;
    private XRayVisionItem xRayVisionItem;
    private DeathAnnounce deathAnnounceItem;
    // ������ Ȱ��ȭ ���¸� �����ϴ� ����Ʈ
    private List<(int originalIndex, GameObject imageObj)> activeItems = new List<(int, GameObject)>();
    // ���� ���� ���� Ÿ�̸� �ڷ�ƾ�� ������ Dictionary �߰�
    private Dictionary<int, Coroutine> activeTimers = new Dictionary<int, Coroutine>();
    // ������ ���� ��Ÿ�� ���� ������
    private Dictionary<int, float> lastPurchaseTime = new Dictionary<int, float>();
    private const float PURCHASE_COOLDOWN = 0.5f; // ���� ���ð� (0.5��)

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
        if (deathAnnounceItem == null)
        {
            deathAnnounceItem = GetComponent<DeathAnnounce>();
            if (deathAnnounceItem == null)
            {
                deathAnnounceItem = gameObject.AddComponent<DeathAnnounce>();
            }
        }

        // XRayVisionItem ������Ʈ �ʱ�ȭ
        xRayVisionItem = GetComponent<XRayVisionItem>();
        if (xRayVisionItem == null)
        {
            xRayVisionItem = gameObject.AddComponent<XRayVisionItem>();
        }

/*        // ��ư �̺�Ʈ ����
        if (itemButtons.Length >= 6)  // 6�� ��ư Ȯ��
        {
            itemButtons[0].onClick.AddListener(BuyDoubleCoin);
            itemButtons[1].onClick.AddListener(BuySpeedUp);
            itemButtons[2].onClick.AddListener(BuyXRayVision);  // X-Ray ���� ���� �Լ� ����
            itemButtons[3].onClick.AddListener(SaveMoney);
            itemButtons[4].onClick.AddListener(DeleteMoney);
            itemButtons[5].onClick.AddListener(BuyDeathAnnounce);
        }*/

        StartCoroutine(FindLocalPlayerCoinController());
    }

    public void OpenShopUI()
    {
        ShopUI.SetActive(true);
    }
    public void CloseShopUI()
    {
        ShopUI.SetActive(false );
    }

    private void InitializeShopItems()
    {
        // ��� ������ �̹����� Ÿ�̸� �ؽ�Ʈ ��Ȱ��ȭ
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

    // ������ ���� ���� ���� Ȯ�� �Լ�
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
    // Buy �Լ��� ����
    public void BuyXRayVision()
    {
        if (player != null  && CanPurchaseItem(2))
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
                ShowError("X-Ray ���� ����Ϸ�!");

                xRayVisionItem.SendMessage("OnActivationButtonClick");

                Coroutine newTimer = StartCoroutine(UpdateItemTimer(2, xRayVisionItem.duration));
                activeTimers[2] = newTimer;

                UpdatePurchaseTime(2);  // ���� �ð� ����
            }
            else
            {
                ShowError("������ �����մϴ�!");
            }
        }
        else
        {
            Debug.Log("��������");
        }
    }

    public void BuyDeathAnnounce()
    {
        if (player != null  && CanPurchaseItem(5)) // �ε����� 5�� ����
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
                ShowError("��� �˸� Ȱ��ȭ!");

                deathAnnounceItem.OnActivationButtonClick();

                Coroutine newTimer = StartCoroutine(UpdateItemTimer(3, deathAnnounceItem.duration, () => {
                    deathAnnounceItem.DeactivateDeathAnnounce();
                }));
                activeTimers[3] = newTimer;

                UpdatePurchaseTime(3);
            }
            else
            {
                ShowError("������ �����մϴ�!");
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
        if (player != null  && CanPurchaseItem(0))
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
                ShowError("���ει� ����Ϸ�!");

                Coroutine newTimer = StartCoroutine(UpdateItemTimer(0, 60.0f, () => player.DeactivateDoubleCoin()));
                activeTimers[0] = newTimer;

                UpdatePurchaseTime(0);  // ���� �ð� ����
            }
            else
            {
                ShowError("������ �����մϴ�!");
            }
        }
    }


    public void BuySpeedUp()
    {
        if (player != null && CanPurchaseItem(1))
        {
            int itemCost = 100;
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                originalSpeed = playerController.playerMoveSpeedUnit;

                if (player.coin >= itemCost)
                {
                    player.coin -= itemCost;
                    player.UpdateCoinUI();

                    // �̹� ���ǵ���� Ȱ��ȭ�Ǿ� �ִ��� Ȯ��
                    bool isAlreadyActive = activeTimers.ContainsKey(1);

                    if (isAlreadyActive)
                    {
                        // ���� Ÿ�̸� ����
                        StopCoroutine(activeTimers[1]);
                        activeTimers.Remove(1);
                    }
                    else
                    {
                        // ó�� Ȱ��ȭ�Ǵ� ��쿡�� �ӵ� ����
                        playerController.ChangeMoveSpeed(increasedSpeed);
                    }

                    itemImages[1].enabled = true;
                    ShowError("���ǵ�� ����Ϸ�!");

                    // ���ο� Ÿ�̸� ����
                    Coroutine newTimer = StartCoroutine(UpdateItemTimer(1, 60.0f, () =>
                    {
                        playerController.OriginMoveSpeed();
                    }));
                    activeTimers[1] = newTimer;

                    UpdatePurchaseTime(1);  // ���� �ð� ����
                }
                else
                {
                    ShowError("������ �����մϴ�!");
                }
            }
            else
            {
                Debug.LogError("PlayerController component not found on the player object.");
            }
        }
    }
    private void ReorderActiveItems()
    {
        // Ȱ��ȭ�� ��� �������� ������� ����
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
            // ���� �������� �̹� Ȱ��ȭ�Ǿ� �ִ��� Ȯ��
            var existingItemIndex = activeItems.FindIndex(item => item.originalIndex == itemIndex);

            if (existingItemIndex != -1)
            {
                // ���� �������� �����ϰ�
                activeItems.RemoveAt(existingItemIndex);
            }

            // ���ο� �������� �� �ڿ� �߰�
            itemImages[itemIndex].enabled = true;
            activeItems.Add((itemIndex, itemImages[itemIndex].gameObject));

            // ��� Ȱ��ȭ�� ������ ������
            ReorderActiveItems();
        }
        else
        {
            // ������ ��Ȱ��ȭ
            itemImages[itemIndex].enabled = false;
            activeItems.RemoveAll(item => item.originalIndex == itemIndex);
            ReorderActiveItems();
        }
    }

    // ���� �ð� ����
    private void UpdatePurchaseTime(int itemIndex)
    {
        lastPurchaseTime[itemIndex] = Time.time;
    }

    private IEnumerator UpdateItemTimer(int itemIndex, float duration, System.Action onComplete = null)
    {
        float remainingTime = duration;

        // ������ Ȱ��ȭ �� ����
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

        // Ÿ�̸� �Ϸ� �� Dictionary���� ����
        if (activeTimers.ContainsKey(itemIndex))
        {
            activeTimers.Remove(itemIndex);
        }

        // ������ ��Ȱ��ȭ �� ����
        UpdateItemActivation(itemIndex, false);

        if (itemTimerTexts[itemIndex] != null)
        {
            itemTimerTexts[itemIndex].text = "";
        }

        // SpeedUp �������� ��쿡�� ���� �ӵ��� ������
        if (itemIndex == 1 && playerController != null)
        {
            playerController.OriginMoveSpeed();
        }

        onComplete?.Invoke();
    }
    public void SaveMoney()
    {
        if (player != null )
        {
            int itemCost = 100;

            if (player.coin >= itemCost)
            {
                player.coin -= itemCost;
                ShowError("��ݿϷ�!");
                player.UpdateCoinUI();
                SaveCoin += 100;
                photonView.RPC("UpdateSaveCoin", RpcTarget.All, SaveCoin);
                
            }
            else
            {
                ShowError("������ �����մϴ�!");
            }
        }
    }

    public void DeleteMoney()
    {
        if (player != null)
        {
            int itemCost = 100;

            if (player.coin >= itemCost && SaveCoin > 0)
            {
                player.coin -= itemCost;
                player.UpdateCoinUI();
                SaveCoin -= 150;
                photonView.RPC("UpdateSaveCoin", RpcTarget.All, SaveCoin);
            }
            else if (player.coin >= itemCost)
            {
                ShowError("0�� �����δ� ���� �� �����ϴ�!!");
            }
            else
            {
                ShowError("������ �����մϴ�!");
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
        SaveCoin = coin;
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