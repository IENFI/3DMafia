using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System;

public class PlayerDBController : MonoBehaviour
{
    public string playerName = "none"; // 플레이어의 이름
    public bool connectSign = false;

    public void SetConnectSign(bool connectSign)
    {
        this.connectSign = connectSign;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
        UpdateLastActiveTime();
        DeleteInactivePlayers();
    }

    private void Start()
    {
        // 50초마다 UpdateLastActiveTime 메서드를 호출
        InvokeRepeating("UpdateLastActiveTime", 0f, 50f);
        // 1분마다 DeleteInactivePlayers 메서드를 호출
        InvokeRepeating("DeleteInactivePlayers", 0f, 60f);
    }

    private void UpdateLastActiveTime()
    {
        //Debug.Log("UpdateLastActiveTime");

        string query = $"UPDATE player SET last_active = NOW() WHERE name = ('{playerName}')";
        AWSDBManager.ExecuteQuery(query);
    }

    public void DeleteInactivePlayers()
    {
        //Debug.Log("DeleteInactivePlayers");
        string query = "DELETE FROM player WHERE last_active IS NULL OR last_active < NOW() - INTERVAL 1 MINUTE";
        AWSDBManager.ExecuteQuery(query);
    }
}
