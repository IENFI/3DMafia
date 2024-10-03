using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportButtonScript : MonoBehaviour
{
    // 오브젝트의 Renderer 컴포넌트 참조
    private Renderer objectRenderer;
    [SerializeField]
    private Material meterial;
    private Light objectLight;

    // Start is called before the first frame update
    void Start()
    {
        // Renderer 컴포넌트 가져오기
        objectRenderer = GetComponent<Renderer>();

        // 색상을 하얀색으로 변경
        if (objectRenderer != null)
        {
            objectRenderer.material.color = Color.white;
        }
        else
        {
            Debug.LogError("Renderer component not found.");
        }

        // Light 컴포넌트 가져오기
        objectLight = GetComponent<Light>();

        // 예외 처리: Light 컴포넌트가 없는 경우
        if (objectLight == null)
        {
            Debug.LogError("Light component not found.");
        }
        objectLight.enabled = false;
    }

    public void ChangeOutlineRenderer(bool change)
    {
        if (change)
        {
            // 색상을 빨간색으로 변경
            if (objectRenderer != null)
            {
                Debug.Log("Turn red");
                objectRenderer.material.color = Color.red;
                objectLight.enabled = true;
            }
            else
            {
                Debug.LogError("Renderer component not found.");
            }
        }
        else
        {
            // 색상을 하얀색으로 변경
            if (objectRenderer != null)
            {
                Debug.Log("Turn white");
                objectRenderer.material.color = Color.white;
                objectLight.enabled = false;
            }
            else
            {
                Debug.LogError("Renderer component not found.");
            }
        }
    }

}
