using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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
    private Collider lastDoorCollider;

    PhotonView votingSystemPhotonView;

    private Color originCustomizeObjectColor;
    // Add variables for the UI text
    private GameObject toolTipUI;
    private TextMeshProUGUI stateText;
    private TextMeshProUGUI keyText;
    private bool isInitialized = false; // 모든 오브젝트가 null이 아닐 때 true

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

    private void Start()
    {
        isInitialized = false;
        // 코루틴을 통해 오브젝트를 계속 찾도록 시도
        StartCoroutine(FindUIElements());
    }

    private IEnumerator FindUIElements()
    {
        // 씬이 Level_0 또는 Level_1일 때 UI 요소를 찾는 작업을 반복
        while (!isInitialized)
        {
            if (SceneManager.GetActiveScene().name == "Level_0")
            {
                toolTipUI = GameObject.Find("Canvas/ToolTipUI");
                stateText = GameObject.Find("Canvas/ToolTipUI/StateText")?.GetComponent<TextMeshProUGUI>();
                keyText = GameObject.Find("Canvas/ToolTipUI/Image/KeyText")?.GetComponent<TextMeshProUGUI>();
            }
            else if (SceneManager.GetActiveScene().name == "Level_1")
            {
                toolTipUI = GameObject.Find("GameUiManager/Canvas/ToolTipUI");
                stateText = GameObject.Find("GameUiManager/Canvas/ToolTipUI/StateText")?.GetComponent<TextMeshProUGUI>();
                keyText = GameObject.Find("GameUiManager/Canvas/ToolTipUI/Image/KeyText")?.GetComponent<TextMeshProUGUI>();
            }

            // 모든 오브젝트가 null이 아닌지 확인
            if (toolTipUI != null && stateText != null && keyText != null)
            {
                toolTipUI.SetActive(false); // toolTipUI를 비활성화
                isInitialized = true; // 초기화 완료 표시
                Debug.Log("All UI elements found and initialized.");
            }
            else
            {
                // UI 요소를 찾을 수 없을 경우 계속 시도
                Debug.Log("UI elements not found, retrying...");
            }

            yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 다시 시도
        }
    }

    void Update()
    {

        // 모든 오브젝트가 null이 아닐 때만 Update 동작
        if (!isInitialized)
        {
            return; // 초기화가 완료되지 않으면 Update 중단
        }

        if (!photonView.IsMine)
        {
            return;
        }

        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * interactDistance, Color.blue, 0.3f);

        // 문 여는 코드
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactDistance, layerMask))
        {
            if (hit.collider.CompareTag("Door"))
            {
                PhotonView doorPhotonView = hit.collider.transform.GetComponent<PhotonView>();

                // Activate toolTipUI
                toolTipUI.SetActive(true);

                // Update StateText and KeyText when interacting with Door
                stateText.text = "문 열기";
                keyText.text = "E";

                if (doorPhotonView != null && Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Open the Door");
                    doorPhotonView.RPC("ChangeDoorState", RpcTarget.All);
                }
                else
                {
                    //Debug.LogError("The Door object does not have a PhotonView component.");
                }

                // 현재 충돌체를 lastDoorCollider로 저장
                lastDoorCollider = hit.collider;
            }
            else
            {
                if (lastDoorCollider != null)
                {
                    toolTipUI.SetActive(false);
                    lastDoorCollider = null;
                }
            }
        }
        else
        {
            if (lastDoorCollider != null)
            {
                toolTipUI.SetActive(false);
                lastDoorCollider = null;
            }
        }

        if (fpCamera != null)
        {
            Debug.DrawRay(fpCamera.transform.position, fpCamera.transform.forward * minigameInteractDistance, Color.red, 0.3f);
            Debug.DrawRay(fpCamera.transform.position, fpCamera.transform.forward * merchantInteractDistance, Color.yellow, 0.3f);
        }

        if (fpCamera != null)
        {
            if (Physics.Raycast(fpCamera.transform.position, fpCamera.transform.forward, out hit, minigameInteractDistance, layerMask))
            {
                // 미니게임 코드
                if (hit.collider.CompareTag("Minigame"))
                {
                    //Debug.Log("Camera is looking at the minigame.");// 다른 작업 수행
                    characterController = GetComponent<CharacterController>();

                    // 선택된 미니게임의 MinigameBase 클래스 참조 (Fix Wiring은 최상위 UI에 스크립트가 없어서 다시 선언해야 할 듯..?)
                    MinigameBase minigameScript = hit.collider.GetComponent<MinigameInteraction>().TaskUI.GetComponent<MinigameBase>();
                    if (minigameScript.GetActive())
                    {
                        // true가 light
                        hit.collider.GetComponent<MinigameInteraction>().ChangeAllChildMaterials(hit.collider.transform, true);
                        if (!hit.collider.GetComponent<MinigameInteraction>().TaskUI.activeSelf)
                        {

                            // Activate toolTipUI
                            toolTipUI.SetActive(true);

                            // Update StateText and KeyText when interacting with Minigame
                            stateText.text = "미니게임";
                            keyText.text = "E";
                        }
                    }
                    if (characterController.isGrounded && Input.GetKeyDown(KeyCode.E) && minigameScript.GetActive())
                    {
                        hit.collider.GetComponent<MinigameInteraction>().TaskUI.SetActive(true);
                        toolTipUI.SetActive(false);
                    }
                    if (hit.collider.GetComponent<MinigameInteraction>().TaskUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
                    {
                        hit.collider.GetComponent<MinigameInteraction>().TaskUI.SetActive(false);
                        toolTipUI.SetActive(false);
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

                        if (toolTipUI != null)
                            toolTipUI.SetActive(false);
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

                    if (toolTipUI != null)
                        toolTipUI.SetActive(false);

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

                    if (!hit.collider.GetComponent<ShopInteraction>().ShopUI.activeSelf)
                    {
                        // Activate toolTipUI
                        toolTipUI.SetActive(true);

                        // Update StateText and KeyText when interacting with Minigame
                        stateText.text = "상점";
                        keyText.text = "E";
                    }
                    if (characterController.isGrounded && Input.GetKeyDown(KeyCode.E))
                    {
                        hit.collider.GetComponent<ShopInteraction>().ShopUI.SetActive(true);
                        toolTipUI.SetActive(false);
                    }
                    if (hit.collider.GetComponent<ShopInteraction>().ShopUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
                    {
                        hit.collider.GetComponent<ShopInteraction>().ShopUI.SetActive(false);
                        toolTipUI.SetActive(false);
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
                        toolTipUI.SetActive(false);

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
                    toolTipUI.SetActive(false);
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

                    if (reportChance == 1)
                    {
                        hit.collider.GetComponent<ReportButtonScript>().ChangeOutlineRenderer(true);
                        // Activate toolTipUI
                        toolTipUI.SetActive(true);

                        // Update StateText and KeyText when interacting with Minigame
                        stateText.text = "긴급 신고";
                        keyText.text = "R";
                    }

                    if (reportChance != 1)
                    {
                        hit.collider.GetComponent<ReportButtonScript>().ChangeOutlineRenderer(false);
                    }
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
                                toolTipUI.SetActive(false);
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
                        toolTipUI.SetActive(false);
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

                    if (!hit.collider.GetComponent<CustomizeScript>().CustomizingUI.activeSelf)
                    {
                        // Activate toolTipUI
                        toolTipUI.SetActive(true);

                        // Update StateText and KeyText when interacting with Minigame
                        stateText.text = "커스터마이징";
                        keyText.text = "E";
                    }

                    if (characterController.isGrounded && Input.GetKeyDown(KeyCode.E))
                    {
                        toolTipUI.SetActive(false);
                        hit.collider.GetComponent<CustomizeScript>().CustomizingUI.SetActive(true);
                    }
                    if (hit.collider.GetComponent<CustomizeScript>().CustomizingUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
                    {
                        toolTipUI.SetActive(false);
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
                        toolTipUI.SetActive(false);

                        lastCustomizeCollider = null;
                    }
                }
            }
            else
            {
                // 레이캐스트가 아무것도 맞추지 않았을 때도 이전에 Merchant와 상호작용하고 있었다면 상태 초기화
                if (lastReportCollider != null)
                {
                    toolTipUI.SetActive(false);
                    ReportButtonScript lastReportButtonScript = lastReportCollider.GetComponent<ReportButtonScript>();
                    lastReportButtonScript.ChangeOutlineRenderer(false);

                    // 충돌체 상태 초기화
                    lastReportCollider = null;
                }



                if (lastCustomizeCollider != null)
                {
                    toolTipUI.SetActive(false);
                    lastCustomizeCollider.transform.GetComponent<Renderer>().material.color = originCustomizeObjectColor;

                    if (lastCustomizeCollider.GetComponent<CustomizeScript>().CustomizingUI.activeSelf)
                        lastCustomizeCollider.GetComponent<CustomizeScript>().CustomizingUI.SetActive(false);

                    lastCustomizeCollider = null;
                }


            }

        }

    }
}