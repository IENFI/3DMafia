using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class MinigameInteraction : MonoBehaviourPun
{
    public GameObject TaskUI; // 미션 UI

    private bool isPlayerInRange = false; // 플레이어가 주변에 있는지 여부
    private PlayerController playerController;
    private PlayerCoinController player; // 플레이어의 재화 관리 스크립트

    public Material originMaterial;
    public Material lightConeMaterial;

    public bool ExitCode=false;

    bool check = false;

    void Start()
    {
        TaskUI.SetActive(false);
    }

    private void ChangeAllChildMaterials(Transform parent, Material material)
    {
        foreach (Transform child in parent)
        {
            Renderer childRenderer = child.GetComponent<Renderer>();
            if (childRenderer != null)
            {
                childRenderer.material = material;
            }

            // 하위 오브젝트에도 같은 작업을 수행합니다
            ChangeAllChildMaterials(child, material);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + " entered the Collider.");
        if (other.CompareTag("Player"))
        {
            PhotonView otherPhotonView = other.GetComponent<PhotonView>();
            if (otherPhotonView != null)
            {
                if (otherPhotonView.IsMine)
                {
                    player = other.GetComponent<PlayerCoinController>();
                    isPlayerInRange = true;
                    playerController = other.GetComponent<PlayerController>();
                    ChangeAllChildMaterials(transform, lightConeMaterial);
                }
            }
        }
    }
        

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.name + " exited the Collider.");
        // 이곳에 필요한 로직을 추가합니다
        if (other.CompareTag("Player"))
        {
            PhotonView otherPhotonView = other.GetComponent<PhotonView>();
            if (otherPhotonView != null)
            {
                if (otherPhotonView.IsMine)
                {
                    isPlayerInRange = false;
                    player = null;
                    ChangeAllChildMaterials(transform, originMaterial);
                    playerController = null;
                    TaskUI.SetActive(false); // 플레이어가 범위를 벗어나면 ShopUI를 비활성화
                    check = false;
                }
            }
            
        }
    }


    private void Update()
    {
        if (ExitCode == true)
        {
            player.GetCoin(60);
            ExitCode = false;
        }
        // 플레이어가 미니게임 주변에 있고 E 키를 누르면 상점 UI 활성화
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && check == false)
        {
            TaskUI.SetActive(true);

            check = true;
        }
        else if (isPlayerInRange && Input.GetKeyDown(KeyCode.F) && check == true)
        {
            TaskUI.SetActive(false);
            check = false;
        }
    }

}
