using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RabbitStatue : MonoBehaviour
{
    public string targetTag = "Player";

    public GameObject RabbitUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            SpawnRabbit();
            RabbitUI.SetActive(true);
            Destroy(gameObject);
        }
    }

    void SpawnRabbit()
    {
        Debug.Log("rabbit appeared");
        Vector3 spawnPosition = transform.position;
        spawnPosition.y += 1;
        PhotonNetwork.Instantiate("Rabbit", spawnPosition, Quaternion.identity, 0);
    }
}