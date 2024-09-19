using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class MinigameBase : MonoBehaviourPun
{
    public abstract void ReceiveToken();
    public abstract bool GetActive();
}

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] minigames;  // 미니게임 "UI" 들을 담을 배열
    private GameObject activeMinigame;  // 활성화된 미니게임

    // 페이즈가 변경될 때 호출
    public void AssignRandomMinigame()
    {
        Debug.Log("AssignRandomMinigame 함수 실행");
        // 모든 미니게임을 비활성화
        // foreach (GameObject minigame in minigames)
        // {
        //     minigame.SetActive(false);
        // }

        // 랜덤으로 미니게임 선택 (현재 한 개만 선택)
        int randomIndex = Random.Range(0, minigames.Length);
        activeMinigame = minigames[randomIndex];

        // 선택된 미니게임에 토큰 전달
        SendTokenToMinigame();
    }

    // 선택된 미니게임에 토큰 전달
    private void SendTokenToMinigame()
    {
        // 선택된 미니게임의 MinigameBase 클래스 참조
        MinigameBase minigameScript = activeMinigame.GetComponent<MinigameBase>();
        if (minigameScript != null)
        {
            minigameScript.ReceiveToken();
        }
        else
        {
            Debug.LogError("선택된 미니게임에 MinigameBase 스크립트가 없습니다.");
        }
    }
}
