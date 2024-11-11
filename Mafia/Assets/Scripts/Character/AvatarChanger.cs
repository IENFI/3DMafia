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
    public GameObject naked;

    private Dictionary<string, GameObject> avatarDict = new Dictionary<string, GameObject>();
    private const string targetSceneName = "Level_0";
    private string currentSceneName;

    [SerializeField]
    private string currentAvatarName = "start";

    Renderer[] Avatar;

    public string getCurrentAvatarName()
    {
        return currentAvatarName;
    }

    private void Awake()
    {
        avatarDict.Add("naked", naked); 
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
        // currentAvatarName = PlayerPrefs.GetString(playerPrefKey, "builder");

        /*if (photonView.IsMine)
            {// 사용하지 않은 외형을 DB에서 무작위로 선택
            string roomID = PhotonNetwork.CurrentRoom.CustomProperties["RoomID"].ToString();
            int clientID = PhotonNetwork.LocalPlayer.ActorNumber;

            DBInteraction.GetRandomUnusedAppearance(roomID, (randomUnusedAppearance) =>
            {
                if (!string.IsNullOrEmpty(randomUnusedAppearance))
                {
                    // 선택된 외형을 적용
                    ChangeAvatar(randomUnusedAppearance);

                    // 해당 외형을 DB에 현재 클라이언트 ID로 업데이트
                    DBInteraction.SetAppearanceClientID(roomID, randomUnusedAppearance, clientID);
                }
                else
                {
                    Debug.LogWarning("No available unused appearance found.");
                }
            });
            InitializeAvatar();
        }
        else
        {
            // 다른 플레이어의 아바타 상태를 요청
            photonView.RPC("RequestAvatarState", RpcTarget.All);
        }*/

        if (photonView.IsMine)
        {
            string roomID = PhotonNetwork.CurrentRoom.CustomProperties["RoomID"].ToString();
            int clientID = PhotonNetwork.LocalPlayer.ActorNumber;

            // Level_0에서만 랜덤 아바타 할당
            if (currentSceneName == "Level_0")
            {
                DBInteraction.GetRandomUnusedAppearance(roomID, (randomUnusedAppearance) =>
                {
                    if (!string.IsNullOrEmpty(randomUnusedAppearance))
                    {
                        ChangeAvatar(randomUnusedAppearance);
                        DBInteraction.SetAppearanceClientID(roomID, randomUnusedAppearance, clientID);
                    }
                });
            }
            else if (currentSceneName == "Level_1")
            {
                // Level_1에서는 이전 씬의 아바타 상태를 복원
                RestoreAvatarState(roomID, clientID);
            }

            InitializeAvatar();
        }
        else
        {
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
        Debug.Log("InitializeAvatar 실행");
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
            Debug.LogError($"Invalid avatar name: {currentAvatarName}. xDefaulting to builderx.");
            // avatarDict["builder"].SetActive(true);
            // currentAvatarName = "builder";
        }

        string roomID = PhotonNetwork.CurrentRoom.CustomProperties["RoomID"].ToString();
        int clientID = PhotonNetwork.LocalPlayer.ActorNumber;
        DBInteraction.SetAppearanceClientID(roomID, currentAvatarName, clientID);

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
        string roomID = PhotonNetwork.CurrentRoom.CustomProperties["RoomID"].ToString();
        int clientID = PhotonNetwork.LocalPlayer.ActorNumber;

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
        if (currentAvatarName != "start"){
            avatarDict[currentAvatarName].SetActive(false);
            DBInteraction.ResetAppearanceByCharacterType(roomID, currentAvatarName);
        }
        avatarDict[newAvatarName].SetActive(true);
        currentAvatarName = newAvatarName;

        // 각 클라이언트별로 고유한 PlayerPrefs 키를 사용
        string playerPrefKey = $"CurrentAvatarName_{PhotonNetwork.LocalPlayer.ActorNumber}";
        PlayerPrefs.SetString(playerPrefKey, currentAvatarName);
        PlayerPrefs.Save();


        DBInteraction.SetAppearanceClientID(roomID, newAvatarName, clientID);

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

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string roomID = PhotonNetwork.CurrentRoom.CustomProperties["RoomID"].ToString();
            List<int> activeClientIDs = new List<int>();

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                activeClientIDs.Add(player.ActorNumber);
            }

            DBInteraction.ResetUnusedAppearances(roomID, activeClientIDs);
        }
    }

    private void RestoreAvatarState(string roomID, int clientID)
    {
        // 현재 클라이언트의 아바타 상태를 DB에서 조회
        string query = $@"
            SELECT 
                CASE 
                    WHEN builder = {clientID} THEN 'builder'
                    WHEN businessWoman = {clientID} THEN 'businessWoman'
                    WHEN cashier = {clientID} THEN 'cashier'
                    WHEN chef = {clientID} THEN 'chef'
                    WHEN fisherman = {clientID} THEN 'fisherman'
                    WHEN miner = {clientID} THEN 'miner'
                    WHEN nurse = {clientID} THEN 'nurse'
                    WHEN police = {clientID} THEN 'police'
                    WHEN security = {clientID} THEN 'security'
                    WHEN worker = {clientID} THEN 'worker'
                    WHEN naked = {clientID} THEN 'naked'
                END as avatarType
            FROM customize 
            WHERE roomID = '{roomID}'";

        AWSDBManager.ReadData(query, (reader) =>
        {
            if (reader.Read() && !reader.IsDBNull(0))
            {
                string savedAvatarType = reader.GetString(0);
                ChangeAvatar(savedAvatarType);
            }
        });
    }
}