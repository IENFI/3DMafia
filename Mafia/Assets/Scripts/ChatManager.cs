using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class ChatManager : MonoBehaviour
{
    [Header("ETC")]
    public TextMeshProUGUI StatusText;
    public PhotonView PV;

    [Header("RoomPanel")]
    public GameObject RoomPanel;
    public TextMeshProUGUI ListText;
    public TextMeshProUGUI RoomInfoText;
    public TextMeshProUGUI[] ChatText;
    public TMP_InputField ChatInput;

    public GameObject VoteUI;

    bool chat_check = false;

    void Update()
    {
        // 현재 활성화된 씬을 가져옵니다.
        Scene currentScene = SceneManager.GetActiveScene();

        // 씬의 이름을 확인합니다.
        if (currentScene.name == "Level_0")
        {
            if (Input.GetKeyDown(KeyCode.T) && chat_check == false)
            {
                RoomPanel.SetActive(true);
                chat_check = true;
                ChatInput.ActivateInputField();
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && chat_check == true)
            {
                RoomPanel.SetActive(false);
                chat_check = false;
            }
            if (chat_check && Input.GetKeyDown(KeyCode.Return)) // 엔터키로 메시지 전송
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
        //else if (currentScene.name == "Level_1")
        //{
        //    if (Input.GetKeyDown(KeyCode.T) && chat_check == false && VoteUI.activeSelf)
        //    {
        //        RoomPanel.SetActive(true);
        //        chat_check = true;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.Escape) && chat_check == true && VoteUI.activeSelf)
        //    {
        //        RoomPanel.SetActive(false);
        //        chat_check = false;
        //    }
        //    else if (chat_check == true && !VoteUI.activeSelf)
        //    {
        //        RoomPanel.SetActive(false);
        //        chat_check = false;
        //    }
        //}
    }

    public void Send()
    {
        if (!string.IsNullOrWhiteSpace(ChatInput.text))
        {
            PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
            ChatInput.text = "";
        }

        ChatInput.ActivateInputField(); // 메시지 전송 후 포커스 유지
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
}
