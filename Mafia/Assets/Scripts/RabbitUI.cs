using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitUI : MonoBehaviour
{
    private GameObject ui;

    // Start is called before the first frame update
    void Start()
    {
        ui = this.gameObject;
        Invoke("HideUI", 3f);
    }

    void HideUI()
    {
        ui.SetActive(false);
    }
}
