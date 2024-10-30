using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class DBInteraction : MonoBehaviourPunCallbacks
{
    public TMP_InputField inputField;
    //private NetworkManager networkManager;

    void Start()
    {
        // Initialize NetworkManager
        //networkManager = FindObjectOfType<NetworkManager>();
    }


    #region 로그인
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
    #endregion

    #region 커스터마이징
    // 1. Room 초기화: 외형 상태를 클라이언트 ID 없이 초기화합니다.
    public static void AddRoomAppearance(string roomID)
    {
        string query = $"INSERT INTO customize (roomID, builder, businessWoman, cashier, chef, fisherman, miner, nurse, police, security, worker, naked) " +
                       $"VALUES ('{roomID}', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)";

        AWSDBManager.ExecuteQuery(query);
        Debug.Log("Room appearance initialized in DB");
    }

    // 2. 특정 외형을 클라이언트 ID로 업데이트합니다.
    public static void SetAppearanceClientID(string roomID, string characterType, int clientID)
    {
        string query = $"UPDATE customize SET {characterType} = {clientID} WHERE roomID = '{roomID}'";
        AWSDBManager.ExecuteQuery(query);
        Debug.Log($"Updated {characterType} appearance to client ID {clientID} for room {roomID}.");
    }

    // 3. 특정 외형의 클라이언트 ID를 NULL로 설정해 초기화합니다.
    public static void ResetAppearanceByCharacterType(string roomID, string characterType)
    {
        Debug.Log("ResetAppearanceByCharacterType 실행 : "+characterType);
        string query = $"UPDATE customize SET {characterType} = NULL WHERE roomID = '{roomID}'";
        AWSDBManager.ExecuteQuery(query);
        Debug.Log($"Reset {characterType} appearance to NULL for room {roomID}.");
    }

    // 4. 특정 클라이언트 ID가 사용 중인 모든 외형을 초기화합니다.
    public static void ResetAppearanceByClientID(string roomID, int clientID)
    {
        string query = $"UPDATE customize SET " +
                       $"builder = IF(builder = {clientID}, NULL, builder), " +
                       $"businessWoman = IF(businessWoman = {clientID}, NULL, businessWoman), " +
                       $"cashier = IF(cashier = {clientID}, NULL, cashier), " +
                       $"chef = IF(chef = {clientID}, NULL, chef), " +
                       $"fisherman = IF(fisherman = {clientID}, NULL, fisherman), " +
                       $"miner = IF(miner = {clientID}, NULL, miner), " +
                       $"nurse = IF(nurse = {clientID}, NULL, nurse), " +
                       $"police = IF(police = {clientID}, NULL, police), " +
                       $"security = IF(security = {clientID}, NULL, security), " +
                       $"worker = IF(worker = {clientID}, NULL, worker), " +
                       $"naked = IF(naked = {clientID}, NULL, naked) " +
                       $"WHERE roomID = '{roomID}'";
        AWSDBManager.ExecuteQuery(query);
        Debug.Log($"Reset all appearances for client ID {clientID} in room {roomID}.");
    }

    // 5. 특정 roomID에 대해 사용 중이지 않은 무작위 외형을 선택하여 반환합니다.
    public static void GetRandomUnusedAppearance(string roomID, Action<string> resultCallback)
    {
        string query = $@"
        SELECT appearance FROM (
            SELECT 'builder' AS appearance FROM customize WHERE roomID = '{roomID}' AND builder IS NULL
            UNION ALL
            SELECT 'businessWoman' AS appearance FROM customize WHERE roomID = '{roomID}' AND businessWoman IS NULL
            UNION ALL
            SELECT 'cashier' AS appearance FROM customize WHERE roomID = '{roomID}' AND cashier IS NULL
            UNION ALL
            SELECT 'chef' AS appearance FROM customize WHERE roomID = '{roomID}' AND chef IS NULL
            UNION ALL
            SELECT 'fisherman' AS appearance FROM customize WHERE roomID = '{roomID}' AND fisherman IS NULL
            UNION ALL
            SELECT 'miner' AS appearance FROM customize WHERE roomID = '{roomID}' AND miner IS NULL
            UNION ALL
            SELECT 'nurse' AS appearance FROM customize WHERE roomID = '{roomID}' AND nurse IS NULL
            UNION ALL
            SELECT 'police' AS appearance FROM customize WHERE roomID = '{roomID}' AND police IS NULL
            UNION ALL
            SELECT 'security' AS appearance FROM customize WHERE roomID = '{roomID}' AND security IS NULL
            UNION ALL
            SELECT 'worker' AS appearance FROM customize WHERE roomID = '{roomID}' AND worker IS NULL
            UNION ALL
            SELECT 'naked' AS appearance FROM customize WHERE roomID = '{roomID}' AND naked IS NULL
        ) AS unusedAppearances
        ORDER BY RAND()
        LIMIT 1";

        AWSDBManager.ReadData(query, (reader) =>
        {
            string randomUnusedAppearance = null;

            if (reader.Read())
            {
                randomUnusedAppearance = reader["appearance"].ToString();
            }
            
            resultCallback(randomUnusedAppearance);
        });
    }

    // 6. 방이 삭제될 때 모든 외형 상태를 DB에서 삭제합니다.
    public static void RemoveRoomAppearance(string roomID)
    {
        string query = $"DELETE FROM customize WHERE roomID = '{roomID}'";
        AWSDBManager.ExecuteQuery(query);
        Debug.Log($"Room with ID {roomID} has been removed from the database.");
    }

    public static void ResetUnusedAppearances(string roomID, List<int> activeClientIDs)
    {
        string activeClientIDsString = string.Join(",", activeClientIDs);
        string query = $@"
            UPDATE customize SET 
                builder = IF(builder NOT IN ({activeClientIDsString}), NULL, builder),
                businessWoman = IF(businessWoman NOT IN ({activeClientIDsString}), NULL, businessWoman),
                cashier = IF(cashier NOT IN ({activeClientIDsString}), NULL, cashier),
                chef = IF(chef NOT IN ({activeClientIDsString}), NULL, chef),
                fisherman = IF(fisherman NOT IN ({activeClientIDsString}), NULL, fisherman),
                miner = IF(miner NOT IN ({activeClientIDsString}), NULL, miner),
                nurse = IF(nurse NOT IN ({activeClientIDsString}), NULL, nurse),
                police = IF(police NOT IN ({activeClientIDsString}), NULL, police),
                security = IF(security NOT IN ({activeClientIDsString}), NULL, security),
                worker = IF(worker NOT IN ({activeClientIDsString}), NULL, worker),
                naked = IF(naked NOT IN ({activeClientIDsString}), NULL, naked)
            WHERE roomID = '{roomID}'";
        
        AWSDBManager.ExecuteQuery(query);
        Debug.Log($"Reset unused appearances for room {roomID} based on active clients.");
    }

    public static void GetAllRoomIDs(Action<List<string>> resultCallback)
    {
        string query = "SELECT roomID FROM customize";
        List<string> roomIDs = new List<string>();

        AWSDBManager.ReadData(query, (reader) =>
        {
            while (reader.Read())
            {
                roomIDs.Add(reader.GetString(0));
            }
            
            resultCallback(roomIDs);
        });
    }
    #endregion
}
