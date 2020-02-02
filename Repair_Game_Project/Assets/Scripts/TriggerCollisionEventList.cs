using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TriggerCollisionEventList : MonoBehaviour
{
    [Serializable]
    public class OnTriggerEvent : UnityEvent<Collider>{}
    

    public List<OnTriggerEvent> m_OnTriggerEvents = new List<OnTriggerEvent>();
    public List<string> m_TriggerTags = new List<string>();


    private void OnTriggerEnter(Collider other)
    {
        for (int i = 0; i < m_OnTriggerEvents.Count; i++)
        {
            if(other.CompareTag(m_TriggerTags[i]))
                m_OnTriggerEvents[i]?.Invoke(other);
        }
    }
}
