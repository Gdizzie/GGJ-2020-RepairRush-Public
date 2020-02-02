using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class EventList : MonoBehaviour
{
    public List<UnityEvent> m_EventList = new List<UnityEvent>();

    public List<float> m_EventTime = new List<float>();

    [FormerlySerializedAs("m_FireOnAwake")] public bool m_FireEventsOnAwake = false;

    // Start is called before the first frame update
    void Start()
    {
        if (m_FireEventsOnAwake)
            FireEventList();
    }

    public void FireEventList()
    {
        for (int i = 0; i < m_EventList.Count; i++)
        {
            float timeToFire = m_EventTime.Count <= i ? m_EventTime[i] : 0f;
            StartCoroutine(InvokeEvent(m_EventList[i], timeToFire));
        }
    }

    IEnumerator InvokeEvent(UnityEvent uEvent, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        uEvent?.Invoke();
    }

    public static void FireEventList(List<UnityEvent> eventList)
    {
        for (int i = 0; i < eventList.Count; i++)
        {
            eventList[i]?.Invoke();
        }
    }
}
