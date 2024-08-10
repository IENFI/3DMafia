using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostInteractScript : MonoBehaviour
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
                    hit.collider.transform.GetComponent<DoorScript>().GhostChangeDoorState();
                }
            }
        }

    }
}
