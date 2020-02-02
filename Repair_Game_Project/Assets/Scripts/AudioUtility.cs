using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUtility : MonoBehaviour
{
    public static void PlayRandomClip(AudioSource audioSource, AudioClip[] audioClips, float minVolume, float maxVolume,
        float minPitch, float maxPitch, bool playOneShot)
    {
        int clipIndex = Random.Range(0, audioClips.Length);
        AudioClip audioClip = audioClips[clipIndex];
        
        float volume = Random.Range(minVolume, maxVolume);
        float pitch = Random.Range(minPitch, maxPitch);
        
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        
        if(playOneShot)
        {
            audioSource.PlayOneShot(audioClip);
        }
        else
        {
            audioSource.clip = audioClip;
            audioSource.Play();    
        }
        
    }

    public static void PlayRandomClip(AudioSource audioSource, AudioClips audioClips, bool playOneShot)
    {
        AudioUtility.PlayRandomClip(audioSource, audioClips.m_AudioClips, audioClips
            .m_MinVolume, audioClips.m_MaxVolume, audioClips.m_MinPitch, audioClips.m_MaxPitch, playOneShot);
    }
}
