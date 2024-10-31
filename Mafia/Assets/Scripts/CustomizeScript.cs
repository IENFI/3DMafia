using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomizeScript : MonoBehaviour
{
    public GameObject CustomizingUI;
    public Color originColor;
    private AvatarChanger player;
    public TMP_Text currentAvatarText;

    // Start is called before the first frame update
    void Start()
    {
        originColor = GetComponent<Renderer>().material.color;
        StartCoroutine(FindLocalAvatarChanger());

        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator FindLocalAvatarChanger()
    {
        while (player == null)
        {
            foreach (var pcc in FindObjectsOfType<AvatarChanger>())
            {
                if (pcc.photonView.IsMine)
                {
                    player = pcc;
                    break;
                }
            }

            if (player == null)
            {
                Debug.Log("(CustomizeScript) AvatarChanger를 찾는 중...");
            }

            // 다음 프레임까지 대기
            yield return null;
        }

        Debug.Log("(CustomizeScript) AvatarChanger를 찾았습니다.");

        // 현재 아바타 이름 표시
        if (currentAvatarText != null)
        {
            UpdateAvatarText(player.getCurrentAvatarName());
            Debug.Log("호출되었따 이말이야!");
        }

    }

    public void CustomizingButton(string avatarName)
    {
        if (player != null && player.photonView.IsMine)
        {
            player.ChangeAvatar(avatarName);
            UpdateAvatarText(avatarName);
        }
        else
        {
            Debug.LogWarning("Cannot change avatar: Player not found or not owned");
        }
    }
    private void UpdateAvatarText(string avatarName)
        {
            if (currentAvatarText != null)
            {
                switch (avatarName)
                {
                    case "naked":
                        currentAvatarText.text = "기본";
                        break;
                    case "builder":
                        currentAvatarText.text = "건축가";
                        break;
                    case "businessWoman":
                        currentAvatarText.text = "사업가";
                        break;
                    case "cashier":
                        currentAvatarText.text = "계산원";
                        break;
                    case "chef":
                        currentAvatarText.text = "요리사";
                        break;
                    case "fisherman":
                        currentAvatarText.text = "어부";
                        break;
                    case "miner":
                        currentAvatarText.text = "광부";
                        break;
                    case "nurse":
                        currentAvatarText.text = "간호사";
                        break;
                    case "police":
                        currentAvatarText.text = "경찰";
                        break;
                    case "security":
                        currentAvatarText.text = "경비원";
                        break;
                    case "worker":
                        currentAvatarText.text = "노동자";
                        break;
                    default:
                        currentAvatarText.text = "Current Avatar: " + avatarName;
                        break;
                }
            }
        }
}
