using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;

public class PlayerController : MonoBehaviourPun, IPunObservable
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
    private PlayerAttack playerAttackCollision;


    [SerializeField]
    public float playerMoveSpeedUnit = 1; // 플레이어 이동 속도 단위

    private float lastKillTime; // 킬한 시간 저장

    public float killCooldown = 15.0f; // 쿨타임 설정 (15초) 

    [SerializeField]
    public GameObject ghostPrefab; // 유령 프리팹
    [SerializeField]
    public GameObject corpsePrefab; // 시체 프리팹

    private bool isDead = false;

    public PlayerReportRadius reportRadius; // ReportRadius 스크립트 참조
    public float reportCooldown = 5f; // 신고 쿨타임
    private float lastReportTime;

    public TextMeshProUGUI nickName;
    public PhotonView PV;

    public GameObject[] objectsToHide;

    [SerializeField]
    private GameObject miniMapPointPrefab1; // MiniMapPoint 프리팹
    private GameObject miniMapPoint1; // MiniMapPoint 인스턴스

    [SerializeField]
    private GameObject miniMapPointPrefab2; // MiniMapPoint 프리팹
    private GameObject miniMapPoint2; // MiniMapPoint 인스턴스

    private Vector3 networkPosition;
    private Quaternion networkRotation;
    public float smoothing = 10f;

    public float GetLastReportTime()
    {
        return lastReportTime;
    }

    public void SetLastReportTime(float reportTime)
    {
        lastReportTime = reportTime;
    }

    void Start()
    {
        movement = GetComponent<Movement>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        cameraController = GetComponentInChildren<FPCameraController>();
        playerAttackCollision = GetComponentInChildren<PlayerAttack>();
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

        nickName.text = PV.Owner.NickName;
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("isMafia") && (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMafia"])
        {
            // 다른 플레이어의 isMafia가 true인지 확인
            if (photonView.Owner.CustomProperties.ContainsKey("isMafia") && (bool)photonView.Owner.CustomProperties["isMafia"])
            {
                // 닉네임을 빨갛게 보이도록 설정
                nickName.color = Color.red;
            }
            else
            {
                // 닉네임을 기본 색상으로 설정
                nickName.color = Color.white;
            }
        }

        if (photonView.IsMine)
        {
            HideObjects();
            CreateMiniMapPoint(); // MiniMapPoint 생성
            CharacterController characterController = GetComponent<CharacterController>();
            characterController.enabled = true;
        }
    }

    // 매 프레임마다 호출되는 Update 함수
    void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * smoothing);
            return;
        }

        // 이 객체가 로컬 플레이어의 객체인지 확인
        if (photonView.IsMine)
        {

            if (isDead) return;

            if (GameManager.instance != null && GameManager.instance.IsAnyUIOpen())
            {
                movement.PauseMovement();
                return;
            }
            else
            {
                movement.ResumeMovement();
            }

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

            // Z 버튼을 누르면 Kill
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (Time.time - lastKillTime >= killCooldown && (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMafia"])
                {
                    playerAnimator.Kill();
                    lastKillTime = Time.time;
                    playerAttackCollision.SeletKillMember();
                }
            }

            // Kill 시뮬레이트
            //if (Input.GetKeyDown(KeyCode.P))
            //{
            //    photonView.RPC("Death", RpcTarget.All);
            //}

            /*// 마우스 오른쪽 버튼을 누르면 무기 공격 (연계)
            if (Input.GetMouseButtonDown(1))
            {
                // playerAnimator.OnWeaponAttack(); // 무기 공격 애니메이션 실행 (주석 처리됨)
            }*/

            /*   // 시체 없애기 시뮬레이트 함수
               if (Input.GetKeyDown(KeyCode.K))
               {
                   photonView.RPC("DisableAllCorpses", RpcTarget.All);
               }*/

            if (Input.GetKeyDown(KeyCode.R) && reportRadius.IsCorpseInRange())
            {

                if (Time.time - lastReportTime >= reportCooldown)
                {
                    ReportCorpse();
                }
            }

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            cameraController.RotateTo(mouseX, mouseY); // 카메라 회전 함수 호출
            UpdateMiniMapPointPosition(); // MiniMapPoint 위치 업데이트
        }
    }
    private void CreateMiniMapPoint()
    {
        if (miniMapPointPrefab1 != null)
        {
            miniMapPoint1 = Instantiate(miniMapPointPrefab1, transform.position + new Vector3(150f, 0, 0), Quaternion.identity);
            miniMapPoint1.transform.SetParent(transform); // MiniMapPoint를 플레이어의 자식으로 설정

            miniMapPoint2 = Instantiate(miniMapPointPrefab2, transform.position + new Vector3(300f, 0, 0), Quaternion.identity);
            miniMapPoint2.transform.SetParent(transform); // MiniMapPoint를 플레이어의 자식으로 설정
        }
    }

    private void UpdateMiniMapPointPosition()
    {
        if (miniMapPoint1 != null)
        {
            // 플레이어의 위치에 x축으로 150만큼 이동한 위치로 MiniMapPoint를 업데이트
            miniMapPoint1.transform.position = transform.position + new Vector3(150f, 0, 0);
        }

        if (miniMapPoint2 != null)
        {
            // 플레이어의 위치에 x축으로 150만큼 이동한 위치로 MiniMapPoint를 업데이트
            miniMapPoint2.transform.position = transform.position + new Vector3(300f, 0, 0);
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

        // 현재 아바타 이름을 기반으로 한 corpseAvatar 문자열 생성
        string corpseAvatar = "corpse_"+GetComponent<AvatarChanger>().getCurrentAvatarName();

        // Resources 폴더에서 corpseAvatar 이름과 동일한 프리팹을 로드
        GameObject corpsePrefab = Resources.Load<GameObject>(corpseAvatar);

        // 프리팹이 로드되었는지 확인 (null 체크)
        if (corpsePrefab != null)
        {
            // PhotonNetwork를 사용하여 프리팹을 인스턴스화
            PhotonNetwork.Instantiate(corpsePrefab.name, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogError("Prefab not found: " + corpseAvatar);
        }

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

    [PunRPC]
    public void DisableAllCorpses()
    {
        GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");
        foreach (GameObject corpse in corpses)
        {
            // 비활성화
            //corpse.SetActive(false);

            // 씬에서 아예 없애기
            GetComponentInChildren<PlayerReportRadius>().DestoryCorpse();
            Destroy(corpse);
        }
    }

    private void HideObjects()
    {
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
            {
                Renderer[] renderers = GetAllRenderersIncludingInactive(obj);
                foreach (Renderer renderer in renderers)
                {
                    // 그림자는 보이게 하기
                    renderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }
            }
        }
    }

    public Renderer[] GetAllRenderersIncludingInactive(GameObject root)
    {
        List<Renderer> renderers = new List<Renderer>();
        AddRenderersRecursive(root.transform, renderers);
        return renderers.ToArray();
    }

    private void AddRenderersRecursive(Transform current, List<Renderer> renderers)
    {
        // 현재 오브젝트의 Renderer를 가져옴
        Renderer renderer = current.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderers.Add(renderer);
        }

        // 자식들을 순회하며 재귀적으로 호출
        foreach (Transform child in current)
        {
            AddRenderersRecursive(child, renderers);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 플레이어의 데이터를 네트워크로 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 네트워크에서 받은 데이터로 원격 플레이어 업데이트
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }

    }
}
