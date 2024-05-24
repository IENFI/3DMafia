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
    private float r;

    [Header("이동 및 회전 속도")]
    public float moveSpeed = 8.0f;
    public float turnSpeed = 0.0f;
    public float jumpPower = 5.0f;

    private float turnSpeedValue = 200.0f;

    RaycastHit hit;
    IEnumerator Start()
    {
        rigidbody = GetComponent<Rigidbody>();

        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.5f);
        turnSpeed = turnSpeedValue;
    }

    void Update()
    {
        // PhotonView가 로컬 플레이어의 것인지 확인
        if (photonView.IsMine)
        {
            v = Input.GetAxis("Vertical");
            h = Input.GetAxis("Horizontal");

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
            transform.Rotate(Vector3.up * Time.smoothDeltaTime * turnSpeed * r);
        }
    }
}
