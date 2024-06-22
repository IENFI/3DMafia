using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class KillTimer : MonoBehaviour
{
    public Image cooldownImage; // 쿨타임 이미지를 할당받기 위한 변수
    public TMP_Text cooldownText; // 쿨타임 남은 시간을 표시할 텍스트
    public float cooldownDuration = 5f; // 쿨타임 지속 시간

    private bool isCooldown = false; // 쿨타임 진행 여부
    private float cooldownTime; // 쿨타임 남은 시간

    void Start()
    {
        cooldownImage.fillAmount = 1; // 초기에는 이미지가 비어있는 상태로 설정
        cooldownText.text = "킬"; // 텍스트도 초기화
    }

    void Update()
    {
        if (isCooldown)
        {
            cooldownTime -= Time.deltaTime; // 쿨타임 시간 감소
            if (cooldownTime <= 0)
            {
                isCooldown = false; // 쿨타임 종료
                cooldownImage.fillAmount = 0; // 이미지 초기화
                cooldownText.text = ""; // 텍스트 초기화
            }
            else
            {
                cooldownImage.fillAmount = 1 - (cooldownTime / cooldownDuration); // 쿨타임 동안 이미지 채워짐
                cooldownText.text = Mathf.Ceil(cooldownTime).ToString(); // 남은 시간 텍스트로 표시
            }
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