using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using ExitGames.Client.Photon; 

public class ChatManager : MonoBehaviourPunCallbacks
{
    [Header("ETC")]
    public PhotonView PV;
    

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public TextMeshProUGUI[] ChatText;
    public TMP_InputField ChatInput;

    public GameObject VoteUI;

    bool chat_check = false;

    [Header("프로필 사진")]
    public Sprite[] avatarSpriteImages;
    private Dictionary<string, int> avatarNameToSpriteIndex;
    public Image[] profileImage;

    void Awake()
    {
        // 아바타 이름과 스프라이트 인덱스 매핑 초기화
        avatarNameToSpriteIndex = new Dictionary<string, int>()
        {
            { "naked", 0 },
            { "builder", 1 },
            { "businessWoman", 2 },
            { "cashier", 3 },
            { "chef", 4 },
            { "fisherman", 5 },
            { "miner", 6 },
            { "nurse", 7 },
            { "police", 8 },
            { "security", 9 },
            { "worker", 10 }
        };

    }

    // public override void OnPlayerEnteredRoom(Player newPlayer)
    // {
    //     Debug.Log($"{newPlayer.NickName} has entered the room. Total players: {PhotonNetwork.CurrentRoom.PlayerCount}");
    //     // 플레이어가 들어올 때 호출할 로직
    // }

    // public override void OnPlayerLeftRoom(Player otherPlayer)
    // {
    //     Debug.Log($"{otherPlayer.NickName} has left the room. Total players: {PhotonNetwork.CurrentRoom.PlayerCount}");
    //     // 나간 플레이어의 닉네임을 포함한 채팅 메시지 삭제
    //     // RemovePlayerMessages(otherPlayer.NickName);
    // }

    // 플레이어가 나갔을 때 메세지를 삭제하려고 만든 함수인데, 미완성에 쓸지 말지 고민 중
    // private void RemovePlayerMessages(string playerName)
    // {
    //     for (int i = 0; i < ChatText.Length; i++)
    //     {
    //         if (ChatText[i].text.StartsWith($"{playerName} :"))
    //         {
    //             // 나간 플레이어의 메시지를 빈 문자열로 설정
    //             ChatText[i].text = "";

    //             // 프로필 이미지도 비활성화
    //             Image profileImage = ChatText[i].transform.Find("ProfileImage").GetComponent<Image>();
    //             if (profileImage != null)
    //             {
    //                 profileImage.sprite = null; // 프로필 이미지 초기화
    //                 profileImage.gameObject.SetActive(false);
    //             }
    //         }
    //     }
    // }

    private void RemovePlayerMessages1(string playerName)
    {
        for (int i = 0; i < ChatText.Length; i++)
        {
            if (ChatText[i].text.StartsWith($"{playerName} :"))
            {
                // 나간 플레이어의 메시지를 찾은 이후로 한 칸씩 위로 당깁니다.
                for (int j = i; j < ChatText.Length - 1; j++)
                {
                    ChatText[j].text = ChatText[j + 1].text;

                    // 프로필 이미지 업데이트
                    Image profileImage = ChatText[j].transform.Find("ProfileImage").GetComponent<Image>();
                    Image nextProfileImage = ChatText[j + 1].transform.Find("ProfileImage").GetComponent<Image>();

                    profileImage.sprite = nextProfileImage.sprite;
                    profileImage.gameObject.SetActive(nextProfileImage.gameObject.activeSelf);
                }

                // 마지막 메시지 칸 초기화 및 프로필 이미지 비활성화
                ChatText[ChatText.Length - 1].text = "";
                Image lastProfileImage = ChatText[ChatText.Length - 1].transform.Find("ProfileImage").GetComponent<Image>();
                lastProfileImage.sprite = null;
                lastProfileImage.gameObject.SetActive(false);
                break;
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // 아바타가 변경된 경우 `AvatarName` 속성이 포함된 업데이트만 반영
        if (changedProps.ContainsKey("AvatarName"))
        {
            UpdateChatProfileImages();
        }
    }

    private void UpdateChatProfileImages()
    {
        for (int i = 0; i < ChatText.Length; i++)
        {
            // 메시지의 작성자 이름을 파싱하여 가져오기
            string[] splitMessage = ChatText[i].text.Split(new[] { " : " }, StringSplitOptions.None);
            if (splitMessage.Length < 2) continue;

            string senderName = splitMessage[0];
            
            // 해당 플레이어의 아바타 이름 가져오기
            Player senderPlayer = Array.Find(PhotonNetwork.PlayerList, p => p.NickName == senderName);
            if (senderPlayer == null || !senderPlayer.CustomProperties.TryGetValue("AvatarName", out object avatarNameObj)) continue;

            string avatarName = (string)avatarNameObj;

            // avatarName에 맞는 스프라이트 인덱스를 가져와 프로필 이미지 설정
            if (avatarNameToSpriteIndex.TryGetValue(avatarName, out int spriteIndex))
            {
                if (spriteIndex < avatarSpriteImages.Length && avatarSpriteImages[spriteIndex] != null)
                {
                    ChatText[i].transform.Find("ProfileImage").GetComponent<Image>().sprite = avatarSpriteImages[spriteIndex];
                }
            }
        }
    }
    void Update()
    {
        // 현재 활성화된 씬을 가져옵니다.
        Scene currentScene = SceneManager.GetActiveScene();
        // 씬의 이름을 확인합니다.
        if (currentScene.name == "Level_0")
        {
            HandleChatToggle(KeyCode.T, KeyCode.Escape);
            if (RoomPanel.activeSelf  && Input.GetKeyDown(KeyCode.Return)) // 엔터키로 메시지 전송
            {
                Send();
            }
        }
        else if (currentScene.name == "Level_1" && VoteUI.activeSelf)
        {
            ChatInput.ActivateInputField();
            if (Input.GetKeyDown(KeyCode.Return)) // 엔터키로 메시지 전송
            {
                Send();
            }
        }

        if(RoomPanel.activeSelf && Input.GetKeyDown(KeyCode.Return)){
            ChatInput.ActivateInputField();
        }
    }

    private void HandleChatToggle(KeyCode openKey, KeyCode closeKey)
    {
        if (Input.GetKeyDown(openKey) && !RoomPanel.activeSelf)
        {
            RoomPanel.SetActive(true);
            ChatInput.ActivateInputField();
        }
        else if (Input.GetKeyDown(closeKey) && RoomPanel.activeSelf )
        {
            RoomPanel.SetActive(false);
        }
    }

    public void Send()
    {
        if (!string.IsNullOrWhiteSpace(ChatInput.text))
        {
            // 현재 플레이어의 아바타 이름을 가져옵니다. (프로필 사진)
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("AvatarName", out object avatarNameObj);
            string avatarName = (string)avatarNameObj;

            PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName, ChatInput.text, avatarName);
            ChatInput.text = "";
        }
        ChatInput.ActivateInputField();
    }


    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string senderName, string msg, string avatarName)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = $"{senderName} : {msg}";

                // avatarName을 기반으로 스프라이트 인덱스를 가져와서 프로필 이미지를 설정
                Image profileImg = ChatText[i].transform.Find("ProfileImage").GetComponent<Image>();
                if (avatarNameToSpriteIndex.TryGetValue(avatarName, out int spriteIndex))
                {
                    if (spriteIndex < avatarSpriteImages.Length && avatarSpriteImages[spriteIndex] != null)
                    {
                        profileImg.gameObject.SetActive(true); // 프로필 이미지 활성화
                        profileImg.sprite = avatarSpriteImages[spriteIndex];
                    }
                }
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++)
            {
                ChatText[i - 1].text = ChatText[i].text;
                Image currentProfileImg = ChatText[i].transform.Find("ProfileImage").GetComponent<Image>();
                Image previousProfileImg = ChatText[i - 1].transform.Find("ProfileImage").GetComponent<Image>();

                previousProfileImg.sprite = currentProfileImg.sprite;
                previousProfileImg.gameObject.SetActive(currentProfileImg.gameObject.activeSelf); // 이전 이미지의 활성화 상태를 현재 상태로 설정
            }

            // 마지막 슬롯에 새 프로필 이미지 설정
            ChatText[ChatText.Length - 1].text = $"{senderName} : {msg}";
            Image lastProfileImg = ChatText[ChatText.Length - 1].transform.Find("ProfileImage").GetComponent<Image>();
            if (avatarNameToSpriteIndex.TryGetValue(avatarName, out int spriteIndex))
            {
                if (spriteIndex < avatarSpriteImages.Length && avatarSpriteImages[spriteIndex] != null)
                {
                    lastProfileImg.sprite = avatarSpriteImages[spriteIndex];
                    lastProfileImg.gameObject.SetActive(true); // 마지막 슬롯 프로필 이미지 활성화
                }
            }
        }
    }
}
