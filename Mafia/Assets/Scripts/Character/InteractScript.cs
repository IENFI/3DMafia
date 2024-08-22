using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class InteractScript : MonoBehaviourPun
{
    private float interactDistance = 5f;
    private float minigameInteractDistance = 15f;
    private float merchantInteractDistance = 4f;

    int playerLayer;
    int layerMask;
    int reportChance;

    Camera[] cameras;
    Camera fpCamera;
    private CharacterController characterController;
    private PlayerController playerController;
    private Collider lastMinigameCollider;
    private Collider lastMerchantCollider;
    private Collider lastReportCollider;
    private Collider lastCustomizeCollider;

    PhotonView votingSystemPhotonView;

    private Color originCustomizeObjectColor;

    private void Awake()
    {
        // Player 레이어의 인덱스를 가져옵니다
        playerLayer = LayerMask.NameToLayer("Player");
        // Ghost 레이어의 인덱스를 가져옵니다
        int ghostLayer = LayerMask.NameToLayer("Ghost");
        // 모든 레이어를 포함하는 LayerMask를 생성합니다
        layerMask = Physics.DefaultRaycastLayers & ~(1 << playerLayer) & ~(1 << ghostLayer);

        cameras = GetComponentsInChildren<Camera>();
        fpCamera = null;
        foreach (Camera cam in cameras)
        {
            if (cam.name == "FPCamera") // FPCamera의 이름이 "FPCamera"라고 가정
            {
                fpCamera = cam;
                break;
            }
        }
        reportChance = 1;

    }
    void Update()
    {
        if(!photonView.IsMine)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {

            Debug.Log("KeyDown E");
            RaycastHit hit;
            Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.blue, 0.3f);

            // 문 여는 코드
            if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance, layerMask))
            {
                if (hit.collider.CompareTag("Door"))
                {
                    PhotonView doorPhotonView = hit.collider.transform.GetComponent<PhotonView>();
                    if (doorPhotonView != null)
                    {
                        Debug.Log("Open the Door");
                        doorPhotonView.RPC("ChangeDoorState", RpcTarget.All);
                    }
                    else
                    {
                        //Debug.LogError("The Door object does not have a PhotonView component.");
                    }
                }
            }
        }

        if (fpCamera != null)
        {
            Debug.DrawRay(fpCamera.transform.position, fpCamera.transform.forward * minigameInteractDistance, Color.red, 0.3f);
            Debug.DrawRay(fpCamera.transform.position, fpCamera.transform.forward * merchantInteractDistance, Color.yellow, 0.3f);
        }

        if (fpCamera != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(fpCamera.transform.position, fpCamera.transform.forward, out hit, minigameInteractDistance, layerMask))
            {
                // 미니게임 코드
                if (hit.collider.CompareTag("Minigame"))
                {
                    //Debug.Log("Camera is looking at the minigame.");// 다른 작업 수행
                    characterController = GetComponent<CharacterController>();
                    // true가 light
                    hit.collider.GetComponent<MinigameInteraction>().ChangeAllChildMaterials(hit.collider.transform, true);
                    if (characterController.isGrounded && Input.GetKeyDown(KeyCode.E))
                    {
                        hit.collider.GetComponent<MinigameInteraction>().TaskUI.SetActive(true);
                    }
                    if (hit.collider.GetComponent<MinigameInteraction>().TaskUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
                    {
                        hit.collider.GetComponent<MinigameInteraction>().TaskUI.SetActive(false);
                    }

                    // 현재 충돌체를 lastMinigameCollider로 저장
                    lastMinigameCollider = hit.collider;

                }
                else
                {
                    //Debug.Log("Camera is not looking at the target object.");
                    characterController = null;
                    // 이전에 minigame과 상호작용하고 있었다면, 그 충돌체를 업데이트
                    if (lastMinigameCollider != null)
                    {
                        MinigameInteraction lastMinigameInteraction = lastMinigameCollider.GetComponent<MinigameInteraction>();
                        lastMinigameInteraction.ChangeAllChildMaterials(lastMinigameCollider.transform, false);
                        lastMinigameInteraction.TaskUI.SetActive(false);

                        // 충돌체 상태 초기화
                        lastMinigameCollider = null;
                    }
                }
            }
            else
            {
                // 레이캐스트가 아무것도 맞추지 않았을 때도 이전에 Merchant와 상호작용하고 있었다면 상태 초기화
                if (lastMinigameCollider != null)
                {
                    MinigameInteraction lastMinigameInteraction = lastMinigameCollider.GetComponent<MinigameInteraction>();
                    lastMinigameInteraction.ChangeAllChildMaterials(lastMinigameCollider.transform, false);
                    lastMinigameInteraction.TaskUI.SetActive(false);

                    // 충돌체 상태 초기화
                    lastMinigameCollider = null;
                }
            }

            if (Physics.Raycast(fpCamera.transform.position, fpCamera.transform.forward, out hit, merchantInteractDistance, layerMask))
            {
                // 상점 코드
                if (hit.collider.CompareTag("ShopNPC"))
                {
                    //Debug.Log("Camera is looking at the merchant.");// 다른 작업 수행
                    characterController = GetComponent<CharacterController>();
                    // 모자를 파란색으로 변경
                    hit.collider.GetComponent<ShopInteraction>().ChangeOutlineRenderer(Color.blue);
                    if (characterController.isGrounded && Input.GetKeyDown(KeyCode.E))
                    {
                        hit.collider.GetComponent<ShopInteraction>().ShopUI.SetActive(true);
                    }
                    if (hit.collider.GetComponent<ShopInteraction>().ShopUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
                    {
                        hit.collider.GetComponent<ShopInteraction>().ShopUI.SetActive(false);
                    }

                    // 현재 충돌체를 lastMinigameCollider로 저장
                    lastMerchantCollider = hit.collider;

                }
                else
                {
                    //Debug.Log("Camera is not looking at the target object.");
                    characterController = null;
                    // 이전에 Merchant와 상호작용하고 있었다면, 그 충돌체를 업데이트
                    if (lastMerchantCollider != null)
                    {
                        ShopInteraction lastShopInteraction = lastMerchantCollider.GetComponent<ShopInteraction>();
                        lastShopInteraction.ChangeOutlineRenderer(Color.white);
                        lastShopInteraction.ShopUI.SetActive(false);

                        // 충돌체 상태 초기화
                        lastMerchantCollider = null;
                    }
                }
            }
            else
            {
                // 레이캐스트가 아무것도 맞추지 않았을 때도 이전에 Merchant와 상호작용하고 있었다면 상태 초기화
                if (lastMerchantCollider != null)
                {
                    ShopInteraction lastShopInteraction = lastMerchantCollider.GetComponent<ShopInteraction>();
                    lastShopInteraction.ChangeOutlineRenderer(Color.white);
                    lastShopInteraction.ShopUI.SetActive(false);

                    // 충돌체 상태 초기화
                    lastMerchantCollider = null;
                }
            }

            if (Physics.Raycast(fpCamera.transform.position, fpCamera.transform.forward, out hit, interactDistance, layerMask))
            {
                // 신고 코드
                if (hit.collider.CompareTag("Report"))
                {
                    //Debug.Log("Camera is looking at the report.");// 다른 작업 수행
                    characterController = GetComponent<CharacterController>();
                    playerController = GetComponent<PlayerController>();
                    // 모자를 파란색으로 변경
                    hit.collider.GetComponent<ReportButtonScript>().ChangeOutlineRenderer(true);
                    if (characterController.isGrounded && Input.GetKeyDown(KeyCode.R) && (reportChance == 1))
                    {
                        if (Time.time - playerController.GetLastReportTime() >= playerController.reportCooldown)
                        {
                            playerController.SetLastReportTime(Time.time);
                            votingSystemPhotonView = FindObjectOfType<VotingSystem>().GetComponent<PhotonView>();
                            if (votingSystemPhotonView != null)
                            {
                                votingSystemPhotonView.RPC("StartVote", RpcTarget.All);
                                reportChance = 0;
                            }
                        }
                    }

                    // 현재 충돌체를 lastMinigameCollider로 저장
                    lastReportCollider = hit.collider;

                }
                else
                {
                    //Debug.Log("Camera is not looking at the target object.");
                    characterController = null;
                    playerController = null;
                    // 이전에 buttonlastReportButtonScript 상호작용하고 있었다면, 그 충돌체를 업데이트
                    if (lastReportCollider != null)
                    {
                        ReportButtonScript lastReportButtonScript = lastReportCollider.GetComponent<ReportButtonScript>();
                        lastReportButtonScript.ChangeOutlineRenderer(false);

                        // 충돌체 상태 초기화
                        lastReportCollider = null;
                    }
                }

                

                // 커스터마이징 UI 활성화
                if (hit.collider.CompareTag("Customize"))
                {
                    characterController = GetComponent<CharacterController>();
                    originCustomizeObjectColor = hit.collider.GetComponent<CustomizeScript>().originColor;
                    hit.collider.transform.GetComponent<Renderer>().material.color = Color.black;
                    if (characterController.isGrounded && Input.GetKeyDown(KeyCode.E))
                    {
                        hit.collider.GetComponent<CustomizeScript>().CustomizingUI.SetActive(true);
                    }
                    if (hit.collider.GetComponent<CustomizeScript>().CustomizingUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
                    {
                        Debug.Log("1");

                        hit.collider.GetComponent<CustomizeScript>().CustomizingUI.SetActive(false);
                    }

                    // 현재 충돌체를 lastCustomizeCollider로 저장
                    lastCustomizeCollider = hit.collider;
                }
                else
                {
                    characterController = null;
                    if (lastCustomizeCollider != null)
                    {
                        lastCustomizeCollider.transform.GetComponent<Renderer>().material.color = originCustomizeObjectColor;

                        if (lastCustomizeCollider.GetComponent<CustomizeScript>().CustomizingUI.activeSelf)
                        {
                            Debug.Log("2");
                            lastCustomizeCollider.GetComponent<CustomizeScript>().CustomizingUI.SetActive(false);
                        }

                        lastCustomizeCollider = null;
                    }
                }
            }
            else
            {
                // 레이캐스트가 아무것도 맞추지 않았을 때도 이전에 Merchant와 상호작용하고 있었다면 상태 초기화
                if (lastReportCollider != null)
                {
                    ReportButtonScript lastReportButtonScript = lastReportCollider.GetComponent<ReportButtonScript>();
                    lastReportButtonScript.ChangeOutlineRenderer(false);

                    // 충돌체 상태 초기화
                    lastReportCollider = null;
                }



                if (lastCustomizeCollider != null)
                {
                    lastCustomizeCollider.transform.GetComponent<Renderer>().material.color = originCustomizeObjectColor;

                    if (lastCustomizeCollider.GetComponent<CustomizeScript>().CustomizingUI.activeSelf)
                        lastCustomizeCollider.GetComponent<CustomizeScript>().CustomizingUI.SetActive(false);

                    lastCustomizeCollider = null;
                }


            }

        }

    }
}