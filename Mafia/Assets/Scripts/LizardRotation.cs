using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardRotation : MonoBehaviour
{
    private Vector3 targetDirection;
    public float rotationSpeed = 1f; // rotation speed

    void Update()
    {
        lizardAI lizardai = GetComponentInParent<lizardAI>();
        targetDirection = lizardai.vector;
        //rotate to given vector direction
        RotateTowards(targetDirection);
    }

    void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        Vector3 eulerAngles = targetRotation.eulerAngles;
        eulerAngles.x = 0;
        Quaternion fixedRotation = Quaternion.Euler(eulerAngles);

        transform.rotation = Quaternion.Slerp(transform.rotation, fixedRotation, rotationSpeed * Time.deltaTime);
    }
}
