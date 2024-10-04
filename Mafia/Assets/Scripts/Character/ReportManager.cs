using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReportManager : MonoBehaviour
{
    private PlayerController playerController; // PlayerController 참조 변수
    public Image reportImage; // Report UI 이미지
    public Color flashColor = Color.red; // 점멸할 때의 색상
    public float flashSpeed = 0.5f; // 점멸 속도
    private bool isFlashing = false; // 현재 점멸 중인지 여부

    void Start()
    {
        StartCoroutine(InitializePlayerController());
    }

    public IEnumerator InitializePlayerController()
    {
        yield return new WaitForSeconds(1); // 1초 대기 (로딩 시간을 고려하여)

        while (playerController == null)
    {
        // 씬 내의 모든 PlayerController를 찾고, 그 중 로컬 플레이어의 PhotonView를 가진 것만 선택
        foreach (var pcc in FindObjectsOfType<PlayerController>())
        {
            if (pcc.photonView.IsMine)
            {
                playerController = pcc;
                break;
            }
        }

        if (playerController == null)
        {
            Debug.LogWarning("로컬 플레이어의 PlayerController를 찾고 있습니다...");
            yield return new WaitForSeconds(1); // 1초 대기 후 다시 시도
        }
    }
    }

    void Update()
    {
        if (playerController == null){
            Debug.Log("playerController가 null입니다.");
            return;
        }

        // 시체가 범위 내에 있을 경우 점멸 시작
        if (playerController.reportRadius.IsCorpseInRange())
        {
            Debug.Log("시체가 범위 내에 있습니다. 점멸을 시작합니다.");
            if (!isFlashing)
            {
                StartCoroutine(FlashReportImage());
            }
        }
        // 시체가 범위 밖에 있을 경우 점멸 중단
        else
        {
            if (isFlashing) // 점멸 중이면 정지하고 색상 원래대로
            {
                StopCoroutine(FlashReportImage());
                isFlashing = false;
                reportImage.color = Color.white; // 기본 색상으로 복귀 (필요에 따라 변경)
            }
        }
    }

    // 이미지가 빨간색으로 점멸하도록 하는 코루틴
    private IEnumerator FlashReportImage()
    {
        isFlashing = true;
        while (isFlashing)
        {
            // 빨간색으로 변경
            reportImage.color = Color.red;
            yield return new WaitForSeconds(0.2f); // 0.5초 대기 (점멸 속도 조정 가능)

            // 원래 색상으로 변경 (흰색으로 가정)
            reportImage.color = Color.white;
            yield return new WaitForSeconds(0.2f); // 0.5초 대기 (점멸 속도 조정 가능)
        }
    }
}


