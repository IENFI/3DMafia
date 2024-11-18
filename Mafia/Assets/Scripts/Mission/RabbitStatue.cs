using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RabbitStatue : MonoBehaviourPun
{
    public string targetTag = "Player";

    public GameObject RabbitUI;

    private void OnTriggerEnter(Collider other)
    {
        // 두 명이 동시에 부딪히면 혹시 오류 안 생기나?
        if (other.CompareTag(targetTag))
        {
            SpawnRabbit();
            RabbitUI.SetActive(true);
            other.GetComponent<PhotonView>().RPC("DestroyRabbit", RpcTarget.All);
        }
    }

    void SpawnRabbit()
    {
        Debug.Log("rabbit appeared");
        Vector3 spawnPosition = transform.position;
        spawnPosition.y += 1;
        PhotonNetwork.Instantiate("Rabbit", spawnPosition, Quaternion.identity, 0);
    }

    [PunRPC]
    void DestroyRabbit()
    {
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}