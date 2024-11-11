using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathAnnounce : MonoBehaviour
{
    private MafiaManager mafiaManager;
    private int currentPlayerNum;
    private int previousPlayerNum;

    public TMP_Text DeathAnnounceText;
    public float duration = 60f; // ������ ���ӽð�
    private bool isActive = false;

    GameObject deathAnnounceObj;

    void Start()
    {
        // MafiaManager ���� ������Ʈ���� MafiaManager ��ũ��Ʈ�� ã���ϴ�.
        mafiaManager = GameObject.Find("Manager/MafiaManager").GetComponent<MafiaManager>();

        // DeathAnnounce �±׸� ���� ������Ʈ���� TMP_Text ������Ʈ�� ã�� �Ҵ�
        deathAnnounceObj = GameObject.FindGameObjectWithTag("DeathAnnounce");
        if (deathAnnounceObj != null)
        {
            DeathAnnounceText = deathAnnounceObj.GetComponent<TMP_Text>();
            if (DeathAnnounceText == null)
            {
                Debug.LogError("DeathAnnounce �±׸� ���� ������Ʈ�� TMP_Text ������Ʈ�� �����ϴ�!");
            }
        }
        else
        {
            Debug.LogError("DeathAnnounce �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�!");
        }
        deathAnnounceObj.SetActive(false);
        currentPlayerNum = mafiaManager.remainingMafiaNum + mafiaManager.remainingCitizenNum;
        previousPlayerNum = currentPlayerNum;

        // DeathAnnounceText�� null�� �ƴ� ��쿡�� �ʱ�ȭ
        if (DeathAnnounceText != null)
        {
            DeathAnnounceText.text = "";
        }
    }

    public void OnActivationButtonClick()
    {
        isActive = true;
        StartMonitoring();
    }

    public void DeactivateDeathAnnounce()
    {
        isActive = false;
        DeathAnnounceText.text = "";
    }

    private void StartMonitoring()
    {
        InvokeRepeating("CheckDeaths", 0f, 1f); // 1�ʸ��� üũ
    }

    private void CheckDeaths()
    {
        if (!isActive) return;
        deathAnnounceObj.SetActive(true);

        currentPlayerNum = mafiaManager.remainingMafiaNum + mafiaManager.remainingCitizenNum;
        if (previousPlayerNum - currentPlayerNum >= 1)
        {
            DeathAnnounceText.text = "누군가 사망하였습니다.";
            Invoke("ClearDeathMessage", 10f);
        }
        previousPlayerNum = currentPlayerNum;
    }

    private void ClearDeathMessage()
    {
        if (isActive)
        {
            DeathAnnounceText.text = "";
        }
    }

    private void OnDisable()
    {
        CancelInvoke("CheckDeaths");
        DeathAnnounceText.text = "";
    }
}