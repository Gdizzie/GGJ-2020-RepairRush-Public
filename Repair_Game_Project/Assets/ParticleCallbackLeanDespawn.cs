using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;

public class ParticleCallbackLeanDespawn : MonoBehaviour
{
    void OnParticleSystemStopped()
    {
        //Debug.Log("Despawning: " + gameObject.name, gameObject);
        LeanPool.Despawn(gameObject);
    }
}