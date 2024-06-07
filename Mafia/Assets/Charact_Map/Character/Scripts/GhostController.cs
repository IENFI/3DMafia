using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GhostController : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private KeyCode jumpKeyCode = KeyCode.Space;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private FPCameraController cameraController;
    private Movement movement;
    [SerializeField]
    private Camera ghostCamera;

    public void InitializeAsGhost()
    {
        // 유령으로 변환할 때 필요한 설정
        int ghostLayer = LayerMask.NameToLayer("Ghost");
        if (ghostLayer == -1)
        {
            Debug.LogError("The 'Ghost' layer does not exist. Please add it to the Tags and Layers settings.");
            return;
        }
        gameObject.layer = ghostLayer;

        // 필요 시 유령만 볼 수 있는 오브젝트 레이어 설정
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Color ghostColor = renderer.material.color;
            ghostColor.a = 0.5f; // 반투명하게 설정
            renderer.material.color = ghostColor;
        }
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            // 자신의 유령이 아닌 경우에는 이동을 제어하지 않습니다.
            enabled = false;
        }
        InitializeAsGhost();
        movement = GetComponent<Movement>();
        cameraController = GetComponentInChildren<FPCameraController>();
        ghostCamera.cullingMask = ~0;
    }


    void Update()
    {
        // 유령으로서의 행동 구현
        // 예: 특정 키를 눌러 맵을 떠돌거나 시체를 찾기
        if (photonView.IsMine)
        {
            // 방향키를 눌러 이동
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            movement.MoveSpeed = 10.0f;

            // 이동 함수 호출 (카메라가 보고있는 방향을 기준으로 방향키에 따라 이동)
            movement.MoveTo(cameraTransform.rotation * new Vector3(x, 0, z));

            // 회전 설정 (항상 앞만 보도록 캐릭터의 회전은 카메라와 같은 회전 값으로 설정)
            transform.rotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);

            movement.Gravity = -9.00f;

            // Space키를 누르면 점프
            if (Input.GetKeyDown(jumpKeyCode))
            {
                //playerAnimator.OnJump();    // 애니메이션 파라미터 설정 (onJump)
                movement.JumpTo();        // 점프 함수 호출
            }

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            cameraController.RotateTo(mouseX, mouseY);
        }
    }

    // 네트워크 동기화를 위한 IPunObservable 인터페이스 구현
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터를 보내는 경우 (로컬 플레이어의 경우)
            // 여기에 유령의 상태를 전송하는 코드를 추가합니다.
            // 예를 들어, 위치, 회전 등의 정보를 전송할 수 있습니다.
            // 예를 들어,
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 데이터를 받는 경우 (원격 플레이어의 경우)
            // 여기에 원격 플레이어의 유령을 동기화하는 코드를 추가합니다.
            // 예를 들어, 전송된 위치, 회전 정보를 사용하여 유령의 위치와 회전을 동기화합니다.
            // 예를 들어,
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    private void SetupGhostCamera()
    {
        // 유령만 볼 수 있는 카메라를 찾거나 생성합니다.
        Camera ghostCamera = Camera.main;
        if (ghostCamera == null)
        {
            Debug.Log("There is not ghostCamera's main camera");
        }

        // 유령은 모든 레이어를 볼 수 있도록 설정합니다.
        ghostCamera.cullingMask = ~0;

        // 기존 카메라는 유령 레이어를 렌더링하지 않도록 설정합니다.
        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera != ghostCamera)
        {
            // 메인 카메라가 존재하고 그것이 유령 카메라가 아닐 때
            // 유령 레이어를 제외한 모든 레이어를 렌더링하도록 설정합니다.
            mainCamera.cullingMask &= ~LayerMask.GetMask("Ghost");
        }
    }
}
