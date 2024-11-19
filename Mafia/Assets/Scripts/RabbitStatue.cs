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
        if (other.CompareTag(targetTag))
        {
            SpawnRabbit();
            RabbitUI.SetActive(true);
            photonView.RPC("DestroyRabbit", RpcTarget.All);
        }
    }

    void SpawnRabbit()
    {
        Debug.Log("rabbit appeared");
        photonView.RPC("SpawnByMaster", RpcTarget.All);
    }

    [PunRPC]
    void DestroyRabbit()
    {
        Collider colliders = this.gameObject.GetComponent<Collider>();
        GetComponent<Collider>().enabled = false;

        this.gameObject.transform.localScale = Vector3.zero;
    }

    [PunRPC]
    void SpawnByMaster()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 spawnPosition = transform.position;
            spawnPosition.y += 1;
            PhotonNetwork.Instantiate("Rabbit", spawnPosition, Quaternion.identity, 0);
        }
    }
}
