using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using TMPro;
public class AvatarCustomizer : MonoBehaviourPunCallbacks
{
    public Button[] avatarButtons;
    public TMP_Text[] avatarUsernameTexts;
    public GameObject[] checkMarks;
    private Dictionary<string, int> avatarNameToIndex = new Dictionary<string, int>();
    private const string AVATAR_PROP_KEY = "AvatarName";
    void Start()
    {
        // 아바타 이름과 인덱스 매핑
        for (int i = 0; i < avatarButtons.Length; i++)
        {
            avatarNameToIndex[avatarButtons[i].name] = i;
        }
        // 초기 UI 설정
        UpdateUI();
    }
    public void SelectAvatar(string avatarName)
    {
        if (PhotonNetwork.IsMessageQueueRunning)
        {
            ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
            customProperties[AVATAR_PROP_KEY] = avatarName;
            PhotonNetwork.LocalPlayer.SetCustomProperties(customProperties);
        }
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (changedProps.ContainsKey(AVATAR_PROP_KEY))
        {
            UpdateUI();
        }
    }
    private void UpdateUI()
    {
        // 모든 버튼 초기화
        for (int i = 0; i < avatarButtons.Length; i++)
        {
            avatarButtons[i].interactable = true;
            avatarUsernameTexts[i].text = "";
            checkMarks[i].SetActive(false);
        }
        // 각 플레이어의 선택에 따라 UI 업데이트
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue(AVATAR_PROP_KEY, out object avatarName))
            {
                if (avatarNameToIndex.TryGetValue((string)avatarName, out int index))
                {
                    avatarButtons[index].interactable = false;
                    avatarUsernameTexts[index].text = player.NickName;
                    checkMarks[index].SetActive(player == PhotonNetwork.LocalPlayer);
                }
            }
        }
    }
    public override void OnJoinedRoom()
    {
        UpdateUI();
    }
}