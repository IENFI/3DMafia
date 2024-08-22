using UnityEngine;
using MySql.Data.MySqlClient;
using System;

public class AWSDBManager : MonoBehaviour
{
    private static string server = "rds-mysql-dyingmessage.cvke0c8ms3i1.ap-northeast-2.rds.amazonaws.com";
    private static string database = "DyingMessage";
    private static string uid = "admin";
    private static string password = "whdgkqtjfrP1234";
    private static string port = "3306";
    private static string connectionString =
        $"Server={server};Port={port};Database={database};Uid={uid};Pwd={password};SslMode=None;";

    public static MySqlConnection GetDBConnection()
    {
        try
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            return connection;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Database connection error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Debug.LogError($"Inner exception: {ex.InnerException.Message}");
            }
            return null;
        }
    }
    
    public static void ExecuteQuery(string query)
    {
        using (MySqlConnection connection = GetDBConnection())
        {
            if (connection != null)
            {
                try
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    cmd.ExecuteNonQuery();
                    Debug.Log("Query executed successfully");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Query execution error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Debug.LogError($"Inner exception: {ex.InnerException.Message}");
                    }
                }
            }
        }
    }

    // 데이터를 읽어오는 메서드
    public static void ReadData(string query, Action<MySqlDataReader> dataHandler)
    {
        using (MySqlConnection connection = GetDBConnection())
        {
            if (connection != null)
            {
                try
                {
                    connection.Open();
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        dataHandler(reader);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Data reading error: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Debug.LogError($"Inner exception: {ex.InnerException.Message}");
                    }
                }
            }
        }
    }
}