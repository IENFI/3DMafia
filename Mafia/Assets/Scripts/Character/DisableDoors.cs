using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableDoors : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        foreach (GameObject door in doors)
        {
            Collider collider = door.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
    }
}