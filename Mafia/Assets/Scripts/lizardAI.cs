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
        if (target != null)
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

        vector = vector.normalized * 1 / 6 * moveSpeed;

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
    }

    void OnTriggerExit(Collider other)
    {
        momentum = initialMomentum;
    }

}
