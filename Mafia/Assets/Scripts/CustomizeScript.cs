using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class CustomizeScript : MonoBehaviour
{
    public GameObject CustomizingUI;
    public Color originColor;
    private AvatarChanger player;

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

    }

    public void CustomizingButton(string avatarName)
    {
        player.ChangeAvatar(avatarName);
    }
}
