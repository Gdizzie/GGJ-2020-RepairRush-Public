using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class TrackSpawner : MonoBehaviour
{
    public GameObject m_trackPiece1;
    public GameObject m_trackPiece2;
    public GameObject m_trackPiece3;
    public GameObject m_trackPiece4;
    public GameObject m_trackPiece5;

    public List<Transform> m_spawnPoints = new List<Transform>();

    LeanGameObjectPool m_pool;


    GameObject m_curPivot;
    public int m_minTrackSize = 5;
    int m_curTrackCount = 0;
    public void decreaseTrackCount() {
        m_curTrackCount--;
    }

    void spawnTrack(int choice = 0) {
        GameObject track = m_trackPiece1;
        if (choice == 1) track = m_trackPiece2;
        else if (choice == 2) track = m_trackPiece3;
        else if (choice == 3) track = m_trackPiece4;
        else if (choice == 4) track = m_trackPiece5;

        GameObject newTrack = LeanPool.Spawn(track);
        newTrack.transform.position = m_curPivot.transform.position;

        m_curPivot = newTrack.transform.Find("Pivot").gameObject;

        m_curTrackCount++;
    }

    // Start is called before the first frame update
    void Start()
    {
        //m_pool = new LeanGameObjectPool();
        TrackPiece.setSpawner(this);

        m_curPivot = new GameObject();
        m_curPivot.transform.position = Vector3.zero;

        for(int i = 0; i < m_minTrackSize; i++) {
            spawnTrack(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        while(m_curTrackCount < m_minTrackSize) {
            spawnTrack(0);
        }
    }
}
