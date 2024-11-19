using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    public AudioClip doorSound;
    AudioSource audioSource;
    public bool open = false;
    public bool ghostOpen = false;
    public float doorOpenAngle = 90f;
    public float doorCloseAngle = 0f;
    public float smooth = 2f;

    private PhotonView photonView;
    private Coroutine currentCoroutine; // 현재 실행 중인 코루틴을 저장할 변수

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();
    }


    [PunRPC]
    public void ChangeDoorState()
    {
        open = !open;
        UpdateDoorState(false);
        audioSource.PlayOneShot(doorSound); // 소리 재생
    }
    public void OpenDoor()
    {
        // 네트워크의 모든 클라이언트에서 소리 재생
        photonView.RPC("ChangeDoorState", RpcTarget.All);

        // 문 열기 로직 추가 (예: 애니메이션 트리거)
        Debug.Log("문이 열렸습니다!");
    }
    public void GhostChangeDoorState()
    {
        ghostOpen = !ghostOpen;
        UpdateDoorState(true);
    }


    // 기본적으로 open이 변하면 open 변수에 맞춰 문이 변화해야함.
    // 근데 open이 변하지 않고 ghostOpen이 변하면 그때도 문이 변함.
    // ghostOpen이 open과 다른데 open에서 변화가 일어나면 ghostOpen과 open 값이 같아지고 open에 따라 문이 변함.
    private void UpdateDoorState(bool isGhostChange)
    {
        if (!isGhostChange)
        {
            if (ghostOpen == open) return; // open 값이 변경될 때 ghostOpen과 동일하면 업데이트하지 않음
            ghostOpen = open; // open 값이 변경될 때 ghostOpen도 동기화
        }

        bool finalState = isGhostChange ? ghostOpen : open;
        Quaternion targetRotation = finalState ? Quaternion.Euler(0, doorOpenAngle, 0) : Quaternion.Euler(0, doorCloseAngle, 0);

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine); // 현재 실행 중인 코루틴이 있으면 중지
        }

        currentCoroutine = StartCoroutine(RotateDoor(targetRotation));
        audioSource.Play();
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.01f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
            yield return null;
        }
        transform.localRotation = targetRotation; // 최종 위치에 정확히 맞춤
    }

    //void Update()
    //{
    //    if (open)
    //    {
    //        Quaternion targetRotation = Quaternion.Euler(0, doorOpenAngle, 0);
    //        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    //    }
    //    else
    //    {
    //        Quaternion targetRotation2 = Quaternion.Euler(0, doorCloseAngle, 0);
    //        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation2, smooth * Time.deltaTime);
    //    }
    //}


}