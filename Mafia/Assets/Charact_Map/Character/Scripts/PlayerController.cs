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

    [SerializeField]
    public float playerMoveSpeedUnit = 1;

    void Start()
    {
        // Cursor.visible = false;                 // ���콺 Ŀ���� ������ �ʰ�
        // Cursor.lockState = CursorLockMode.Locked;   // ���콺 Ŀ�� ��ġ ����

        movement = GetComponent<Movement>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        cameraController = GetComponentInChildren<FPCameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            // ����Ű�� ���� �̵�
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // shift Ű�� �� ������ �ִ� 0.5, ������ �ִ� 1����
            bool isShiftKeyPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float offset = isShiftKeyPressed ? 1.0f : 0.5f;
            Debug.Log(offset);
            // �ִϸ��̼� �� ���� (-1 : ����, 0 : ���, 1 : ������)
            // �ִϸ��̼� �Ķ���� ���� (horizontal, vertical)
            playerAnimator.OnMovement(x * offset, z * offset);

            // �̵� �ӵ� ���� (������ �̵��Ҷ��� 5, �������� 2)
            if (offset == 1)
            {
                movement.MoveSpeed = z > 0 ? 10.0f : 5.0f;
            }
            else
            {
                movement.MoveSpeed = z > 0 ? 6.0f : 4.0f;
            }

            // �̵� �Լ� ȣ�� (ī�޶� �����ִ� ������ �������� ����Ű�� ���� �̵�)
            movement.MoveTo(cameraTransform.rotation * new Vector3(x, 0, z));

            // ȸ�� ���� (�׻� �ո� ������ ĳ������ ȸ���� ī�޶�� ���� ȸ�� ������ ����)
            transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);

            // SpaceŰ�� ������ ����
            if (Input.GetKeyDown(jumpKeyCode))
            {
                //playerAnimator.OnJump();    // �ִϸ��̼� �Ķ���� ���� (onJump)
                movement.JumpTo();        // ���� �Լ� ȣ��
            }

            // ���콺 ���� ��ư�� ������ ������ ����
            if (Input.GetMouseButtonDown(0))
            {
                playerAnimator.Kill();
            }

            // ���콺 ������ ��ư�� ������ ���� ���� (����)
            if (Input.GetMouseButtonDown(1))
            {
                //playerAnimator.OnWeaponAttack();
            }

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            cameraController.RotateTo(mouseX, mouseY);
        }
    }
}
