using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    // 이속 증가 아이템 편하게 쓰려고 moveSpeed를 public으로 둠
    [SerializeField]
    private float moveSpeed = 4;        // 이동 속도
    private Vector3 moveDirection;      // 이동 방향

    private CharacterController characterController;

    public float MoveSpeed
    {
        // 이동속도는 4~100 사이의 값만 설정 가능
        // .....이속제한... "50배"(두등-!)
        set => moveSpeed = Mathf.Clamp(value, 4.0f, 100.0f);
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {

        // 이동 설정. CharacterController의 Move() 함수를 이용한 이동
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }

    public void MoveTo(Vector3 direction)
    {
        moveDirection = new Vector3(direction.x, moveDirection.y, direction.z);
    }
}