using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class NPC : MonoBehaviour
{
    public float minFireWaitTime = 3f;
    public float maxFireWaitTime = 6f;

    public float minFireRate = 0.1f;
    public float maxFireRate = 0.2f;
    public float maxFireLength = 2f;
    public float minFireLength = 0.3f;

    public AudioClips fireSound;
    
    public Bullet bulletPrefab;
    public GameObject gun;
    public GameObject firePoint;
    public GameObject muzzleFlash;

    public LookAtConstraint lookAtConstraint;

    //public bool fireAtStart = false;

    private AudioSource audioSource;

    Transform playerTr;
    public bool debugGunRay;

    IEnumerator Start()
    {
        if(fireSound)
            if (!audioSource)
                audioSource = GetComponent<AudioSource>();

        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }

        yield return new WaitForEndOfFrame();
        playerTr = WheelDriveCustom.s_player.transform;

        //if (fireAtStart)
        //{
        //    yield return new WaitForSeconds(2f);
        //    FireRandomDelayLoop();
        //}

        ConstraintSource cs = new ConstraintSource {sourceTransform = playerTr, weight = 1};
        lookAtConstraint.RemoveSource(0);
        lookAtConstraint.AddSource(cs);
    }

    void FixedUpdate()
    {
        if(debugGunRay)
        {
            Debug.DrawRay(firePoint.transform.position, (playerTr.position + playerTr.up * 1f) - firePoint.transform.position);
        }
    }

    public void FireRandomDelayLoop(float initialDelay = 0f)
    {
        StartCoroutine( NPCFire(initialDelay > 0f ? initialDelay : UnityEngine.Random.Range(minFireWaitTime, maxFireWaitTime)));
    }

    private IEnumerator NPCFire(float delay)
    {
        yield return new WaitForSeconds(delay);

        float fireStartTime = Time.time;
        float fireTime = UnityEngine.Random.Range(minFireLength, maxFireLength);
        float fireRate = maxFireRate;
        while (Time.time - fireStartTime < fireTime)
        {
            yield return new WaitForFixedUpdate();
            AudioUtility.PlayRandomClip(audioSource, fireSound, true);
            //audioSource.PlayOneShot(fireSound);
            LeanPool.Spawn(bulletPrefab.gameObject, firePoint.transform.position, Quaternion.LookRotation((playerTr.position + playerTr.up * 1f) - firePoint.transform.position, transform.up)); //gun.transform.rotation);
            LeanPool.Spawn(muzzleFlash, firePoint.transform.position, firePoint.transform.rotation);
            yield return new WaitForSeconds(UnityEngine.Random.Range(minFireRate, maxFireRate));
        }

        FireRandomDelayLoop();
    }
}
