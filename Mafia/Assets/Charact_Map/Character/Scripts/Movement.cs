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

    public float MoveSpeed
    {
        // 이동속도는 2 ~ 4 사이의 값만 설정 가능
        // .....이속제한... "50배"(두등-!)
        set => moveSpeed = Mathf.Clamp(value, 2.0f, 100.0f);
    }

    public float Gravity
    {
        // 이동속도는 2 ~ 4 사이의 값만 설정 가능
        // .....이속제한... "50배"(두등-!)
        set => gravity = Mathf.Clamp(value, -8.0f, -10.0f);
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // 중력 설정. 플레이어가 땅을 밟고 있지 않다면
        // y축 이동방향에 gravity * Time.deltaTime을 더해준다
        if (characterController.isGrounded == false)
        {
            moveDirection.y += gravity * Time.deltaTime;
        }

        // 이동 설정. CharacterController의 Move() 함수를 이용한 이동
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = new Vector3(direction.x, moveDirection.y, direction.z);
    }

    public void JumpTo()
    {
        // 캐릭터가 바닥을 밟고 있으면 점프
        if (characterController.isGrounded == true)
        {
            moveDirection.y = jumpForce;
        }
    }
}