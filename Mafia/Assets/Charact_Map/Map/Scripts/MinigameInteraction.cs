using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class MinigameInteraction : MonoBehaviourPunCallbacks
{
    public GameObject TaskUI; // 미션 UI

    private PlayerCoinController player; // 플레이어의 재화 관리 스크립트

    public Material originMaterial;
    public Material lightConeMaterial;

    public bool ExitCode = false;

    public Transform someTargetObjectTransform;

    void Start()
    {
        TaskUI.SetActive(false);

        StartCoroutine(FindLocalPlayerCoinController());
    }

    private IEnumerator FindLocalPlayerCoinController()
    {
        while (player == null)
        {
            foreach (var pcc in FindObjectsOfType<PlayerCoinController>())
            {
                if (pcc.photonView.IsMine)
                {
                    player = pcc;
                    break;
                }
            }

            if (player == null)
            {
                //Debug.Log("(MinigameInteraction) PlayerCoinController를 찾는 중...");
            }

            // 다음 프레임까지 대기
            yield return null;
        }

        //Debug.Log("(MinigameInteraction) PlayerCoinController를 찾았습니다.");
    }

    public void ChangeAllChildMaterials(Transform parent, bool change)
    {
        Material material = change ? lightConeMaterial : originMaterial;
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


    private void Update()
    {
        if (ExitCode)
        {
            if (player != null)
            {
                player.GetCoin(60);
            }
            else
            {
                //Debug.LogError("PlayerCoinController가 할당되지 않았습니다.");
            }
            ExitCode = false;
        }
    }

}