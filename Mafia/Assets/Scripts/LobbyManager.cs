using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviour
{
    private PhotonView pv;

    private Hashtable CP;

    private Ray ray;
    private RaycastHit hit;
    private new Camera camera;

    public Material[] playerMt;

    public Renderer player;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        camera = Camera.main;

        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Color", -1 } });
        CP = PhotonNetwork.LocalPlayer.CustomProperties;
    }

    private void Update()
    {
        ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100.0f, 1 << 6))
            {
                string item = hit.collider.gameObject.name;
                string[] words = item.Split('_');
                SetColorProperty(int.Parse(words[1]));
            }
        }
    }

    public void SetColorProperty(int num)
    {
        CP["Color"] = num;
        SetMt(num);
    }

    void SetMt(int num)
    {
        player.material = playerMt[num - 1];
    }

}