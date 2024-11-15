using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
  
public class LightOnOff : MonoBehaviourPun
{
    [SerializeField]
    public GameObject flashlight;

    void Update()
    {
        // UIOpen
        if (GameManager.instance.IsAnyUIOpen())
            return;

        if (photonView.IsMine && Input.GetKeyDown(KeyCode.Alpha1))
        {
            Light lightComponent = flashlight.GetComponent<Light>();

            bool isFlashlightOn = lightComponent.enabled;
            isFlashlightOn = !isFlashlightOn;

            lightComponent.enabled = isFlashlightOn;

            photonView.RPC("SyncFlashlightState", RpcTarget.All, isFlashlightOn);
        }
    }

    [PunRPC]
    void SyncFlashlightState(bool state)
    {
        Light lightComponent0 = flashlight.GetComponent<Light>();
        lightComponent0.enabled = state;
    }
}
