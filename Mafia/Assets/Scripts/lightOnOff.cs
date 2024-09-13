using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class lightOnOff : MonoBehaviourPun
{
    [SerializeField]
    public GameObject flashlight;

    void Update()
    {
        if (photonView.IsMine && Input.GetKeyDown(KeyCode.L))
        {
            bool isFlashlightOn = flashlight.activeSelf;
            isFlashlightOn = !isFlashlightOn;

            flashlight.SetActive(isFlashlightOn);

            photonView.RPC("SyncFlashlightState", RpcTarget.All, isFlashlightOn);
        }
    }

    [PunRPC]
    void SyncFlashlightState(bool state)
    {
        flashlight.SetActive(state);
    }
}
