using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInteraction : MonoBehaviour
{
    [SerializeField] private Renderer outlineRenderer;
    public GameObject ShopUI; // 상점 UI

    private bool isPlayerInRange = false; // 플레이어가 상점 주변에 있는지 여부

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            // 플레이어가 범위 내에 들어온 경우
            outlineRenderer.material.color = Color.blue; // 외곽을 파란색으로 변경
            // 노란색 외곽 등의 시각적 피드백을 줄 수 있음
            // 예: 외곽 색상 변경, UI 표시 등
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            outlineRenderer.material.color = Color.white; // 외곽을 기본 색상으로 변경
            // 시각적 피드백 초기화
        }
    }

    private void Update()
    {
        // 플레이어가 상점 주변에 있고 F 키를 누르면
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.G))
        {
            // 상점 UI 활성화
            ShopUI.SetActive(true);
            // 다른 상호작용 방지를 위해 플레이어 조작 비활성화
            // 예: 캐릭터 이동, 카메라 회전 등
        }
    }
}
