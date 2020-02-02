using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioEffect : MonoBehaviour
{
    public AudioSource audioSource;

    public float despawnTime = 5f;

    private float onTime = 0f;

    void OnEnable()
    {
        onTime = Time.time;
        StartCoroutine(DespawnCheck());
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    IEnumerator DespawnCheck(float initialDelay = 0.5f, float checkRate = 1f)
    {
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            if (!audioSource.isPlaying)
                LeanPool.Despawn(this);
            yield return new WaitForSeconds(checkRate);
        }
    }
}
