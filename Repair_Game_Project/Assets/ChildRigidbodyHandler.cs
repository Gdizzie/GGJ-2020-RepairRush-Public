using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildRigidbodyHandler : MonoBehaviour
{
    private Rigidbody[] rigidbodies;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
    }

    public void SetChildVelocityMagnitude(float velocity)
    {
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.velocity *= velocity;
        }
    }
}
