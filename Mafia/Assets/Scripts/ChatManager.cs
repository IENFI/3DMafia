using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

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

    void Update()
    {
        // ChatInput이 포커스된 상태에서는 특정 키 입력을 무시
        if (ChatInput.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.K) ||
                Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.V))
            {
                Debug.Log("Input ignored while ChatInput is focused.");
                return; // 특정 키 입력을 무시하고 리턴
            }
        }

        // ChatInput이 포커스되지 않은 상태에서는 일반적인 처리
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (RoomPanel.activeSelf)
            {
                RoomPanel.SetActive(false);
            }
            else
            {
                RoomPanel.SetActive(true);
            }
        }
        
    }



    void Start()
    {
        // 입력 필드의 Submit 이벤트 리스너 등록
        ChatInput.onSubmit.AddListener(OnSubmit);
    }

    // 입력 필드에서 Enter 키 입력 시 호출될 메서드
    void OnSubmit(string input)
    {
        Send();
    }

    public void Send()
    {
        if (string.IsNullOrEmpty(ChatInput.text))
        {
            return;
        }

        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }

    [PunRPC]
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
        {
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        }

        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++)
            {
                ChatText[i - 1].text = ChatText[i].text;
            }
            ChatText[ChatText.Length - 1].text = msg;
        }
    }
}
