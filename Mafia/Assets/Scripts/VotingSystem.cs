using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class VotingSystem : MonoBehaviourPunCallbacks
{
    // ȸ            
    public bool syncedCriterion;

    [SerializeField]
    public GameObject voteManager;

    // Start is called before the first frame update
    void Start()
    {
        syncedCriterion = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!photonView.IsMine) return;

        // press V to activate vote and meeting
        // it must be revised to an appropriate condition
        /* if (Input.GetKeyDown(KeyCode.V))
         {
             photonView.RPC("StartVote", RpcTarget.All);
         }
 */
    }

    [PunRPC]
    void StartVote()
    {
        syncedCriterion = true;
        voteManager.SetActive(true);
        syncedCriterion = false;
    }
}
