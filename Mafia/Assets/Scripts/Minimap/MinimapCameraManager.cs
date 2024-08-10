using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // RawImage를 사용하기 위해 추가

public class MinimapCameraManager : MonoBehaviour
{
    public Transform playerTransform; // 플레이어의 Transform을 Inspector에서 할당
    public Camera camera1F; // 1층을 비추는 카메라
    public Camera camera2F; // 2층을 비추는 카메라
    public RawImage rawImage1F; // 1층을 위한 RawImage
    public RawImage rawImage2F; // 2층을 위한 RawImage
    public float switchHeight = 12f; // 1층과 2층을 나누는 Y값
    private PlayerController player;

    void Start()
    {
        // 초기 상태 설정
       
        StartCoroutine(FindLocalPlayerController());

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

            if (player == null)
            {
                //Debug.Log("(MinimapCameraManager) PlayerController를 찾는 중...");
            }

            // 다음 프레임까지 대기
            yield return null;
        }

        //Debug.Log("(MinimapCameraManager) PlayerController를 찾았습니다.");

        UpdateCamera();
    }

    void Update()
    {
        // 매 프레임마다 플레이어의 Y값을 체크하고 카메라를 업데이트
        if (player != null)
        {
            UpdateCamera();
        }

    }

    void UpdateCamera()
    {
        float playerY = player.transform.position.y;
        //Debug.Log("Player Y position: " + playerY);

        if (playerY < switchHeight)
        {
            // 플레이어가 1층에 있을 때
            //Debug.Log("Switching to 1F camera");
            SetCameraAndRawImage(camera1F, camera2F, rawImage1F, rawImage2F);
        }
        else
        {
            // 플레이어가 2층에 있을 때
            //Debug.Log("Switching to 2F camera");
            SetCameraAndRawImage(camera2F, camera1F, rawImage2F, rawImage1F);
        }
    }


    void SetCameraAndRawImage(Camera activeCamera, Camera inactiveCamera, RawImage activeRawImage, RawImage inactiveRawImage)
    {
        activeCamera.gameObject.SetActive(true);
        inactiveCamera.gameObject.SetActive(false);
        activeRawImage.gameObject.SetActive(true);
        inactiveRawImage.gameObject.SetActive(false);
    }
}
