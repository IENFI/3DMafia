using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro; 

public abstract class MinigameBase : MonoBehaviourPun
{
    public abstract void ReceiveToken();
    public abstract bool GetActive();
    public abstract MinigameManager GetMinigameManager();
}

public class MinigameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] minigames;  // 미니게임 "UI" 들을 담을 배열
    private string[] minigamesNameList = {"전등 회로 맞추기", "어려운 수학 문제 풀기"};
    private GameObject activeMinigame;  // 활성화된 미니게임
    public TextMeshProUGUI[] minigameNameText; // UI에 표시될 미니게임 이름
    public GameObject[] successCheckbox;       // 성공 시 활성화할 체크박스
    private int currentMinigameIndex = 0;
    private Dictionary<int, int> indexDic; // key가 mission의 고유 index, value가 부여 받은 index(curent)

    private void Awake(){
        // minigameNameText 배열의 모든 요소를 빈 문자열로 초기화
        for (int i = 0; i < minigameNameText.Length; i++)
        {
            minigameNameText[i].text = ""; // 빈 문자열로 초기화
        }

        // successCheckbox 배열의 모든 체크박스를 비활성화
        for (int i = 0; i < successCheckbox.Length; i++)
        {
            successCheckbox[i].SetActive(false); // 비활성화
        }
    }

    // 페이즈가 변경될 때 호출
    public void AssignRandomMinigame()
    {
        indexDic = new Dictionary<int, int>();
        // minigameNameText 배열의 모든 요소를 빈 문자열로 초기화
        for (int i = 0; i < minigameNameText.Length; i++)
        {
            minigameNameText[i].text = ""; // 빈 문자열로 초기화
        }

        // successCheckbox 배열의 모든 체크박스를 비활성화
        for (int i = 0; i < successCheckbox.Length; i++)
        {
            successCheckbox[i].SetActive(false); // 비활성화
        }
        currentMinigameIndex = 0;

        Debug.Log("AssignRandomMinigame 함수 실행");
        // 모든 미니게임을 비활성화
        // foreach (GameObject minigame in minigames)
        // {
        //     minigame.SetActive(false);
        // }

        // 랜덤으로 미니게임 선택 (현재 한 개만 선택)
        int randomIndex = Random.Range(0, minigames.Length);
        // 현재 미니게임 인덱스 설정
        indexDic.Add(randomIndex, currentMinigameIndex);
        // UI에 미니게임 이름 설정
        minigameNameText[currentMinigameIndex].text = minigamesNameList[randomIndex];

        currentMinigameIndex ++;

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

    public void SuccessMission(int index){
        successCheckbox[indexDic[index]].SetActive(true);
    }
}
