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
    public float duration = 60f; // 아이템 지속시간
    private bool isActive = false;

    void Start()
    {
        // MafiaManager 게임 오브젝트에서 MafiaManager 스크립트를 찾습니다.
        mafiaManager = GameObject.Find("Manager/MafiaManager").GetComponent<MafiaManager>();

        // DeathAnnounce 태그를 가진 오브젝트에서 TMP_Text 컴포넌트를 찾아 할당
        GameObject deathAnnounceObj = GameObject.FindGameObjectWithTag("DeathAnnounce");
        if (deathAnnounceObj != null)
        {
            DeathAnnounceText = deathAnnounceObj.GetComponent<TMP_Text>();
            if (DeathAnnounceText == null)
            {
                Debug.LogError("DeathAnnounce 태그를 가진 오브젝트에 TMP_Text 컴포넌트가 없습니다!");
            }
        }
        else
        {
            Debug.LogError("DeathAnnounce 태그를 가진 오브젝트를 찾을 수 없습니다!");
        }

        currentPlayerNum = mafiaManager.remainingMafiaNum + mafiaManager.remainingCitizenNum;
        previousPlayerNum = currentPlayerNum;

        // DeathAnnounceText가 null이 아닌 경우에만 초기화
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
        InvokeRepeating("CheckDeaths", 0f, 1f); // 1초마다 체크
    }

    private void CheckDeaths()
    {
        if (!isActive) return;

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