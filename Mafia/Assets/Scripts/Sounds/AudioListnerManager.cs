using Photon.Pun;
using UnityEngine;

public class AudioListenerManager : MonoBehaviourPun
{
    void Start()
    {
        if (!photonView.IsMine) // 로컬 플레이어가 아니라면
        {
            // 해당 플레이어의 AudioListener를 비활성화
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }
        }
    }
}
