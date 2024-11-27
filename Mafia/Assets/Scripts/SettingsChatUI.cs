using UnityEngine;
using TMPro;
using System.Collections;

public class SettingsChatUI : MonoBehaviour
{
    public PlayerController playerController; // ChatManager에서 사용하긴 하는데 private로 고칠 수 있으면 좋을듯.
    public TMP_InputField chatInput; // 텍스트 입력 필드 참조
    private bool wasFocusedOnDisable = false; // 비활성화 시 isFocused 상태 저장

    private void Awake()
    {
        StartCoroutine(FindPlayerController());
    }

    private IEnumerator FindPlayerController()
    {
        while (playerController == null)
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

            if (playerController == null)
            {
                Debug.LogWarning("PlayerController를 찾는 중입니다...");
                yield return new WaitForSeconds(0.1f); // 0.1초 간격으로 재시도
            }
        }
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.IsAnyUIOpen() && GameManager.instance.CheckRoomPanel()){

            // 텍스트 입력 필드가 포커스를 받고 있는지 (커서 활성화 상태인지) 확인
            if (!(playerController == null) && chatInput.isFocused)
            {
                Debug.Log("텍스트 입력 필드의 커서가 활성화된 상태입니다.");
                if (GameManager.instance.CheckUiList() == 0){
                    Debug.Log("CheckUiList True");
                    playerController.EnableControl(true);
                }
                else{
                    Debug.Log("CheckUiList false");
                    playerController.EnableControl(false);
                }
            }
            else if (!(playerController == null))
            {
                Debug.Log("텍스트 입력 필드의 커서가 비활성화된 상태입니다.");
                playerController.EnableControl(true);
            }

            // 다른 창을 키려고 할 때 채팅창 끄기... 킬까?
            Debug.Log("CheckUiList : " + GameManager.instance.CheckUiList());
            if (GameManager.instance.CheckUiList() > 1){
                
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