using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraController : MonoBehaviourPun
{
    public Camera FPCamera;
    public Camera TPCamera;
    bool cam_check = false;
    public float width;
    public float height;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            FPCamera.enabled = true;
            TPCamera.enabled = true;
            ShowFPCamera();
        }
        else
        {
            FPCamera.enabled = false;
            TPCamera.enabled = false;
        }
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

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

    public void ShowFPCamera()
    {
        FPCamera.rect = new Rect(0, 0, 1, 1);
        TPCamera.rect = new Rect(width, width, height, height);
        FPCamera.depth = -1;
        TPCamera.depth = 1;
    }

    public void ShowTPCamera()
    {
        FPCamera.rect = new Rect(width, width, height, height);
        TPCamera.rect = new Rect(0, 0, 1, 1);
        FPCamera.depth = 1;
        TPCamera.depth = -1;
    }
}
