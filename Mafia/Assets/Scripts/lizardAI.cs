using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class lizardAI : MonoBehaviour
{
    public string targetTag = "Player";
    public float detectionRadius = 15f;
    public float moveSpeed = 3f;

    //private bool isChasing = false;
    private CharacterController characterController;
    public Animator animator;

    public Vector3 vector = new Vector3(0f, 0f, 0f);
    private float gravity = -9.81f;

    public float initialMomentum = 50;
    public float triggerMomentum = 10;
    private float momentum;

    public float idleDuration = 2.0f;
    public float walkDuration = 10.0f;

    private float stateTimer;
    private bool isWalking;

    public GameObject[] trees;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        momentum = initialMomentum;
        stateTimer = idleDuration;
        isWalking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        GameObject target = DetectPlayer();
        if (target != null && !IsMafia(target))
        {
            animator.SetBool("startRun", true);
            Chase(target);
        }
        else
        {
            animator.SetBool("startRun", false);
            stateTimer -= Time.deltaTime;

            if (stateTimer <= 0)
            {
                // 상태 전환
                if (isWalking)
                {
                    // walk -> idle
                    animator.SetBool("startWalk", false);
                    isWalking = false;
                    stateTimer = idleDuration;
                }
                else
                {
                    // idle -> walk
                    animator.SetBool("startWalk", true);
                    isWalking = true;
                    stateTimer = walkDuration;
                }
            }

            if (isWalking)
            {
                walk();
            }

            else
            {
                // idle
                vector = new Vector3(0f, 0f, 0f);
            }
        }
    }

    void walk()
    {
        float randomX = Random.Range(-5f, 5f);
        float randomZ = Random.Range(-5f, 5f);
        Vector3 move_vector = new Vector3(randomX, 0f, randomZ);
        vector = momentum * vector + move_vector;
        vector = vector.normalized * 1 / 10 * moveSpeed;
        if (characterController.isGrounded == false)
        {
            vector.y += gravity * Time.deltaTime;
        }

        else
        {
            vector.y = 0f;
        }

        characterController.Move(vector);

        vector = vector.normalized;
    }

    void Chase(GameObject target)
    {
        Vector3 direction = target.transform.position - transform.position;
        direction.y = 0; // Ensure movement is only horizontal

        vector = direction;

        vector = vector.normalized * 1 / 60 * moveSpeed;

        characterController.Move(vector);
    }

    GameObject DetectPlayer()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject target in targets)
        {
            // Calculate the distance between rabbit and the target
            float distance = Vector3.Distance(transform.position, target.transform.position);

            // Check if the distance is less than detectionRadius
            if (distance < detectionRadius)
            {
                return target;
            }
        }
        return null;
    }

    void OnTriggerEnter(Collider other)
    {
        momentum = triggerMomentum;
        if (other.CompareTag(targetTag))
        {
            if (!IsMafia(other.gameObject))
            {
                Attack(other.gameObject);
                animator.SetBool("attack", true);
                Invoke("RandomSpawn", 2f);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        momentum = initialMomentum;
    }

    bool IsMafia(GameObject obj)
    {
        int index = -1;
        for (int i=0; i<PhotonNetwork.PlayerList.Length;  i++)
        {
            if (PhotonNetwork.PlayerList[i].TagObject == obj)
            {
                index = i;
                break;
            }
        }
        if (index == -1) return false;
        if ((bool)PhotonNetwork.PlayerList[index].CustomProperties["isMafia"])
        {
            return true;
        }
        else return false;
    }

    void Attack(GameObject obj)
    {
        PlayerController playerCont = obj.GetComponent<PlayerController>();
        playerCont.EnableControl(false);
        StartCoroutine(WaitAndRelease(obj));
    }

    IEnumerator WaitAndRelease(GameObject obj)
    {
        yield return new WaitForSeconds(20f);
        PlayerController playerCont = obj.GetComponent<PlayerController>();
        playerCont.EnableControl(true);
    }

    void RandomSpawn()
    {
        animator.SetBool("attack", false);
        trees = GameObject.FindGameObjectsWithTag("Spawn");

        Debug.Log("Spawn");
        // 하위 오브젝트의 수를 얻음
        int childCount = trees[0].transform.childCount;

        // 랜덤 정수 추출
        int randomIndex = Random.Range(0, childCount);

        // 랜덤 정수번째 하위 오브젝트의 트랜스폼을 얻음
        Transform randomChild = trees[0].transform.GetChild(randomIndex);
        // 플레이어를 하위 오브젝트의 위치로 이동
        this.gameObject.transform.position = randomChild.position;
    }
}
