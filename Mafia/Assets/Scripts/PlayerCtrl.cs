using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Photon 관련 네임스페이스 추가
using TMPro;

public class PlayerCtrl : MonoBehaviourPunCallbacks // Photon 관련 클래스를 상속
{
    private new Rigidbody rigidbody;
    private float v;
    private float h;

    [Header("이동 및 회전 속도")]
    public float moveSpeed = 8.0f;
    public float jumpPower = 5.0f;

    RaycastHit hit;

    IEnumerator Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        yield return null; // 0.5초 기다릴 필요가 없으므로 null을 반환하여 즉시 실행
    }

    void Update()
    {
        // PhotonView가 로컬 플레이어의 것인지 확인
        if (photonView.IsMine)
        {
            v = Input.GetAxis("Vertical");
            h = Input.GetAxis("Horizontal");

            // Debug.Log를 통해 입력값을 확인
            Debug.Log($"v: {v}, h: {h}");

            Debug.DrawRay(transform.position, -transform.up * 0.6f, Color.green);
            if (Input.GetKeyDown("space"))
            {
                if (Physics.Raycast(transform.position, -transform.up, out hit, 0.6f))
                {
                    rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                }
            }



        }

    }

    private void FixedUpdate()
    {
        // PhotonView가 로컬 플레이어의 것인지 확인
        if (photonView.IsMine)
        {
            Vector3 dir = (Vector3.forward * v) + (Vector3.right * h);
            transform.Translate(dir.normalized * Time.deltaTime * moveSpeed, Space.Self);
        }
    }
}
