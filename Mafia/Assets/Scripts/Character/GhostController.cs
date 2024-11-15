using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Voice.PUN;
using Photon.Realtime;

public class GhostController : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private KeyCode jumpKeyCode = KeyCode.Space;
    [SerializeField]
    private Transform cameraTransform;
    [SerializeField]
    private GhostFPCamera cameraController;
    private GhostMovement movement;
    [SerializeField]
    private Camera ghostCamera;

    public TextMeshProUGUI nickName;
    public PhotonView PV;

    [SerializeField]
    private GameObject miniMapPointPrefab1; // MiniMapPoint 프리팹
    private GameObject miniMapPoint1; // MiniMapPoint 인스턴스

    [SerializeField]
    private GameObject miniMapPointPrefab2; // MiniMapPoint 프리팹
    private GameObject miniMapPoint2; // MiniMapPoint 인스턴스

    public bool GhostIsMafia = false;
    private GameObject GameUIManager;

    [Header("보이스")]
    private const byte PlayerGroup = 1; // 플레이어 그룹 ID
    private const byte GhostGroup = 2; // 유령 그룹 ID
    Recorder recorder;
    Speaker speaker;
    private PunVoiceClient punVoiceClient; // PunVoiceClient 참조
    public MicrophoneVolumeIndicator microphoneVolumeIndicator;

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
        movement = GetComponent<GhostMovement>();
        cameraController = GetComponentInChildren<GhostFPCamera>();
        // 유령은 모든 레이어를 볼 수 있도록 설정
        ghostCamera.cullingMask = ~0;

        if (photonView.IsMine)
        {
            GameUIManager = GameObject.Find("GameUiManager");
            // 유령 킬, 신고 버튼 삭제

            GameUIManager.transform.Find("Canvas/KillImage").gameObject.SetActive(false);
            GameUIManager.transform.Find("Canvas/ReportImage").gameObject.SetActive(false);

            recorder = GetComponent<Recorder>();
            speaker = GetComponentInChildren<Speaker>();
            if (recorder != null){
                // recorder.RestartRecording();
                Debug.Log("Recorder started recording.");
            }
            else {
                Debug.LogWarning("Recorder component not found on this object.");
            }

            punVoiceClient = GameManager.instance.GetComponent<PunVoiceClient>();
            if (punVoiceClient != null)
            {
                // LocalPlayer의 Recorder와 Speaker를 PunvoiceClient에 설정
                punVoiceClient.PrimaryRecorder = GetComponent<Recorder>();
                punVoiceClient.SpeakerPrefab = GetComponentInChildren<Speaker>().gameObject;
            }

            StartCoroutine(ConfigureVoiceSetting());
            microphoneVolumeIndicator.UpdateMicrophoneUI(recorder.TransmitEnabled);

            UpdateMafiaNicknames();
        }

        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("isMafia") && (bool)PhotonNetwork.LocalPlayer.CustomProperties["isMafia"])
        {
            GhostIsMafia = true;
        }

        nickName.text = PV.Owner.NickName;

        if (PV.IsMine && GhostIsMafia)
        {
            PV.RPC("SetNicknameColorRed", RpcTarget.AllBuffered);
        }

        CreateMiniMapPoint2();
    }

    private IEnumerator ConfigureVoiceSetting() {
        yield return new WaitUntil(() => PhotonNetwork.InRoom && PunVoiceClient.Instance.Client.State == ClientState.Joined);

        if (recorder != null){
            recorder.InterestGroup = GhostGroup;
            recorder.TransmitEnabled = true;
            Debug.Log("GhostController : Recorder configured for GhostGroup (2) for all ghosts.");
        }

        if (PunVoiceClient.Instance.Client.State == ClientState.Joined)
        {
            PunVoiceClient.Instance.Client.OpChangeGroups(null, new byte[] { GhostGroup });
            Debug.Log("GhostController : Listening to GhostGroup (2) for all ghosts.");
        }
    }

    [PunRPC]
    void SetNicknameColorRed()
    {
        nickName.color = Color.red;
    }

    private void UpdateMafiaNicknames()
    {
        // 모든 PlayerController를 찾아서 isMafia 상태를 확인합니다.
        PlayerController[] allPlayerControllers = FindObjectsOfType<PlayerController>();
        foreach (PlayerController controller in allPlayerControllers)
        {
            if (controller.PV.Owner.CustomProperties.ContainsKey("isMafia") &&
                (bool)controller.PV.Owner.CustomProperties["isMafia"])
            {
                controller.UpdateNicknameColor(Color.red);
            }
        }
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

            // Space키를 누르면 점프
            if (Input.GetKeyDown(jumpKeyCode))
            {
                //playerAnimator.OnJump();    // 애니메이션 파라미터 설정 (onJump)
                movement.JumpTo();        // 점프 함수 호출
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                // 마이크 상태를 토글합니다.
                recorder.TransmitEnabled = !recorder.TransmitEnabled;

                microphoneVolumeIndicator.UpdateMicrophoneUI(recorder.TransmitEnabled);
                
                // 마이크가 켜졌는지 꺼졌는지 상태를 로그로 출력
                Debug.Log("마이크 상태: " + (recorder.TransmitEnabled ? "켜짐" : "꺼짐"));
            }


            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            cameraController.RotateTo(mouseX, mouseY);
            UpdateMiniMapPointPosition2();

            // Update엔 필요 없지 않을까?
            // punVoiceClient = GameManager.instance.GetComponent<PunVoiceClient>();
            // if (punVoiceClient != null)
            // {
            //     // LocalPlayer의 Recorder와 Speaker를 PunvoiceClient에 설정
            //     punVoiceClient.PrimaryRecorder = GetComponent<Recorder>();
            //     punVoiceClient.SpeakerPrefab = GetComponentInChildren<Speaker>().gameObject;
            // }
        }
    }
    private void CreateMiniMapPoint2()
    {
        if (miniMapPointPrefab1 != null)
        {
            miniMapPoint1 = Instantiate(miniMapPointPrefab1, transform.position + new Vector3(150f, 0, 0), Quaternion.identity);
            miniMapPoint1.transform.SetParent(transform); // MiniMapPoint를 플레이어의 자식으로 설정

            miniMapPoint2 = Instantiate(miniMapPointPrefab2, transform.position + new Vector3(300f, 0, 0), Quaternion.identity);
            miniMapPoint2.transform.SetParent(transform); // MiniMapPoint를 플레이어의 자식으로 설정
        }
    }

    private void UpdateMiniMapPointPosition2()
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
