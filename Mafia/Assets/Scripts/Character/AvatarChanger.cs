using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class AvatarChanger : MonoBehaviourPunCallbacks
{
    public GameObject avatarParent;
    public GameObject builder;
    public GameObject business_woman;
    public GameObject cashier;
    public GameObject chef;
    public GameObject fisherman;
    public GameObject miner;
    public GameObject nurse;
    public GameObject police;
    public GameObject security;
    public GameObject worker;

    private Dictionary<string, GameObject> avatarDict = new Dictionary<string, GameObject>();
    private const string targetSceneName = "Level_0";
    private string currentSceneName;

    [SerializeField]
    private string currentAvatarName = "builder";

    Renderer[] Avatar;

    public string getCurrentAvatarName()
    {
        return currentAvatarName;
    }

    private void Awake()
    {
        avatarDict.Add("builder", builder);
        avatarDict.Add("businessWoman", business_woman);
        avatarDict.Add("cashier", cashier);
        avatarDict.Add("chef", chef);
        avatarDict.Add("fisherman", fisherman);
        avatarDict.Add("miner", miner);
        avatarDict.Add("nurse", nurse);
        avatarDict.Add("police", police);
        avatarDict.Add("security", security);
        avatarDict.Add("worker", worker);

        foreach (var avatar in avatarDict)
        {
            if (avatar.Value == null)
            {
                Debug.LogError($"The '{avatar.Key}' GameObject is null.");
            }
        }
    }

    private void Start()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        // 각 클라이언트별로 고유한 PlayerPrefs 키를 사용
        string playerPrefKey = $"CurrentAvatarName_{PhotonNetwork.LocalPlayer.ActorNumber}";
        currentAvatarName = PlayerPrefs.GetString(playerPrefKey, "builder");

        if (photonView.IsMine)
        {
            InitializeAvatar();
        }
        else
        {
            // 다른 플레이어의 아바타 상태를 요청
            photonView.RPC("RequestAvatarState", RpcTarget.All);
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
        Avatar = GetComponent<PlayerController>().GetAllRenderersIncludingInactive(avatarParent);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string newSceneName = scene.name;
        if (newSceneName != currentSceneName)
        {
            Debug.Log("Scene changed to: " + newSceneName);
            currentSceneName = newSceneName;

            if (photonView.IsMine)
            {
                InitializeAvatar();
            }
        }
    }

    private void InitializeAvatar()
    {
        if (!photonView.IsMine)
        {
            Debug.LogWarning("Attempting to initialize avatar on non-owned player");
            return;
        }

        foreach (var avatar in avatarDict.Values)
        {
            avatar.SetActive(false);
        }

        if (avatarDict.TryGetValue(currentAvatarName, out GameObject currentAvatar))
        {
            currentAvatar.SetActive(true);
        }
        else
        {
            Debug.LogError($"Invalid avatar name: {currentAvatarName}. Defaulting to builder.");
            avatarDict["builder"].SetActive(true);
            currentAvatarName = "builder";
        }

        // Sync with other clients
        Hashtable props = new Hashtable() { { "AvatarName", currentAvatarName } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!photonView.IsMine && targetPlayer == photonView.Owner && changedProps.ContainsKey("AvatarName"))
        {
            string newAvatarName = (string)changedProps["AvatarName"];
            UpdateRemotePlayerAvatar(newAvatarName);
        }
    }

    private void UpdateRemotePlayerAvatar(string newAvatarName)
    {
        foreach (var avatar in avatarDict.Values)
        {
            avatar.SetActive(false);
        }

        if (avatarDict.TryGetValue(newAvatarName, out GameObject newAvatar))
        {
            newAvatar.SetActive(true);
        }
        else
        {
            Debug.LogError($"Invalid avatar name for remote player: {newAvatarName}");
        }
    }

    public void ChangeAvatar(string newAvatarName)
    {
        if (!photonView.IsMine)
        {
            Debug.LogWarning("Attempt to change avatar on non-owned player");
            return;
        }

        if (!avatarDict.ContainsKey(newAvatarName))
        {
            Debug.LogError($"Invalid avatar name: {newAvatarName}");
            return;
        }

        avatarDict[currentAvatarName].SetActive(false);
        avatarDict[newAvatarName].SetActive(true);
        currentAvatarName = newAvatarName;

        // 각 클라이언트별로 고유한 PlayerPrefs 키를 사용
        string playerPrefKey = $"CurrentAvatarName_{PhotonNetwork.LocalPlayer.ActorNumber}";
        PlayerPrefs.SetString(playerPrefKey, currentAvatarName);
        PlayerPrefs.Save();

        Debug.Log($"Changed avatar to: {newAvatarName} for player {PhotonNetwork.LocalPlayer.ActorNumber}");

        // Sync with other clients
        Hashtable props = new Hashtable() { { "AvatarName", currentAvatarName } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void Update()
    {
        // Debug.Log($"Player {photonView.Owner.ActorNumber} - CurrentAvatarName: {currentAvatarName}");
    }

    [PunRPC]
    private void RequestAvatarState()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("ReceiveAvatarState", RpcTarget.All, currentAvatarName);
        }
    }

    [PunRPC]
    private void ReceiveAvatarState(string avatarName)
    {
        if (!photonView.IsMine)
        {
            UpdateRemotePlayerAvatar(avatarName);
        }
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // 방에 입장할 때 모든 플레이어의 아바타 상태를 요청
        photonView.RPC("RequestAvatarState", RpcTarget.All);
    }
}