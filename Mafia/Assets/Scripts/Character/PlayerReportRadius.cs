using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReportRadius : MonoBehaviour
{
    private int corpsesInRange = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Corpse"))
        {
            corpsesInRange = 1;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Corpse"))
        {
            corpsesInRange = 0;
        }
    }

    void Update()
    {
        CheckForCorpses();
    }

    private void CheckForCorpses()
    {
        // 현재 씬에서 모든 'corpse' 태그를 가진 GameObject 찾기
        GameObject[] corpses = GameObject.FindGameObjectsWithTag("Corpse");

        if (corpses.Length == 0)
        {
            corpsesInRange = 0; // Corpse가 없으면 0으로 설정
            // Debug.Log("No corpses found in the scene. corpsesInRange set to 0.");
        }
    }

    public void DestoryCorpse()
    {
        corpsesInRange = 0;
    }

    public bool IsCorpseInRange()
    {
        return corpsesInRange > 0;
    }
}
