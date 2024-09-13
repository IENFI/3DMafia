using UnityEngine;

public class ViewRadiusController : MonoBehaviour
{
    public Material circularViewMaterial;  // Shader가 적용된 Material
    public Transform player;  // 플레이어의 Transform

    void Update()
    {
        // 플레이어의 위치를 가져와서 시야 중심으로 설정
        Vector3 playerPos = player.position;
        circularViewMaterial.SetVector("_Center", new Vector4(playerPos.x, playerPos.y, 0, 0));
    }
}