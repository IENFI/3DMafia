using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera FPCamera;
    public Camera TPCamera;
    bool cam_check = false;
    public float width;
    public float height;

    private void Awake()
    {
        FPCamera.enabled = true;
        TPCamera.enabled = true;
        ShowFPCamera();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && cam_check == false)
        {
            ShowTPCamera();
            cam_check = true;
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && cam_check == true)
        {
            ShowFPCamera();
            cam_check = false;
        }
    }

    void ShowFPCamera()
    {
        FPCamera.rect = new Rect(0, 0, 1, 1);
        TPCamera.rect = new Rect(width, width, height, height);
        FPCamera.depth = -1;
        TPCamera.depth = 1;
    }

    void ShowTPCamera()
    {
        FPCamera.rect = new Rect(width, width, height, height);
        TPCamera.rect = new Rect(0, 0, 1, 1);
        FPCamera.depth = 1;
        TPCamera.depth = -1;
    }
}
