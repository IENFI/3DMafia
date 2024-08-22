using UnityEngine;

public class SettingsUI : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.instance.RegisterUIWindow(gameObject);
        GameManager.instance.CheckUIState();
    }

    private void OnDisable()
    {
        GameManager.instance.UnregisterUIWindow(gameObject);
        GameManager.instance.CheckUIState();
    }
}