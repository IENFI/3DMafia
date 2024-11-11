using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomUI : MonoBehaviour
{

    //[SerializeField]
    //private List<Image> playerImgs;
    [SerializeField]
    private List<Button> mapiaCountButtons;
    [SerializeField]
    private List<Button> maxPlayerCountButtons;

    public CreateGameRoomData roomData;

    public NetworkManager networkManager;
    public WaitingRoomManager waitingRoomManager;

    // Start is called before the first frame update
    void Start()
    {
        // 원본 material이 바뀌는 오류를 위해 복제된 material 사용
        //for (int i = 0; i< playerImgs.Count; i++)
        //{
        //    Material materialInstance = Instantiate(playerImgs[i].material);
        //    playerImgs[i].material = materialInstance;
        //}

        roomData = new CreateGameRoomData() { mapiaCount = 1, maxPlayerCount = 10 };
        //UpdatePlayerImages();
    }
    
    public void UpdateMapiaCount(int count)
    {

        roomData.mapiaCount = count;
       
        GameManager.instance.mafiaNum = count;
        for (int i = 0; i < mapiaCountButtons.Count; i++)
        {
            if (i == count - 1)
            {
                mapiaCountButtons[i].image.color = new Color(1f, 1f, 1f, 27f / 255f);
            }
            else
            {
                mapiaCountButtons[i].image.color = new Color(1f, 1f, 1f, 0f);
            }
        }

        // 마피아가 1명이면 플레이어는 최소 4명, 2명은 7, 3명은 9, 4명은 10.
        int limitMaxPlayer = count == 1 ? 4 : count == 2 ? 7 : count == 3 ? 9 : count == 4 ? 10 : 12;
        if(roomData.maxPlayerCount < limitMaxPlayer)
        {
            UpdateMaxPlayerCount(limitMaxPlayer);
            waitingRoomManager.maxPlayerNum = limitMaxPlayer;

        }
        else
        {
            UpdateMaxPlayerCount(roomData.maxPlayerCount);
            waitingRoomManager.maxPlayerNum = roomData.maxPlayerCount;
        }

        for (int i = 0; i < maxPlayerCountButtons.Count; i++)
        {
            var text = maxPlayerCountButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if(i < limitMaxPlayer - 4)
            {
                maxPlayerCountButtons[i].interactable = false;
                text.color = Color.gray;
            }
            else
            {
                maxPlayerCountButtons[i].interactable = true;
                text.color = Color.white;
            }
        }

            }

    public void UpdateMaxPlayerCount(int count)
    {
        roomData.maxPlayerCount = count;

        GameManager.instance.maxPlayerNum = count;
        for (int i = 0; i < maxPlayerCountButtons.Count; i++)
        {
            if ( i == count -4)
            {
                maxPlayerCountButtons[i].image.color = new Color(1f, 1f, 1f, 27f / 255f);
            }
            else
            {
                maxPlayerCountButtons[i].image.color = new Color(1f, 1f, 1f, 0f);
            }
        }

        //UpdatePlayerImages()
    }

    //private void UpdatePlayerImages()
    //{
    //    for (int i = 0; i < playerImgs.Count; i ++ )
    //    {
    //        playerImgs[i].material.SetColor("_PlayerColor", Color.white);
    //    }
    //
    //    int mapiaCount = roomData.mapiaCount;
    //    int idx = 0;
    //    while(mapiaCount != 0)
    //    {
    //        if(idx >= roomData.maxPlayerCount)
    //        {
    //            idx = 0;
    //        }

    //        if (playerImgs[idx].material.GetColor("_PlayerColor") != Color.red && Random.Range(0, 5) == 0)
    //        {
    //            playerImgs[idx].material.SetColor("_PlyaerColor", Color.red);
    //            mapiaCount--;
    //        }
    //        idx++;
    //    }

    //    for (int i = 0; i < playerImgs.Count; i++)
    //    {
    //        if(i< roomData.maxPlayerCount)
    //        {
    //            playerImgs[i].gameObject.SetActive(true);
    //        }
    //        else
    //        {
    //            playerImgs[i].gameObject.SetActive(false);
    //        }
    //    }
    //}
}

public class CreateGameRoomData
{
    public int mapiaCount;
    public int maxPlayerCount;
}
