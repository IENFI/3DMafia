using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField]
    private KeyCode jumpKeyCode = KeyCode.Space; // 점프 키 (Space 키)
    [SerializeField]
    private Transform cameraTransform; // 카메라의 Transform
    [SerializeField]
    private FPCameraController cameraController; // 1인칭 카메라 컨트롤러
    private Movement movement; // 이동을 담당하는 Movement 컴포넌트
    private PlayerAnimator playerAnimator; // 플레이어 애니메이터

    [SerializeField]
    public float playerMoveSpeedUnit = 1; // 플레이어 이동 속도 단위

    void Start()
    {
        // 이동 및 애니메이션 컴포넌트 초기화
        movement = GetComponent<Movement>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        cameraController = GetComponentInChildren<FPCameraController>();

        // 커서를 숨기고 잠금 (필요에 따라 주석 해제)
        // Cursor.visible = false; 
        // Cursor.lockState = CursorLockMode.Locked; 
    }

    // 매 프레임마다 호출되는 Update 함수
    void Update()
    {
        // 이 객체가 로컬 플레이어의 객체인지 확인
        if (photonView.IsMine)
        {
            // 키보드 입력을 받아 이동 값 설정
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // Shift 키가 눌려 있는지 확인하고 속도 배율 설정
            bool isShiftKeyPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float offset = isShiftKeyPressed ? 1.0f : 0.5f;

            // 애니메이터에 이동 값 전달
            // -1 : 뒤로 이동, 0 : 정지, 1 : 앞으로 이동
            playerAnimator.OnMovement(x * offset, z * offset);

            // 이동 속도 설정 (앞으로 이동 시 5, 뒤로 이동 시 2)
            if (offset == 1)
            {
                movement.MoveSpeed = z >= 0 ? playerMoveSpeedUnit * 10.0f : playerMoveSpeedUnit * 5.0f;
            }
            else
            {
                movement.MoveSpeed = z >= 0 ? playerMoveSpeedUnit * 6.0f : playerMoveSpeedUnit * 4.0f;
            }

            // 이동 함수 호출 (카메라 방향을 기준으로 이동)
            movement.MoveTo(cameraTransform.rotation * new Vector3(x, 0, z));

            // 캐릭터의 회전 설정 (카메라의 y축 회전 값으로 설정)
            transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);

            // 점프 키 입력 처리
            if (Input.GetKeyDown(jumpKeyCode))
            {
                // 점프 애니메이션 트리거 (주석 처리됨)
                // playerAnimator.OnJump();    

                // 점프 함수 호출
                movement.JumpTo();
            }

            // 마우스 왼쪽 버튼 클릭 시 처리 (공격)
            if (Input.GetMouseButtonDown(0))
            {
                playerAnimator.Kill(); // 킬 애니메이션 실행
            }

            // 마우스 오른쪽 버튼 클릭 시 처리 (추가 기능 주석 처리됨)
            if (Input.GetMouseButtonDown(1))
            {
                // playerAnimator.OnWeaponAttack(); // 무기 공격 애니메이션 실행 (주석 처리됨)
            }

            // 마우스 움직임에 따른 카메라 회전
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            cameraController.RotateTo(mouseX, mouseY); // 카메라 회전 함수 호출
        }
    }

    public void ChangeMoveSpeed()
    {
        playerMoveSpeedUnit *= 5; 
    }
}
