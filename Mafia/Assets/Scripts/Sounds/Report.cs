using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Report : MonoBehaviour
{
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))  // R 키가 눌렸을 때
        {
            audioSource.Play();
        }
    }

 
}
