using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class GhostInteractScript : MonoBehaviour
{

    public float interactDiastance = 10f;
    [SerializeField]
    private GameObject toolTipUI;
    [SerializeField]
    private TextMeshProUGUI stateText;
    [SerializeField]
    private TextMeshProUGUI keyText;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactDiastance))
        {
            if (hit.collider.CompareTag("Door"))
            {
                // Activate toolTipUI
                toolTipUI.SetActive(true);
                // Update StateText and KeyText when interacting with Door
                stateText.text = "문 열기";
                keyText.text = "E";
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("KeyDown E");
                    hit.collider.transform.GetComponent<DoorScript>().GhostChangeDoorState();
                }
            }
        }

    }
}
