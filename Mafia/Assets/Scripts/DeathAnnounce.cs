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
    public float duration = 30f; // 알림의 지속 시간
    private bool isActive = false;

    GameObject deathAnnounceObj;

    void Start()
    {
        // MafiaManager 오브젝트에서 MafiaManager 스크립트를 찾음
        mafiaManager = GameObject.Find("Manager/MafiaManager").GetComponent<MafiaManager>();

        // DeathAnnounce 태그가 붙은 오브젝트에서 TMP_Text 컴포넌트를 찾아 할당
        deathAnnounceObj = GameObject.Find("GameUiManager/Canvas/DeathAnnounceText");
        if (deathAnnounceObj != null)
        {
            DeathAnnounceText = deathAnnounceObj.GetComponent<TMP_Text>();
            if (DeathAnnounceText == null)
            {
                Debug.LogError("DeathAnnounce 태그가 붙은 오브젝트에 TMP_Text 컴포넌트가 없습니다!");
            }
        }
        else
        {
            Debug.LogError("DeathAnnounce 태그가 붙은 오브젝트를 찾을 수 없습니다!");
        }
        deathAnnounceObj.SetActive(false); // DeathAnnounce 오브젝트를 비활성화
        currentPlayerNum = mafiaManager.remainingMafiaNum + mafiaManager.remainingCitizenNum;
        previousPlayerNum = currentPlayerNum;

        // DeathAnnounceText가 null이 아닐 경우 초기화
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
        InvokeRepeating("CheckDeaths", 0f, 1f); // 1초 간격으로 사망 여부 체크
    }

    private void CheckDeaths()
    {
        if (!isActive) return;
        deathAnnounceObj.SetActive(true);

        currentPlayerNum = mafiaManager.remainingMafiaNum + mafiaManager.remainingCitizenNum;
        if (previousPlayerNum - currentPlayerNum >= 1)
        {
            DeathAnnounceText.text = "누군가 사망하였습니다.";
            Invoke("ClearDeathMessage", 5f); // 5초 후에 메시지를 지움
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
        CancelInvoke("CheckDeaths"); // 반복 호출 취소
        DeathAnnounceText.text = "";
    }
}
