using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemController : MonoBehaviour
{
    void Start()
    {
        EventSystem eventSystem = GetComponent<EventSystem>();
        if (eventSystem != null)
        {
            // 키보드 내비게이션 비활성화
            eventSystem.sendNavigationEvents = false;
        }
    }
}