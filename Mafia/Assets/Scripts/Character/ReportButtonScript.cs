using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportButtonScript : MonoBehaviour
{
    // 오브젝트의 Renderer 컴포넌트 참조
    private Renderer objectRenderer;
    private Material originalMaterial; // 원래 Material을 저장할 변수
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
            originalMaterial = objectRenderer.material; // 초기 Material을 저장
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
                objectRenderer.material = meterial;
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
                objectRenderer.material = originalMaterial;
                objectLight.enabled = false;
            }
            else
            {
                Debug.LogError("Renderer component not found.");
            }
        }
    }

}
