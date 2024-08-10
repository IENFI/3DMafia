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
            // 비활성화
            //corpse.SetActive(false);

            // 씬에서 아예 없애기
            Destroy(corpse);
        }
    }

}
