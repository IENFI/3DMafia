using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class DBInteraction : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputField;
    //private NetworkManager networkManager;

    void Start()
    {
        // Initialize NetworkManager
        //networkManager = FindObjectOfType<NetworkManager>();
    }

    public static void AddPlayer(string playerName)
    {
        string query = $"INSERT INTO player (name) VALUES ('{playerName}')";
        AWSDBManager.ExecuteQuery(query);
        Debug.Log("Player added to the database");
        PlayerDBController playerDBController = FindObjectOfType<PlayerDBController>();
        playerDBController.SetConnectSign(true);
        playerDBController.SetPlayerName(playerName);
        // PrintPlayers();
        // networkManager.Connect(); // Connect to the network
    }

    public static bool DeletePlayer(string playerName)
    {
        string query = $"DELETE FROM player WHERE name = '{playerName}'";
        AWSDBManager.ExecuteQuery(query);
        Debug.Log("Player deleted from the database");
        return true;
        // PrintPlayers();
    }

    // 플레이어 목록을 콘솔에 출력하는 메서드 (테스트용)
    /*public void PrintPlayers()
    {
        string query = "SELECT * FROM player";
        AWSDBManager.ReadData(query, (reader) =>
        {
            while (reader.Read())
            {
                int id = reader.GetInt32("id");
                string name = reader.GetString("name");
                Debug.Log($"Player ID: {id}, Name: {name}");
            }
        });
    }*/

    public static bool Login(string nickName)
    {
        // string id = inputField.text;

        // Check if the player name already exists
        string query = $"SELECT COUNT(*) FROM player WHERE name = '{nickName}'";
        int count = 0;

        AWSDBManager.ReadData(query, (reader) =>
        {
            if (reader.Read())
            {
                count = reader.GetInt32(0);
            }
        });

        if (count == 0)
        {
            // If the name does not exist, add the player
            AddPlayer(nickName);
            return true;
        }
        else
        {
            // If the name already exists, log the message
            Debug.Log("Player name already exists in the database");
            return false;
        }

        // Print the current list of players
        // PrintPlayers();
    }
}
