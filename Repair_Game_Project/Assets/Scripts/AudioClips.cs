using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New AudioClips", menuName = "AudioClips")]
public class AudioClips : ScriptableObject
{
    [FormerlySerializedAs("m_FootstepAudioClips")] public AudioClip[] m_AudioClips;
    [Range(0f, 1f)]
    public float m_MinVolume = 1f;
    [Range(0f, 1f)]
    public float m_MaxVolume = 1f;
    [Range(-3f, 3f)]
    public float m_MinPitch = 1f;
    [Range(-3f, 3f)]
    public float m_MaxPitch = 1f;
}
