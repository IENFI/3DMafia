using Photon.Pun;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class VoiceManager : MonoBehaviourPunCallbacks
{
    public byte playerGroup = 1;  // 플레이어 그룹 (채널)
    public byte ghostGroup = 2;   // 유령 그룹 (채널)

    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            // 방에 들어갔을 때 모든 플레이어의 음성 그룹을 설정
            SetupVoiceGroupForAll();
        }
    }

    private void SetupVoiceGroupForAll()
    {
        foreach (var view in FindObjectsOfType<PhotonVoiceView>())
        {
            var photonView = view.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                // 플레이어의 음성 그룹을 플레이어 그룹으로 설정
                SetVoiceGroup(view, playerGroup);
            }
        }
    }

    public void OnPlayerDeath(GameObject deadPlayer)
    {
        if (deadPlayer.TryGetComponent(out PhotonVoiceView voiceView))
        {
            // 죽은 플레이어의 음성 그룹을 유령 그룹으로 설정
            SetVoiceGroup(voiceView, ghostGroup);
        }
    }

    private void SetVoiceGroup(PhotonVoiceView voiceView, byte group)
    {

        // 중복 방지를 위해 기존 Recorder 제거
        RemoveDuplicateRecorders(voiceView.gameObject);
        var recorder = voiceView.GetComponentInChildren<Recorder>();
        if (recorder != null)
        {
            // 중복 추가 방지: 그룹이 이미 설정된 경우 업데이트
            if (recorder.InterestGroup != group)
            {
                recorder.InterestGroup = group;
            }
        }
        else
        {
            // Recorder가 없으면 새로 추가
            recorder = voiceView.gameObject.AddComponent<Recorder>();
            recorder.InterestGroup = group;
        }

        
    }

    private void RemoveDuplicateRecorders(GameObject gameObject)
    {
        var recorders = gameObject.GetComponents<Recorder>();
        if (recorders.Length > 1)
        {
            for (int i = 1; i < recorders.Length; i++)
            {
                Destroy(recorders[i]);
            }
        }
    }
}
