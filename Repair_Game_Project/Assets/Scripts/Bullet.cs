using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletVelocity = 5f;
    public float lifeTime = 5f;

    public GameObject metalImpactEffectPrefab;
    public GameObject dirtImpactEffectPrefab;

    public AudioClips metalImpactAudioClips;
    public AudioClips dirtImpactAudioClips;

    public bool useTraumaInducer;
    public TraumaInducer traumaInducer;

    public AudioEffect audioEffect;

    private float spawnTime = 0;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if(Time.time - spawnTime > lifeTime)
            LeanPool.Despawn(gameObject);
        
        //transform.Translate(transform.forward * bulletVelocity);
        transform.position += transform.forward * bulletVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Bullet collision: " + other.name, other.gameObject);
        if (other.CompareTag("Player"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                //Debug.Log("Point of contact: " + hit.point);
                LeanPool.Spawn(metalImpactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
            }
            //Debug.Log("Inducing Trauma");
            if(useTraumaInducer)
                traumaInducer.SendTrauma();
            AudioUtility.PlayRandomClip(LeanPool.Spawn(audioEffect, transform.position, Quaternion.identity).audioSource, metalImpactAudioClips, true);

            if (UnityEngine.Random.Range(0, 1) == 1)
                GameController.s_playerController.damage1(1f);
            else GameController.s_playerController.damage1(2f);
        }
        else if(other.CompareTag("Ground"))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                //Debug.Log("Point of contact: " + hit.point);
                LeanPool.Spawn(dirtImpactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
            }
            AudioUtility.PlayRandomClip(LeanPool.Spawn(audioEffect, transform.position, Quaternion.identity).audioSource, dirtImpactAudioClips, true);
        }
        LeanPool.Despawn(gameObject);
    }

    private void OnEnable()
    {
        spawnTime = Time.time;
    }
}
