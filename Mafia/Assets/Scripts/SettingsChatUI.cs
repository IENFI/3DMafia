using UnityEngine;
using TMPro;

public class SettingsChatUI : MonoBehaviour
{
    private PlayerController playerController;
    public TMP_InputField chatInput; // 텍스트 입력 필드 참조

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

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.IsAnyUIOpen() && GameManager.instance.CheckRoomPanel()){

            // 텍스트 입력 필드가 포커스를 받고 있는지 (커서 활성화 상태인지) 확인
            if (!(playerController == null) && !chatInput.isFocused)
            {
                Debug.Log("텍스트 입력 필드의 커서가 활성화된 상태입니다.");
                if (!GameManager.instance.CheckUiList()){
                    playerController.EnableControl(true);
                }
                else{
                    playerController.EnableControl(false);
                }
            }
            else if (!(playerController == null))
            {
                // Debug.Log("텍스트 입력 필드의 커서가 비활성화된 상태입니다.");
                playerController.EnableControl(false);
            }

            // 다른 창을 키려고 할 때 채팅창 끄기... 킬까?
            Debug.Log("CheckUiList : " + GameManager.instance.CheckUiList());
            if (GameManager.instance.CheckUiList()){
                
                gameObject.SetActive(false); 
            }
        }

    }

    private void OnEnable()
    {
        if (!(GameManager.instance == null))
        {
            GameManager.instance.RegisterUIWindow(gameObject);
            GameManager.instance.CheckUIState();
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        if (!(GameManager.instance == null))
        {
            GameManager.instance.UnregisterUIWindow(gameObject);
            GameManager.instance.CheckUIState();
        }
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}