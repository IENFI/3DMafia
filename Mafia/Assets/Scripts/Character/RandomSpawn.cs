using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RandomSpawn : MonoBehaviourPunCallbacks
{
    public GameObject[] spawnPoints;
    private static HashSet<int> usedSpawnPoints = new HashSet<int>();
    private bool isSpawning = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // 통계 추적을 위한 변수들
    private static int totalAttempts = 0;
    private static int successfulSpawns = 0;
    private static int failedSpawns = 0;
    private static int emergencySpawns = 0;
    private static Dictionary<int, int> methodSuccesses = new Dictionary<int, int>()
    {
        {0, 0}, // 기본 방식
        {1, 0}, // SetPositionAndRotation
        {2, 0}  // Rigidbody 방식
    };

    private void Awake()
    {
        InitializeSpawnPoints();
    }

    private void InitializeSpawnPoints()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                //Debug.LogError($"[{gameObject.name}] Failed to find spawn points!");
            }
        }
    }

    // 통계 출력 함수
    private void LogSpawnStatistics()
    {
        float successRate = totalAttempts > 0 ? (float)successfulSpawns / totalAttempts * 100 : 0;
        string stats = $"\n=== SPAWN STATISTICS ===\n" +
                      $"Total Attempts: {totalAttempts}\n" +
                      $"Successful Spawns: {successfulSpawns}\n" +
                      $"Failed Spawns: {failedSpawns}\n" +
                      $"Emergency Spawns: {emergencySpawns}\n" +
                      $"Success Rate: {successRate:F1}%\n" +
                      $"Method Success Breakdown:\n" +
                      $"- Basic Transform: {methodSuccesses[0]} times\n" +
                      $"- SetPositionAndRotation: {methodSuccesses[1]} times\n" +
                      $"- Rigidbody Method: {methodSuccesses[2]} times\n" +
                      $"========================";
        Debug.Log(stats);
    }

    [PunRPC]
    void Spawn()
    {
        if (!photonView.IsMine) return;
        StartCoroutine(SpawnCoroutine());
    }

    private IEnumerator SpawnCoroutine()
    {
        if (isSpawning) yield break;
        isSpawning = true;

        originalPosition = transform.position;
        originalRotation = transform.rotation;
        //Debug.Log($"[{gameObject.name}] Starting spawn process. Original position: {originalPosition}");

        InitializeSpawnPoints();

        if (spawnPoints == null || spawnPoints.Length == 0 || spawnPoints[0] == null)
        {
            //LogSpawnError("No valid spawn points available");
            photonView.RPC("UpdateSpawnStatistics", RpcTarget.All, false, false, -1);
            isSpawning = false;
            yield break;
        }

        var playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.EnableControl(false);
        }

        yield return new WaitForSeconds(0.1f);

        int childCount = 0;
        bool isValidSpawnPoint = false;

        try
        {
            childCount = spawnPoints[0].transform.childCount;
            isValidSpawnPoint = childCount > 0;
        }
        catch (System.Exception e)
        {
            //LogSpawnError($"Error accessing spawn points: {e.Message}");
        }

        if (!isValidSpawnPoint)
        {
            //LogSpawnError("No valid child spawn points found");
            photonView.RPC("UpdateSpawnStatistics", RpcTarget.All, false, false, -1);
            isSpawning = false;
            if (playerController != null) playerController.EnableControl(true);
            yield break;
        }

        bool spawnSuccess = false;
        int maxAttempts = 5;
        int currentAttempt = 0;

        while (!spawnSuccess && currentAttempt < maxAttempts)
        {
            currentAttempt++;
            int randomIndex = Random.Range(0, childCount);
            Transform randomChild = null;
            bool isValidChild = false;

            try
            {
                randomChild = spawnPoints[0].transform.GetChild(randomIndex);
                isValidChild = true;
            }
            catch (System.Exception e)
            {
                //LogSpawnError($"Error getting spawn point {randomIndex}: {e.Message}");
            }

            if (!isValidChild)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            for (int methodIndex = 0; methodIndex < 3 && !spawnSuccess; methodIndex++)
            {
                bool positionUpdateSuccess = false;

                try
                {
                    switch (methodIndex)
                    {
                        case 0:
                            transform.position = randomChild.position;
                            transform.rotation = randomChild.rotation;
                            positionUpdateSuccess = true;
                            break;
                        case 1:
                            gameObject.transform.SetPositionAndRotation(randomChild.position, randomChild.rotation);
                            positionUpdateSuccess = true;
                            break;
                        case 2:
                            var rb = GetComponent<Rigidbody>();
                            if (rb != null)
                            {
                                rb.velocity = Vector3.zero;
                                rb.angularVelocity = Vector3.zero;
                                rb.MovePosition(randomChild.position);
                                rb.MoveRotation(randomChild.rotation);
                                positionUpdateSuccess = true;
                            }
                            break;
                    }
                }
                catch (System.Exception e)
                {
                    //LogSpawnError($"Error in spawn method {methodIndex}: {e.Message}");
                    positionUpdateSuccess = false;
                }

                if (!positionUpdateSuccess)
                {
                    continue;
                }

                yield return new WaitForSeconds(0.1f);

                float positionDifference = Vector3.Distance(originalPosition, transform.position);
                if (positionDifference > 0.1f)
                {
                    spawnSuccess = true;
                    photonView.RPC("UpdateSpawnStatistics", RpcTarget.All, true, false, methodIndex);
                    //Debug.Log($"[{gameObject.name}] Spawn successful on attempt {currentAttempt} using method {methodIndex}!");
                   // photonView.RPC("LogSpawnSuccess", RpcTarget.All, randomIndex, positionDifference);
                    break;
                }
            }

            if (!spawnSuccess)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        if (!spawnSuccess)
        {
            bool emergencySpawnSuccess = false;
            try
            {
                Transform forceSpawnPoint = spawnPoints[0].transform.GetChild(0);
                Vector3 targetPosition = forceSpawnPoint.position + Vector3.up * 0.1f;

                transform.position = targetPosition;
                transform.rotation = forceSpawnPoint.rotation;

                var characterController = GetComponent<CharacterController>();
                if (characterController != null)
                {
                    characterController.enabled = false;
                    transform.position = targetPosition;
                    characterController.enabled = true;
                }

                var rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }

                emergencySpawnSuccess = true;
                //Debug.Log($"[{gameObject.name}] Emergency spawn successful");
            }
            catch (System.Exception e)
            {
                //LogSpawnError($"Emergency spawn failed: {e.Message}");
            }

            if (emergencySpawnSuccess)
            {
                float finalDifference = Vector3.Distance(originalPosition, transform.position);
                //photonView.RPC("LogSpawnSuccess", RpcTarget.All, 0, finalDifference);
                photonView.RPC("UpdateSpawnStatistics", RpcTarget.All, true, true, -1);
            }
            else
            {
                photonView.RPC("UpdateSpawnStatistics", RpcTarget.All, false, false, -1);
            }
        }

        yield return new WaitForSeconds(0.2f);

        float finalPositionDifference = Vector3.Distance(originalPosition, transform.position);
        if (finalPositionDifference < 0.1f)
        {
            Debug.LogError($"[{gameObject.name}] Critical spawn failure. Final position change: {finalPositionDifference}");
        }

        if (playerController != null)
        {
            playerController.EnableControl(true);
        }

        isSpawning = false;
    }

    private void LogSpawnError(string message)
    {
        //Debug.LogError($"[{gameObject.name}] {message}");
        photonView.RPC("LogSpawnErrorRPC", RpcTarget.All, message);
    }

    [PunRPC]
    private void LogSpawnSuccess(int index, float positionChange)
    {
        //Debug.Log($"[{gameObject.name}] Spawn confirmed at index {index}. Position changed by {positionChange:F2} units");
    }

    [PunRPC]
    private void LogSpawnErrorRPC(string message)
    {
        //Debug.LogError($"[{gameObject.name}] {message}");
    }

    [PunRPC]
    private void UpdateSpawnStatistics(bool isSuccess, bool isEmergency, int methodUsed)
    {
        totalAttempts++;

        if (isSuccess)
        {
            successfulSpawns++;
            if (isEmergency)
            {
                emergencySpawns++;
            }
            else if (methodUsed >= 0 && methodUsed < 3)
            {
                methodSuccesses[methodUsed]++;
            }
        }
        else
        {
            failedSpawns++;
        }

        LogSpawnStatistics();
    }

    public void ResetSpawnSystem()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        photonView.RPC("ResetSpawnStatisticsRPC", RpcTarget.All);
    }

    [PunRPC]
    private void ResetSpawnStatisticsRPC()
    {
        isSpawning = false;
        totalAttempts = 0;
        successfulSpawns = 0;
        failedSpawns = 0;
        emergencySpawns = 0;
        foreach (var key in methodSuccesses.Keys)
        {
            methodSuccesses[key] = 0;
        }
        usedSpawnPoints.Clear();
        LogSpawnStatistics();
    }

    private void OnLevelWasLoaded(int level)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ResetSpawnSystem();
        }
    }

    [PunRPC]
    private void ClearSpawnPoints()
    {
        usedSpawnPoints.Clear();
    }
}