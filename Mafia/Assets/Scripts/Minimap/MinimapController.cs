using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public Transform player1; // Inspector에서 할당할 변수
    public Transform player2; // 복사한 맵의 플레이어 위치를 동기화할 변수

    void Start()
    {
        // 원본 맵에서 플레이어 객체의 Transform 컴포넌트 가져오기
        player1 = GameObject.FindGameObjectWithTag("Player").transform;

        // 복사한 맵에서 player2 GameObject의 Transform 컴포넌트 가져오기
        player2 = GameObject.FindGameObjectWithTag("Player2").transform;
    }

    void Update()
    {
        // 원본 맵의 플레이어 위치를 복사한 맵의 player2 위치에 동기화
        if (player1 != null && player2 != null)
        {
            player2.position = player1.position;
        }
    }
}
