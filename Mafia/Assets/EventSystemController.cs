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
            // Ű���� ������̼� ��Ȱ��ȭ
            eventSystem.sendNavigationEvents = false;
        }
    }
}