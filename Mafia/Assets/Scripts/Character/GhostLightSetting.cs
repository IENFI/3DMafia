using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GhostLightSetting : MonoBehaviour
{
    // Start is called before the first frame update
    public string ghostTag;
    GameObject[] GhostList;
    void Start()
    {

        if (!(bool)PhotonNetwork.LocalPlayer.CustomProperties["isDead"]){
            return;
        }
        ghostTag = "Ghost";
        GhostList = GameObject.FindGameObjectsWithTag(ghostTag);

        foreach (GameObject ghost in GhostList)
        {
            Transform FPCamera = ghost.transform.Find("FPCamera");
            Transform PointLight = FPCamera.transform.Find("PointLight");
            Light pointLight = PointLight.GetComponent<Light>();
            pointLight.enabled = true;
        }
    }

}
