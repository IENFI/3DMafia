using Photon.Pun;
using UnityEngine;

public class ShopInteraction : MonoBehaviourPunCallbacks
{
    [SerializeField] private Renderer outlineRenderer;
    public GameObject ShopUI;
    public ShopManager shopManager;

    public void SetShopUIActive(bool isActive, bool isLocalPlayer)
    {
        if (isLocalPlayer)
        {
            ShopUI.SetActive(isActive);
            if (isActive)
            {
                shopManager.OpenShopUI();
            }
            else
            {
                shopManager.CloseShopUI();
            }
        }
    }

    public void ChangeOutlineRenderer(Color color)
    {
        outlineRenderer.material.color = color;
    }
}