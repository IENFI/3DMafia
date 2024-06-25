using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    void Start()
    {
        LockCursor();
    }

    void Update()
    {
        // Esc 키를 눌렀을 때 커서 고정을 해제하고 커서를 표시합니다.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        // 마우스 왼쪽 버튼을 클릭했을 때, UI 요소를 클릭한 것이 아니면 커서를 고정하고 숨깁니다.
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIElement())
            {
                LockCursor();
            }
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    bool IsPointerOverUIElement()
    {
        // UI 요소 위에 마우스 포인터가 있는지 확인합니다.
        return EventSystem.current.IsPointerOverGameObject();
    }
}