using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class Obstacle : MonoBehaviour
{
    //control values//
    public bool m_repair1;
    public bool m_repair2;
    public float m_repairValue;

    //animate//
    Vector3 m_basePos;
    Vector3 m_baseRotation;
    Vector3 m_animateOffset = Vector3.zero;
    Vector3 m_animateRotation = Vector3.zero;

    //bob
    public bool m_animateBob = false;
    public float m_animateBobPeriod = 1f;
    public float m_animateBobHalfPeriod = 0.5f;
    public float m_animateBobSpeed = 1f;
    public Vector3 m_animateBobMin = Vector3.zero;
    public Vector3 m_animateBobMax = Vector3.zero;    
    
    IEnumerator animateBob() {
        float startTime = Time.time;
        while (m_animateBob) {
            while((Time.time - startTime) % m_animateBobPeriod < m_animateBobHalfPeriod) {
                m_animateOffset = Vector3.Lerp(m_animateOffset, m_animateBobMax, Time.deltaTime * m_animateBobSpeed);
                //Debug.Log("period 1");
                yield return new WaitForEndOfFrame();
            }
            while ((Time.time - startTime) % m_animateBobPeriod >= m_animateBobHalfPeriod) {
                m_animateOffset = Vector3.Lerp(m_animateOffset, m_animateBobMin, Time.deltaTime * m_animateBobSpeed);
                //Debug.Log("period 2");
                yield return new WaitForEndOfFrame();
            }
        }

        yield return 0;
    }

    //rotate
    public bool m_animateRotate = false;
    public Vector3 m_animateRotateSpeed = Vector3.zero;
    IEnumerator animateRotate() {
        while (m_animateRotate) {
            m_animateRotation += Time.deltaTime * m_animateRotateSpeed;

            yield return new WaitForEndOfFrame();
        }

        yield return 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        //animation//
        //clamp anim values
        m_animateBobHalfPeriod = Mathf.Clamp(m_animateBobHalfPeriod, 0.1f, 0.9f * m_animateBobPeriod);
        //start
        if (m_animateBob) StartCoroutine(animateBob());
        if (m_animateRotate) StartCoroutine(animateRotate());

        m_basePos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = m_basePos;
        if (m_animateBob) pos += m_animateOffset;
        transform.position = pos;

        Vector3 rot = m_baseRotation;
        if (m_animateRotate) rot += m_animateRotation;
        transform.Rotate(rot);
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Pickup Radius") {
            //affect player
            if (m_repair1) GameController.s_playerController.tryRepair1(m_repairValue);
            if (m_repair2) GameController.s_playerController.tryRepair2(m_repairValue);

            //despawn
            LeanPool.Despawn(gameObject.transform);
        }
    }

    void OnDisable()
    {
        TrackPiece.s_obstacleSpawner.currentObstacles--;
    }
}
