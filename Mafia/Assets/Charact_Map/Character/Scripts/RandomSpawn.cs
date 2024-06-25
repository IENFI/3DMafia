using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RandomSpawn : MonoBehaviour
{
    public GameObject[] trees;

    private void Start()
    {
        trees = GameObject.FindGameObjectsWithTag("Spawn");
    }

    [PunRPC]
    void Spawn()
    {
        Debug.Log("Spawn");
        // 하위 오브젝트의 수를 얻음
        int childCount = trees[0].transform.childCount;

        // 랜덤 정수 추출
        int randomIndex = Random.Range(0, childCount);

        // 랜덤 정수번째 하위 오브젝트의 트랜스폼을 얻음
        Transform randomChild = trees[0].transform.GetChild(randomIndex);

        // 플레이어를 하위 오브젝트의 위치로 이동
        this.gameObject.transform.position = randomChild.position;
    }
}