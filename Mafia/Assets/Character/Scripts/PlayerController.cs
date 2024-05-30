using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField]
    private KeyCode jumpKeyCode = KeyCode.Space;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private FPCameraController cameraController;
    private Movement movement;
    private PlayerAnimator playerAnimator;

    private void Awake()
    {
        // Cursor.visible = false;                 // 마우스 커서를 보이지 않게
        // Cursor.lockState = CursorLockMode.Locked;   // 마우스 커서 위치 고정

        movement = GetComponent<Movement>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        cameraController = GetComponentInChildren<FPCameraController>();
    }

    private void Update()
    {
        // PhotonView가 로컬 플레이어의 것인지 확인
        
        /*
        if (photonView.IsMine)
        {
            
            // 방향키를 눌러 이동
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // shift 키를 안 누르면 최대 0.5, 누르면 최대 1까지
            bool isShiftKeyPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float offset = isShiftKeyPressed ? 1.0f : 0.5f;
            Debug.Log(offset);
            // 애니메이션 값 설정 (-1 : 왼쪽, 0 : 가운데, 1 : 오른쪽)
            // 애니메이션 파라미터 설정 (horizontal, vertical)
            playerAnimator.OnMovement(x * offset, z * offset);

            // 이동 속도 설정 (앞으로 이동할때만 5, 나머지는 2)
            if (offset == 1)
            {
                movement.MoveSpeed = z > 0 ? 5.0f : 2.0f;
            }
            else
            {
                movement.MoveSpeed = z > 0 ? 3.0f : 2.0f;
            }

            // 이동 함수 호출 (카메라가 보고있는 방향을 기준으로 방향키에 따라 이동)
            movement.MoveTo(cameraTransform.rotation * new Vector3(x, 0, z));

            // 회전 설정 (항상 앞만 보도록 캐릭터의 회전은 카메라와 같은 회전 값으로 설정)
            transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);

            // Space키를 누르면 점프
            if (Input.GetKeyDown(jumpKeyCode))
            {
                //playerAnimator.OnJump();    // 애니메이션 파라미터 설정 (onJump)
                movement.JumpTo();        // 점프 함수 호출
            }

            // 마우스 왼쪽 버튼을 누르면 발차기 공격
            if (Input.GetMouseButtonDown(0))
            {
                playerAnimator.Kill();
            }

            // 마우스 오른쪽 버튼을 누르면 무기 공격 (연계)
            if (Input.GetMouseButtonDown(1))
            {
                //playerAnimator.OnWeaponAttack();
            }

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            cameraController.RotateTo(mouseX, mouseY);
        }
        */

        // 방향키를 눌러 이동
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // shift 키를 안 누르면 최대 0.5, 누르면 최대 1까지
        bool isShiftKeyPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float offset = isShiftKeyPressed ? 1.0f : 0.5f;
        Debug.Log(offset);
        // 애니메이션 값 설정 (-1 : 왼쪽, 0 : 가운데, 1 : 오른쪽)
        // 애니메이션 파라미터 설정 (horizontal, vertical)
        playerAnimator.OnMovement(x*offset, z*offset);

        // 이동 속도 설정 (앞으로 이동할때만 5, 나머지는 2)
        if ( offset == 1 ) {
            movement.MoveSpeed = z > 0 ? 5.0f : 2.0f;
        }
        else
        {
            movement.MoveSpeed = z > 0 ? 3.0f : 2.0f;
        }
        
        // 이동 함수 호출 (카메라가 보고있는 방향을 기준으로 방향키에 따라 이동)
        movement.MoveTo(cameraTransform.rotation * new Vector3(x, 0, z));

        // 회전 설정 (항상 앞만 보도록 캐릭터의 회전은 카메라와 같은 회전 값으로 설정)
        transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);

        // Space키를 누르면 점프
        if (Input.GetKeyDown(jumpKeyCode))
        {
            //playerAnimator.OnJump();    // 애니메이션 파라미터 설정 (onJump)
            movement.JumpTo();        // 점프 함수 호출
        }

        // 마우스 왼쪽 버튼을 누르면 발차기 공격
        if (Input.GetMouseButtonDown(0))
        {
            playerAnimator.Kill();
        }

        // 마우스 오른쪽 버튼을 누르면 무기 공격 (연계)
        if (Input.GetMouseButtonDown(1))
        {
            //playerAnimator.OnWeaponAttack();
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        cameraController.RotateTo(mouseX, mouseY);
    }
}