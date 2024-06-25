using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Photon.Pun;

public class KillTimer : MonoBehaviour
{
    public Image cooldownImage; // 쿨타임 이미지를 할당받기 위한 변수
    public GameObject tooltip; //툴팁UI
    public TMP_Text cooldownText; // 쿨타임 남은 시간을 표시할 텍스트
    public float cooldownDuration = 5f; // 쿨타임 지속 시간

    private bool isCooldown = false; // 쿨타임 진행 여부
    private float cooldownTime; // 쿨타임 남은 시간

    void Start()
    {
        tooltip = GameObject.FindWithTag("tooltip");
        StartCoroutine(KillTime());

    }

    void Update()
    {
        if (isCooldown)
        {
            cooldownTime -= Time.deltaTime; // 쿨타임 시간 감소
            if (cooldownTime <= 0)
            {
                isCooldown = false; // 쿨타임 종료
                cooldownImage.fillAmount = 1; // 이미지를 원래대로 표시
                cooldownText.text = "킬"; // 텍스트를 원래대로 표시
            }
            else
            {
                cooldownImage.fillAmount = 1 - (cooldownTime / cooldownDuration); // 쿨타임 동안 이미지 채워짐
                cooldownText.text = Mathf.Ceil(cooldownTime).ToString(); // 남은 시간 텍스트로 표시
            }
        }
    }

    public IEnumerator KillTime()
    {
        yield return new WaitForSeconds(1);

        if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isMafia"])
        {
            cooldownImage.fillAmount = 1;
            cooldownText.text = "킬";
        }
        else
        {
            cooldownImage.fillAmount = 0; // 마피아가 아니면 이미지는 숨김
            cooldownText.text = ""; // 마피아가 아니면 텍스트도 숨김
            tooltip.SetActive(false);
        }
    }

    public void StartCooldown()
    {
        isCooldown = true; // 쿨타임 시작
        cooldownTime = cooldownDuration; // 쿨타임 시간 설정
        cooldownImage.fillAmount = 0; // 이미지 초기화
        cooldownText.text = cooldownDuration.ToString(); // 텍스트 초기화
    }

    public static implicit operator KillTimer(GameObject v)
    {
        throw new NotImplementedException();
    }
}
