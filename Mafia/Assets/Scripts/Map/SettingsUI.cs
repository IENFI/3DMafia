using UnityEngine;

public class SettingsUI : MonoBehaviour
{

    private PlayerController playerController;


    private void Awake()
    {
        foreach (var pcc in FindObjectsOfType<PlayerController>())
        {
            if (pcc.photonView.IsMine)
            {
                playerController = pcc;
                Debug.Log("SettingsUI에서 playerController를 찾았습니다.");
                break;
            }
        }
    }

    private void OnEnable()
    {
        //Debug.Log("Setting UI Enable");
        if (!(GameManager.instance == null))
        {
            GameManager.instance.RegisterUIWindow(gameObject);
            GameManager.instance.CheckUIState();
        }
        if (!(playerController == null))
            playerController.EnableControl(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        //Debug.Log("Setting UI Disable");
        if (!(GameManager.instance == null))
        {
            GameManager.instance.UnregisterUIWindow(gameObject);
            GameManager.instance.CheckUIState();
        }
        if (!(playerController == null))
            playerController.EnableControl(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}