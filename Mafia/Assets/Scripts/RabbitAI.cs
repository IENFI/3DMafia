using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RabbitAI : MonoBehaviourPun
{
    public string targetTag = "Player";
    public float detectionRadius = 5f;
    public float moveSpeed = 4f;
    //private float speed;
    //public float waitTime = 0.5f; //wait after move

    private CharacterController characterController;
    private bool isRunningAway = false;
    public Vector3 vector = new Vector3(0f, 0f, 0f);
    public float initialMomentum = 100;
    public float triggerMomentum = 20;
    private float momentum;
    private float gravity = -9.81f;

    private PlayerCoinController player;

    void Start()
    {
        // Initialize the CharacterController
        characterController = GetComponent<CharacterController>();
        //speed = moveSpeed;
        momentum = initialMomentum;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (isRunningAway) return;

            GameObject target = DetectPlayer();
            if (target != null)
            {
                RunAway(target);
            }
            else
            {
                float randomX = Random.Range(-10f, 10f);
                float randomZ = Random.Range(-10f, 10f);
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
            }
        }
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

    void RunAway(GameObject target)
    {

        Vector3 direction = transform.position - target.transform.position;
        direction.y = 0; // Ensure movement is only horizontal


        vector = direction;

        /*
        int n = 0;
        n = (int)(10.0 * vector.x);
        n = n % 2;
        int m = -1;
        if (n == 0) { m = 1; } // calculate m=(-1)^n
        */

        //vector = new Vector3(0.7f * vector.x - (float)m * 0.7f * vector.z, 0f, (float)m * 0.7f * vector.x + 0.7f * vector.z);
        vector = vector.normalized * 1 / 6 * moveSpeed;


        characterController.Move(vector);

        //StartCoroutine(WaitAndExecute());
        isRunningAway = false; // Reset flag after the run away is complete
    }

    //isRunningAway = false; // Reset flag after the run away is complete

    IEnumerator WaitAndExecute()
    {
        yield return new WaitForSeconds(0.5f);
    }

    void OnTriggerEnter(Collider other)
    {
        momentum = triggerMomentum;

        if (other.CompareTag(targetTag))
        {
            StartCoroutine(FindLocalPlayerCoinController(other));
            player.GetCoin(100);
            photonView.RPC("DestroyRabbit", RpcTarget.All);
        }
    }

    void OnTriggerExit(Collider other)
    {
        momentum = initialMomentum;
    }

    private IEnumerator FindLocalPlayerCoinController(Collider other)
    {
        while (player == null)
        {
            foreach (var pcc in FindObjectsOfType<PlayerCoinController>())
            {
                if (pcc.photonView.IsMine && other.GetComponent<PhotonView>().IsMine)
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

    [PunRPC]
    void DestroyRabbit()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
