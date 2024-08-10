using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AvatarChanger : MonoBehaviourPunCallbacks
{
    // 현재 사용중인 아바타의 부모 오브젝트
    public GameObject avatarParent;

    // 변경할 새로운 아바타 프리팹
    public GameObject builder;
    public GameObject business_woman;
    public GameObject cashier;
    public GameObject chef;

    private Dictionary<string, GameObject> avatarDict = new Dictionary<string, GameObject>();

    // 특정 씬 이름을 상수로 정의
    private const string targetSceneName = "Level_0";
    private string currentSceneName;

    [SerializeField]
    private string currentAvatarName = "builder";

    Renderer[] Avatar;

    // RPC 호출을 큐에 저장
    private Queue<System.Action> rpcQueue = new Queue<System.Action>();


    private void Awake()
    {
        avatarDict.Add("builder", builder);
        if (builder == null)
        {
            Debug.LogError("The 'builder' GameObject is null.");
        }
        avatarDict.Add("businessWoman", business_woman);
        avatarDict.Add("cashier", cashier);
        avatarDict.Add("chef", chef);
    }

    private void Start()
    {
        // 시작할 때 현재 활성화된 씬의 이름을 저장
        currentSceneName = SceneManager.GetActiveScene().name;
        currentAvatarName = PlayerPrefs.GetString("CurrentAvatarName", "builder");
        //if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("currentAvatarName", out object avatarNameObj))
        //{
        //    currentAvatarName = (string)avatarNameObj;
        //}
        SceneChangeAvatar(currentAvatarName);

        // 씬이 로드될 때마다 호출되는 이벤트에 핸들러를 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
        Avatar = GetComponent<PlayerController>().GetAllRenderersIncludingInactive(avatarParent);

        ProcessQueuedRPCs();
    }

    private void ProcessQueuedRPCs()
    {
        // 큐에 저장된 RPC 호출을 실행
        while (rpcQueue.Count > 0)
        {
            var rpcAction = rpcQueue.Dequeue();
            rpcAction();
        }
    }

    void OnDestroy()
    {
        // 씬 로드 이벤트 핸들러 등록 해제 (메모리 누수를 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때 호출되는 메소드
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬의 이름을 저장
        string newSceneName = scene.name;

        // 씬이 바뀌었는지 확인
        if (newSceneName != currentSceneName)
        {
            // 씬이 바뀌었을 때 실행할 작업
            Debug.Log("Scene changed to: " + newSceneName);

            // 현재 씬 이름 업데이트
            currentSceneName = newSceneName;

            // 특정 작업을 한 번만 실행
            //HandleSceneChange(newSceneName);
        }
        currentAvatarName = PlayerPrefs.GetString("CurrentAvatarName", "builder");
        //if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("currentAvatarName", out object avatarNameObj))
        //{
        //    currentAvatarName = (string)avatarNameObj;
        //}
        SceneChangeAvatar(currentAvatarName);
    }

    //private void HandleSceneChange(string newSceneName)
    //{
    //    // 특정 씬에서만 작업을 실행
    //    if (newSceneName == "TargetScene")
    //    {
    //        Debug.Log("Special handling for TargetScene.");
    //        // 여기서 원하는 작업을 수행
    //    }
    //}

    private void Update()
    {
        
    }

    private void SceneChangeAvatar(string newAvatarName)
    {
        Debug.Log("SceneChangeAvatar");

        GameObject currentAvatar = avatarDict["builder"];
        currentAvatar.SetActive(false);
        string lastAvatarName = "builder";

        switch (newAvatarName)
        {
            case "builder":
                builder.SetActive(true);
                currentAvatarName = "builder";
                break;
            case "businessWoman":
                business_woman.SetActive(true);
                currentAvatarName = "businessWoman";
                break;
            case "cashier":
                cashier.SetActive(true);
                currentAvatarName = "cashier";
                break;
            case "chef":
                chef.SetActive(true);
                currentAvatarName = "chef";
                break;
            default:
                Debug.LogError("Invalid avatar name");
                return;
        }

        // PlayerPrefs에 새 아바타 이름 저장
        PlayerPrefs.SetString("CurrentAvatarName", currentAvatarName);
        //PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "currentAvatarName", currentAvatarName } });


        // 변경 사항을 즉시 저장
        PlayerPrefs.Save();

        Debug.Log("newAvatarName : "+ newAvatarName);
        // 네트워크 동기화를 위해 RPC 호출
        photonView.RPC("RPC_ChangeAvatar", RpcTarget.OthersBuffered, photonView.ViewID, lastAvatarName, newAvatarName);
    }

    // 현재 아바타를 교체하는 메서드
    public void ChangeAvatar(string newAvatarName)
    {

        Debug.Log("ChangeAvatar");

        GameObject currentAvatar = avatarDict[currentAvatarName];
        currentAvatar.SetActive(false);
        string lastAvatarName = currentAvatarName;

        switch (newAvatarName)
        {
            case "builder":
                builder.SetActive(true);
                currentAvatarName = "builder";
                break;
            case "businessWoman":
                business_woman.SetActive(true);
                currentAvatarName = "businessWoman";
                break;
            case "cashier":
                cashier.SetActive(true);
                currentAvatarName = "cashier";
                break;
            case "chef":
                chef.SetActive(true);
                currentAvatarName = "chef";
                break;
            default:
                Debug.LogError("Invalid avatar name");
                return;
        }

        // PlayerPrefs에 새 아바타 이름 저장
        PlayerPrefs.SetString("CurrentAvatarName", currentAvatarName);
        //PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "currentAvatarName", currentAvatarName } });

        // 변경 사항을 즉시 저장
        PlayerPrefs.Save();


        // 네트워크 동기화를 위해 RPC 호출
        photonView.RPC("RPC_ChangeAvatar", RpcTarget.OthersBuffered, photonView.ViewID, lastAvatarName, newAvatarName);
    }

    // RPC로 다른 클라이언트에서 아바타를 변경하는 메서드
    [PunRPC]
    public void RPC_ChangeAvatar(int viewID, string lastAvatarName, string newAvatarName)
    {
        Debug.Log("RPC_ChangeAvatar");

        // 전달된 viewID를 이용해 정확한 오브젝트 찾기
        PhotonView targetPhotonView = PhotonView.Find(viewID);
        if (targetPhotonView == null)
        {
            Debug.LogError("PhotonView not found for viewID: " + viewID);
            return;
        }

        if(avatarDict.TryGetValue(lastAvatarName, out GameObject avatar))
        {
            // 해당 오브젝트의 현재 아바타를 비활성화
            targetPhotonView.GetComponent<AvatarChanger>().avatarDict[lastAvatarName].SetActive(false);
        }
        else
        {
            if (lastAvatarName == "builder")
            {
                avatarDict.Add(lastAvatarName, builder);
            }
        }

        targetPhotonView.GetComponent<AvatarChanger>().avatarDict[newAvatarName].SetActive(true);
    }
}
