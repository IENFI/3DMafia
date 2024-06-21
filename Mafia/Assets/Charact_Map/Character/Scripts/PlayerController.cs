using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField]
    private KeyCode jumpKeyCode = KeyCode.Space; // 점프 키 (Space 키)
    [SerializeField]
    private Transform cameraTransform; // 카메라의 Transform
    [SerializeField]
    private FPCameraController cameraController;
    [SerializeField]
    private Camera FPcamera;
    private Movement movement;
    private PlayerAnimator playerAnimator;

    [SerializeField]
    public float playerMoveSpeedUnit = 1; // 플레이어 이동 속도 단위

    private float lastKillTime; // 킬한 시간 저장
    public float killCooldown = 5.0f; // 쿨타임 설정 (5초)

    [SerializeField]
    public GameObject ghostPrefab; // 유령 프리팹
    [SerializeField]
    public GameObject corpsePrefab; // 시체 프리팹

    private bool isDead = false;

    public PlayerReportRadius reportRadius; // ReportRadius 스크립트 참조
    public float reportCooldown = 5f; // 신고 쿨타임
    private float lastReportTime;

    void Start()
    {
        // Cursor.visible = false;                 // 마우스 커서를 보이지 않게
        // Cursor.lockState = CursorLockMode.Locked;   // 마우스 커서 위치 고정

        movement = GetComponent<Movement>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        cameraController = GetComponentInChildren<FPCameraController>();
        FPcamera.cullingMask &= ~LayerMask.GetMask("Ghost");

        // 유령으로 변환할 때 필요한 설정
        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer == -1)
        {
            Debug.LogError("The 'Ghost' layer does not exist. Please add it to the Tags and Layers settings.");
            return;
        }
        gameObject.layer = playerLayer;

        lastReportTime = Time.time;

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
            if (isDead) return;
            // 방향키를 눌러 이동
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            // shift 키를 안 누르면 최대 0.5, 누르면 최대 1까지
            bool isShiftKeyPressed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            float offset = isShiftKeyPressed ? 1.0f : 0.5f;
            // Debug.Log(offset);
            // 애니메이션 값 설정 (-1 : 왼쪽, 0 : 가운데, 1 : 오른쪽)
            // 애니메이션 파라미터 설정 (horizontal, vertical)
            playerAnimator.OnMovement(x * offset, z * offset);

            // 이동 속도 설정
            if (offset == 1)
            {
                movement.MoveSpeed = z >= 0 ? playerMoveSpeedUnit * 10.0f : playerMoveSpeedUnit * 5.0f;
            }
            else
            {
                movement.MoveSpeed = z >= 0 ? playerMoveSpeedUnit * 6.0f : playerMoveSpeedUnit * 4.0f;
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

            // 마우스 왼쪽 버튼을 누르면 Kill
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Before Kill()");
                if (Time.time - lastKillTime >= killCooldown)
                {
                    playerAnimator.Kill();
                    lastKillTime = Time.time;
                }
            }

            // 마우스 오른쪽 버튼을 누르면 무기 공격 (연계)
            if (Input.GetMouseButtonDown(1))
            {
                // playerAnimator.OnWeaponAttack(); // 무기 공격 애니메이션 실행 (주석 처리됨)
            }

            // 예시로 D키를 눌러 죽음을 시뮬레이트 (테스트용)
            if (Input.GetKeyDown(KeyCode.K) && !isDead)
            {
                photonView.RPC("Death", RpcTarget.All);
            }

            if (Input.GetKeyDown(KeyCode.R) && reportRadius.IsCorpseInRange())
            {
                Debug.Log("Before ReportCorpse()");

                if (Time.time - lastReportTime >= reportCooldown)
                {
                    ReportCorpse();
                }
            }

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            cameraController.RotateTo(mouseX, mouseY); // 카메라 회전 함수 호출
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
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable
        {
        { "isDead" , true }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
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

    public void ChangeMoveSpeed()
    {
        playerMoveSpeedUnit *= 5;
    }

    public void OriginMoveSpeed()
    {
        playerMoveSpeedUnit /= 5;
    }

    private void ReportCorpse()
    {
        Debug.Log("Corpse reported!");
        lastReportTime = Time.time;
        PhotonView votingSystemPhotonView = FindObjectOfType<VotingSystem>().GetComponent<PhotonView>();
        if (votingSystemPhotonView != null)
        {
            votingSystemPhotonView.RPC("StartVote", RpcTarget.All);
        }
        // 신고 처리 로직 추가
        // 예: 시체를 삭제하거나 상태를 변경합니다.
    }
}
