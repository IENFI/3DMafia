using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine("AutoDisable");
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotonView otherPhotonView = other.GetComponent<PhotonView>();
        if (otherPhotonView != null && !otherPhotonView.IsMine && other.CompareTag("Player"))
        {
            // 다른 플레이어의 PhotonView이면 RPC를 호출하여 Death 함수 실행
            otherPhotonView.RPC("Death", RpcTarget.All);
        }
    }

    private IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(0.1f);

        gameObject.SetActive(false);
    }
}
