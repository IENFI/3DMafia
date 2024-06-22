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
            corpsesInRange++;
            Debug.Log("Corpse entered report radius.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Corpse"))
        {
            corpsesInRange--;
            Debug.Log("Corpse exited report radius.");
        }
    }

    public bool IsCorpseInRange()
    {
        return corpsesInRange > 0;
    }
}
