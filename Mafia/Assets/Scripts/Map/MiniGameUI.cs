using UnityEngine;

public class MiniGameUI : MonoBehaviour
{
    private FPCameraController cameraController;

    private void OnEnable()
    {
        if (GameManager.instance.LocalPlayer != null)
        {
            Debug.Log("LocalPlayer found.");

            cameraController = GameManager.instance.LocalPlayer.GetComponentInChildren<FPCameraController>();
            if (cameraController != null)
            {
                cameraController.DisableRotation(); // 카메라 회전 비활성화
                Debug.Log("FPCameraController rotation disabled.");
            }
            else
            {
                Debug.LogError("FPCameraController not found in children of LocalPlayer.");
            }

           
        }
        else
        {
            Debug.LogError("LocalPlayer is null.");
        }
    }

    private void OnDisable()
    {
        if (cameraController != null)
        {
            cameraController.EnableRotation(); // 카메라 회전 활성화
            Debug.Log("FPCameraController rotation enabled.");
        }
        else
        {
            Debug.LogError("FPCameraController was not found when trying to enable.");
        }

    }
}
