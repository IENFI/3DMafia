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

    private float lastKillTime; // 킬한 시간 저장
    public float killCooldown = 5.0f; // 쿨타임 설정 (5초)

    [SerializeField]
    public GameObject ghostPrefab; // 유령 프리팹
    [SerializeField]
    public GameObject corpsePrefab; // 시체 프리팹

    private bool isDead = false;

    void Start()
    {
        // Cursor.visible = false;                 // 마우스 커서를 보이지 않게
        // Cursor.lockState = CursorLockMode.Locked;   // 마우스 커서 위치 고정

        movement = GetComponent<Movement>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        cameraController = GetComponentInChildren<FPCameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (isDead) return;
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

            // 이동 속도 설정
            if (offset == 1)
            {
                movement.MoveSpeed = z >= 0 ? 10.0f : 5.0f;
            }
            else
            {
                movement.MoveSpeed = z >= 0 ? 6.0f : 4.0f;
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
                if (Time.time - lastKillTime >= killCooldown)
                {
                    playerAnimator.Kill();
                    lastKillTime = Time.time;
                }
            }

            // 마우스 오른쪽 버튼을 누르면 무기 공격 (연계)
            if (Input.GetMouseButtonDown(1))
            {
                //playerAnimator.OnWeaponAttack();
            }

            // 예시로 D키를 눌러 죽음을 시뮬레이트 (테스트용)
            if (Input.GetKeyDown(KeyCode.K) && !isDead)
            {
                photonView.RPC("Death", RpcTarget.All);
            }

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            cameraController.RotateTo(mouseX, mouseY);
        }
    }

    [PunRPC]
    void Death()
    {
        if (!photonView.IsMine) return; // 로컬 플레이어가 아니면 중지

        if (isDead) return;
        isDead = true;
        playerAnimator.Death();
        StartCoroutine(HandleDeath());
    }

    [PunRPC]
    public void DisableGameObject()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator HandleDeath()
    {
        // 애니메이션의 길이를 대기합니다.
        yield return new WaitForSeconds(playerAnimator.GetAnimatorTime().length);

        PhotonNetwork.Instantiate(ghostPrefab.name, transform.position, transform.rotation);
        PhotonNetwork.Instantiate(corpsePrefab.name, transform.position, transform.rotation);

        // RPC를 통해 모든 클라이언트에서 gameObject를 비활성화합니다.
        photonView.RPC("DisableGameObject", RpcTarget.All);
    }

}
