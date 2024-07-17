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

    public void DestoryCorpse()
    {
        corpsesInRange = 0;
    }

    public bool IsCorpseInRange()
    {
        return corpsesInRange > 0;
    }
}
