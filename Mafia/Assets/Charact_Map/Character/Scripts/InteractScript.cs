using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class InteractScript : MonoBehaviour
{

    public float interactDiastance = 10f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("KeyDown E");
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, interactDiastance))
            {
                if (hit.collider.CompareTag("Door"))
                {
                    PhotonView doorPhotonView = hit.collider.transform.GetComponent<PhotonView>();
                    if (doorPhotonView != null)
                    {
                        doorPhotonView.RPC("ChangeDoorState", RpcTarget.All);
                    }
                    else
                    {
                        Debug.LogError("The Door object does not have a PhotonView component.");
                    }
                }
            }
        }

    }
}