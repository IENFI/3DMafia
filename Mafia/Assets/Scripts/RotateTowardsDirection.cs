using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsDirection : MonoBehaviour
{
    private Vector3 targetDirection;
    public float rotationSpeed = 20f; // rotation speed

    void Update()
    {
        RabbitAI rabbitAI = GetComponentInParent<RabbitAI>();
        targetDirection = rabbitAI.vector;
        //rotate to given vector direction
        RotateTowards(targetDirection);
    }

    void RotateTowards(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}

