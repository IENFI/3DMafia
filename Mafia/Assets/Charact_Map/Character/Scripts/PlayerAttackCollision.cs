using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviour
{
    private List<Collider> collidersInRange = new List<Collider>(); // 범위 내의 collider를 저장할 리스트
    private float lastKillTime; // 킬한 시간 저장
    public float killCooldown = 5.0f; // 쿨타임 설정 (5초)


    private void OnTriggerEnter(Collider other)
    {
        PhotonView otherPhotonView = other.GetComponent<PhotonView>();

        // 다른 플레이어의 PhotonView이며 아직 처리되지 않은 collider일 때 리스트에 추가
        if (otherPhotonView != null && !otherPhotonView.IsMine && other.CompareTag("Player"))
        {
            collidersInRange.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PhotonView otherPhotonView = other.GetComponent<PhotonView>();

        // 리스트에서 collider 제거
        if (otherPhotonView != null && !otherPhotonView.IsMine && other.CompareTag("Player"))
        {
            collidersInRange.Remove(other);
        }
    }

    public void SeletKillMember()
    {
        StartCoroutine(ExecuteAfterDelay());
    }

    private IEnumerator ExecuteAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);

        // 가장 가까운 collider를 찾기 위한 변수 초기화
        Collider nearestCollider = null;
        float minDistance = float.MaxValue;

        // 범위 내의 모든 collider를 검사하여 가장 가까운 collider 찾기
        foreach (Collider col in collidersInRange)
        {
            float distance = Vector3.Distance(transform.position, col.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCollider = col;
            }
        }

        // 가장 가까운 collider가 있는 경우 해당 collider의 Death 함수 호출
        if (nearestCollider != null && Time.time - lastKillTime >= killCooldown)
        {

            PhotonView otherPhotonView = nearestCollider.GetComponent<PhotonView>();
            if (otherPhotonView != null)
            {
                otherPhotonView.RPC("Death", RpcTarget.All);
                lastKillTime = Time.time;
            }
        }

        collidersInRange.Clear();
    }
}