using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(AudioSource))]
public class CollisionDetection : EventList
{
    public float maxFrontCollisionAngle;
    public bool debugCollision = false;

    public AudioEffect audioEffectPrefab;

    public AudioClips crashAudioClips;

    // Update is called once per frame
    void Update()
    {
        if (!debugCollision)
            return;

        Vector3 leftCollisionDirection = transform.forward * 3f;

        leftCollisionDirection = Quaternion.AngleAxis(-maxFrontCollisionAngle, transform.up) * leftCollisionDirection;

        Debug.DrawRay(transform.position + (transform.up * 1), leftCollisionDirection, Color.magenta);
        
        Vector3 rightCollisionDirection = transform.forward * 3f;

        rightCollisionDirection = Quaternion.AngleAxis(maxFrontCollisionAngle, transform.up) * rightCollisionDirection;

        Debug.DrawRay(transform.position + (transform.up * 1), rightCollisionDirection, Color.magenta);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Vector3 collisionDir = collision.GetContact(0).point - transform.position;
            if (Vector3.Angle(collisionDir, transform.forward) < maxFrontCollisionAngle)
            {
                Debug.Log("Front Collision");
                base.FireEventList();
            }
        }

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Ground"))
            if (collision.relativeVelocity.magnitude > 2) AudioUtility.PlayRandomClip(LeanPool.Spawn(audioEffectPrefab, transform.position, Quaternion.identity).audioSource, crashAudioClips, true);
    }
}
