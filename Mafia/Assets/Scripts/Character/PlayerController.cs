using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Photon.Pun.Demo.PunBasics;
using System.Threading;
using UnityEngine.SceneManagement;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using MySql.Data.MySqlClient.Memcached;
using Photon.Realtime;
using System.Linq;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private KeyCode jumpKeyCode = KeyCode.Space; // 점프 키 (Space 키)
    [SerializeField]
    private Transform cameraTransform; // 카메라의 Transform
    [SerializeField]
    private FPCameraController fpCameraController;
    [SerializeField]
    private CameraController cameraController;
    [SerializeField]
    private Camera FPcamera;
    private Movement movement;
    private PlayerAnimator playerAnimator;
    private PlayerAttack playerAttackCollision;
    private AvatarChanger avatarChanger;


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


    [SerializeField]
    private bool canControl = true; // 플레이어 컨트롤 가능 여부
    float x_;
    float z_;
    // 플레이어 컨트롤 활성화/비활성화 함수

    [Header("보이스")]
    private const byte PlayerGroup = 1; // 플레이어 그룹 ID
    private const byte GhostGroup = 2; // 유령 그룹 ID
    Recorder recorder;
    Speaker speaker;
    private PunVoiceClient punVoiceClient; // PunVoiceClient 참조
    public MicrophoneVolumeIndicator microphoneVolumeIndicator;


    public void EnableControl(bool enable)
    {
        Debug.Log("Player EnableControl() : enable : " + enable);
        canControl = enable;
        if (enable)
            fpCameraController.EnableRotation();
        else
        {
            fpCameraController.DisableRotation();
        }

        x_ = Input.GetAxis("Horizontal");
        z_ = Input.GetAxis("Vertical");
    }

    void Start()
    {
        movement = GetComponent<Movement>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        fpCameraController = GetComponentInChildren<FPCameraController>();
        playerAttackCollision = GetComponentInChildren<PlayerAttack>();
        FPcamera.cullingMask &= ~LayerMask.GetMask("Ghost");
        avatarChanger = GetComponent<AvatarChanger>();

        // 유령으로 변환할 때 필요한 설정
        int playerLayer = LayerMask.NameToLayer("Player");
        if (playerLayer == -1)
        {
            Debug.LogError("The 'Ghost' layer does not exist. Please add it to the Tags and Layers settings.");
            return;
        }
        gameObject.layer = playerLayer;

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

            recorder = GetComponent<Recorder>();
            speaker = GetComponentInChildren<Speaker>();
            if (recorder != null) {
                // recorder.RestartingRecording();
                Debug.Log("Recorder started recording.");
            }
            else {
                Debug.LogWarning("Recorder component not found on this object.");
            }

            punVoiceClient = GameManager.instance.GetComponent<PunVoiceClient>();
            if (punVoiceClient != null)
            {
                // LocalPlayer의 Recorder와 Speaker를 PunVoiceClient에 설정
                punVoiceClient.PrimaryRecorder = GetComponent<Recorder>();
                punVoiceClient.SpeakerPrefab = GetComponentInChildren<Speaker>().gameObject;
            }

            StartCoroutine(ConfigureVoiceSetting());
            microphoneVolumeIndicator.UpdateMicrophoneUI(recorder.TransmitEnabled);
        }
    }

    private IEnumerator ConfigureVoiceSetting(){
        yield return new WaitUntil(() => PhotonNetwork.InRoom && PunVoiceClient.Instance.Client.State == ClientState.Joined);

        // 모든 플레이어의 Recorder에 동일한 InterestGroup 설정
        if (recorder != null) {
            recorder.InterestGroup = PlayerGroup;
            recorder.TransmitEnabled = true;
            Debug.Log("PlayerController : Recorder configured for PlayerGroup (1) for all players.");
        }

        // PunVoiceClient를 통해 수신 그룹 설정
        if (PunVoiceClient.Instance.Client.State == ClientState.Joined)
        {
            PunVoiceClient.Instance.Client.OpChangeGroups(new byte[] { GhostGroup }, new byte[] { PlayerGroup });
            Debug.Log("PlayerController : Listening to PlayerGroup (1) for all players.");
        }
    }

    public void UpdateNicknameColor(Color color)
    {
        if (nickName != null)
        {
            nickName.color = color;
        }
    }


    // 매 프레임마다 호출되는 Update 함수
    void Update()
    {
        // Debug.Log("canControl : " + canControl);
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

            if (GameManager.instance != null && GameManager.instance.IsAnyUIOpen() && !GameManager.instance.CheckRoomPanel() && !canControl)
            {
                movement.PauseMovement();
            }
            else if (GameManager.instance.CheckRoomPanel() && canControl)
            {
                movement.ResumeMovement();
            }
            else if (GameManager.instance.CheckRoomPanel() && !canControl)
            {
                movement.PauseMovement();
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

            if (canControl)
            {
                playerAnimator.OnMovement(x * offset, z * offset);
            }
            else if(!GameManager.instance.CheckRoomPanel())
            {
                // 플레이어가 제어 불가능할 때의 파라미터 값을 부드럽게 0으로 줄이기
                x_ = Mathf.Lerp(x_, 0f, Time.deltaTime * 5f);
                z_ = Mathf.Lerp(z_, 0f, Time.deltaTime * 5f);

                // 변경된 x, z 값을 애니메이터에 전달
                playerAnimator.OnMovement(x_ * offset, z_ * offset);
            }


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
            if (Input.GetKeyDown(jumpKeyCode) && canControl)
            {
                //playerAnimator.OnJump();    // 애니메이션 파라미터 설정 (onJump)
                movement.JumpTo();        // 점프 함수 호출
            }

            Scene currentScene = SceneManager.GetActiveScene();

            if (currentScene.name == "Level_1")
            {
                if (Input.GetKeyDown(KeyCode.Z) && canControl && !GameManager.instance.IsAnyUIOpen())
                {
                    if (Time.time - lastKillTime >= killCooldown && (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMafia"])
                    {
                        playerAnimator.Kill();
                        lastKillTime = Time.time;
                        playerAttackCollision.SeletKillMember();
                    }
                }

                if (Input.GetKeyDown(KeyCode.R) && reportRadius.IsCorpseInRange())
                {
                    ReportCorpse();
                }
            }

            if (Input.GetKeyDown(KeyCode.V) && canControl)
            {
                // 마이크 상태를 토글합니다.
                recorder.TransmitEnabled = !recorder.TransmitEnabled;

                microphoneVolumeIndicator.UpdateMicrophoneUI(recorder.TransmitEnabled);
                
                // 마이크가 켜졌는지 꺼졌는지 상태를 로그로 출력
                Debug.Log("마이크 상태: " + (recorder.TransmitEnabled ? "켜짐" : "꺼짐"));
            }

            // Kill 시뮬레이트
            if (Input.GetKeyDown(KeyCode.P) && canControl)
            {
               photonView.RPC("Death", RpcTarget.All);
            }

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

            if (canControl)
            {
                float mouseX = Input.GetAxis("Mouse X");
                float mouseY = Input.GetAxis("Mouse Y");

                fpCameraController.RotateTo(mouseX, mouseY); // 카메라 회전 함수 호출
            }
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
    private void CloseAllActiveUIs()
    {
        if ( GameManager.instance.GetUIWindows() == null)
            return;
        // 활성화된 UI 목록을 새로운 리스트로 복사
        var activeUIs = GameManager.instance.GetUIWindows().ToList();

        foreach (var window in activeUIs)
        {
            if (window != null && window.activeSelf)
            {
                window.SetActive(false);
            }
        }
    }

    [PunRPC]
    void Death()
    {
        if (!photonView.IsMine) return;
        if (isDead) return;

        isDead = true;

        // 죽기 전 현재 아바타 상태를 저장
        string currentAvatar = avatarChanger.getCurrentAvatarName();

        // UI 창 닫기
        CloseAllActiveUIs();

        // 죽는 애니메이션 동안 다른 아바타들이 보이지 않도록 처리
        if (avatarChanger != null)
        {
            foreach (var avatar in avatarChanger.avatarDict.Values)
            {
                if (avatar != null && avatar != avatarChanger.avatarDict[currentAvatar])
                {
                    avatar.SetActive(false);
                }
            }
        }

        playerAnimator.Death();
        ShowObjects();
        cameraController.ShowTPCamera();

        punVoiceClient.PrimaryRecorder = null;
        punVoiceClient.SpeakerPrefab = null;

        StartCoroutine(HandleDeathWithAvatar(currentAvatar));

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

    private IEnumerator HandleDeathWithAvatar(string avatarName)
    {
        // 애니메이션 길이만큼 대기
        yield return new WaitForSeconds(playerAnimator.GetAnimatorTime().length);

        PhotonNetwork.Instantiate(ghostPrefab.name, transform.position, transform.rotation);

        // 죽은 아바타에 맞는 시체 프리팹 생성
        string corpseAvatar = "corpse_" + avatarName;
        GameObject corpsePrefab = Resources.Load<GameObject>(corpseAvatar);

        if (corpsePrefab != null)
        {
            PhotonNetwork.Instantiate(corpsePrefab.name, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogError("Prefab not found: " + corpseAvatar);
        }

        cameraController.ShowFPCamera();
        HideObjects();
        photonView.RPC("DisableGameObject", RpcTarget.All);
    }

    public void ChangeMoveSpeed(float multiplier = 2f)
    {
        playerMoveSpeedUnit *= multiplier;
    }

    public void OriginMoveSpeed(float divider = 2f)
    {
        playerMoveSpeedUnit /= divider;
    }

    private void ReportCorpse()
    {
        Debug.Log("Corpse reported!");
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

    // 모든 자식 오브젝트의 Renderer를 보이도록 설정하는 함수
    private void ShowObjects()
    {
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
            {
                Renderer[] renderers = GetAllRenderersIncludingInactive(obj);
                foreach (Renderer renderer in renderers)
                {
                    // 그림자와 함께 오브젝트가 보이도록 설정
                    renderer.shadowCastingMode = ShadowCastingMode.On;
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
