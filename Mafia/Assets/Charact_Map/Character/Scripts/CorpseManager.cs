using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CorpseManager : MonoBehaviourPun
{
    [PunRPC]
    public void DisableAllCorpses()
    {
        GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");
        foreach (GameObject corpse in corpses)
        {
            corpse.SetActive(false);
        }
    }
}
