using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCameraController : MonoBehaviour
{
    private float rotateSpeedX = 3;
    private float rotateSpeedY = 5;
    private float limitMinX = -80;
    private float limitMaxX = 50;
    private float eulerAngleX;
    private float eulerAngleY;

    private bool canRotate = true; // 회전 기능을 활성화/비활성화할 수 있는 플래그 변수


    //public Transform player;       // 플레이어의 Transform
    //public Vector3 offset;         // 카메라와 플레이어 사이의 오프셋 값
    //public float smoothTime = 0.3f; // 카메라 이동의 부드러움을 조절하는 값

    //private Vector3 velocity = Vector3.zero;
    //private Animator playerAnimator;

    //private void Start()
    //{
    //    // 플레이어의 Animator 컴포넌트 가져오기
    //    playerAnimator = player.GetComponentInChildren<Animator>();

    //    // 기본 오프셋 값 설정 (카메라와 플레이어 사이의 초기 거리)
    //    offset = transform.position - player.position;
    //}

    //private void LateUpdate()
    //{
    //    if (playerAnimator != null)
    //    {
    //        // 애니메이션 상태에 따라 카메라 위치 조정
    //        Vector3 targetPosition = player.position + offset;

    //        // 애니메이션이 실행 중일 때 카메라 위치 조정 (예: "IsKicking" 파라미터를 기준으로)
    //        if (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Kill"))
    //        {
    //            targetPosition = player.position + offset + player.forward; // 카메라를 애니메이션에 맞게 조정
    //        }

    //        // 부드럽게 카메라 위치를 변경
    //        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    //    }
    //    else
    //    {
    //        // 플레이어의 위치를 가져와 카메라 위치 설정
    //        transform.position = player.position + offset;
    //    }
    //}

    public void RotateTo(float mouseX, float mouseY)
    {
        if (!canRotate) return;
        // 마우스를 좌/우로 움직이는 mouseX 값을 y축에 대입하는 이유는
        // 마우스를 좌/우로 움직일 때 카메라도 좌/우를 보려면 카메라 오브젝트의
        // y축이 회전되어야 하기 때문
        eulerAngleY += mouseX * rotateSpeedX;
        // 같은 개념으로 카메라가 위/아래를 보려면 카메라 오브젝트의 x축이 회전!
        // (카메라가 아래를 보는 것은 양수, 마우스가 아래로 이동하는 것은 음수)
        eulerAngleX -= mouseY * rotateSpeedY;

        // x축 회전 값의 경우 아래, 위를 볼 수 있는 제한 각도가 설정되어 있다.
        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        // 실제 오브젝트의 쿼터니온 회전에 적용
        transform.rotation = Quaternion.Euler(eulerAngleX, eulerAngleY, 0);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        // Mathf.Clamp()를 이용해 angle이 min <= angle <= max을 유지하도록 함
        return Mathf.Clamp(angle, min, max);
    }

    // 회전 기능을 비활성화하는 메서드
    public void DisableRotation()
    {
        canRotate = false;
    }

    // 회전 기능을 활성화하는 메서드
    public void EnableRotation()
    {
        canRotate = true;
    }
}
