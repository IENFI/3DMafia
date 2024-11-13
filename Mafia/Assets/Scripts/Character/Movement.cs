using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Movement : MonoBehaviour
{
    // 이속 증가 아이템 편하게 쓰려고 moveSpeed를 public으로 둠
    [SerializeField]
    private float moveSpeed = 4;        // 이동 속도
    [SerializeField]
    private float gravity = -9.81f; // 중력 계수
    [SerializeField]
    private float jumpForce = 3.0f; // 뛰어 오르는 힘
    private Vector3 moveDirection;      // 이동 방향

    private CharacterController characterController;
    private bool isMovementPaused = false;

    public float MoveSpeed
    {
        // 이동속도는 4~100 사이의 값만 설정 가능
        // .....이속제한... "50배"(두등-!)
        set => moveSpeed  = Mathf.Clamp(value, 4.0f, 100.0f);
    }

    public float Gravity
    {
        set => gravity = Mathf.Clamp(value, -8.0f, -10.0f);
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 중력 설정. 플레이어가 땅을 밟고 있지 않다면 y축 이동방향에 gravity * Time.deltaTime을 더해준다
        // 캐릭터가 땅에 있지 않을 경우 중력을 적용하여 Y축 이동을 지속적으로 감소시킴
        if (!characterController.isGrounded)
        {
            moveDirection.y += gravity * Time.deltaTime;
        }
        else if (isMovementPaused)
        {
            // 땅에 있을 경우 Y축 이동을 0으로 설정하여 중력의 영향을 초기화
            moveDirection.y = 0;
        }

        if (isMovementPaused)
        {
            // 이동은 멈추고 중력만 적용
            characterController.Move(new Vector3(0, moveDirection.y, 0) * Time.deltaTime);
        }
        else
        {
            // 일반적인 이동 설정
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    public void MoveTo(Vector3 direction)
    {
        if (!isMovementPaused)
        {
            moveDirection = new Vector3(direction.x, moveDirection.y, direction.z);
        }
    }

    public void JumpTo()
    {
        // 캐릭터가 바닥을 밟고 있으면 점프
        if (characterController.isGrounded && !isMovementPaused)
        {
            moveDirection.y = jumpForce;
        }
    }
    public void PauseMovement()
    {
        isMovementPaused = true;
    }

    public void ResumeMovement()
    {
        isMovementPaused = false;
    }
}