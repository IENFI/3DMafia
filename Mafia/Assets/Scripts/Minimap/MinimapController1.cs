using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public Transform playerTransform; // 플레이어의 Transform을 Inspector에서 할당
    public RectTransform minimap; // 미니맵의 RectTransform 참조
    public RawImage minimapImage; // 미니맵의 RawImage 컴포넌트 참조
    public Vector2 enlargedSize = new Vector2(600, 600); // 확장할 크기
    public Vector2 originalSize = new Vector2(250, 250); // 원래 크기
    public Vector2 enlargedPosition = new Vector2(-685, -685); // 중앙 위치 (앵커 기준)
    public Vector2 originalPosition = new Vector2(-152, -153); // 우측 상단 위치 (앵커 기준)
    private bool isEnlarged = false; // 미니맵이 확장된 상태인지 여부
    private PlayerController player;

    public float transparencySpeed = 2.0f; // 투명도 전환 속도
    public float transparentAlpha = 0.5f; // 이동 중 미니맵의 알파 값
    private Vector3 lastPlayerPosition; // 이전 프레임의 플레이어 위치
    private Color originalColor; // 미니맵의 원래 색상

    void Start()
    {
        // 초기 상태 설정

        StartCoroutine(FindLocalPlayerController());
        originalColor = minimapImage.color;

    }

    private IEnumerator FindLocalPlayerController()
    {
        while (player == null)
        {
            foreach (var pcc in FindObjectsOfType<PlayerController>())
            {
                if (pcc.photonView.IsMine)
                {
                    player = pcc;
                    break;
                }
            }

            // 다음 프레임까지 대기
            yield return null;
        }
       
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)) // 'm' 키를 누르면
        {
            if (isEnlarged)
            {
                // 원래 크기와 위치로 되돌리기
                minimap.sizeDelta = originalSize;
                minimap.anchoredPosition = originalPosition;
                minimapImage.color = originalColor; // 원래 색상으로 복구
                isEnlarged = false;
            }
            else
            {
                // 확대하고 중앙으로 이동
                minimap.sizeDelta = enlargedSize;
                minimap.anchoredPosition = enlargedPosition;
                isEnlarged = true;
            }
        }
        if (player != null)
        {
            float playerY = player.transform.position.y;
            if (isEnlarged)
            {
                Vector3 playerDelta = player.transform.position - lastPlayerPosition;
                if (playerDelta.sqrMagnitude > 0.01f) // 이동을 감지할 임계값
                {
                    Color newColor = minimapImage.color;
                    newColor.a = Mathf.Lerp(minimapImage.color.a, transparentAlpha, Time.deltaTime * transparencySpeed);
                    minimapImage.color = newColor;
                }
                else
                {
                    Color newColor = minimapImage.color;
                    newColor.a = Mathf.Lerp(minimapImage.color.a, originalColor.a, Time.deltaTime * transparencySpeed);
                    minimapImage.color = newColor;
                }
                lastPlayerPosition = player.transform.position; // 현재 위치를 저장
            }
        }

    }
}


