using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    public void tryRepair1(float value) {
        Debug.Log("try repair 1");
        if (!m_isRepairing1 && !m_isRepairing2) repair1(value);
    }
    public void tryRepair2(float value) {
        Debug.Log("try repair 1");
        if (!m_isRepairing1 && !m_isRepairing2) repair2(value);
    }

    float m_curHealthValue1 = 0f;
    float m_curHealthValue2 = 0f;
    float m_baseHealth1 = 0f;
    float m_baseHealth2 = 0f;
    public float m_repairSpeed1;
    public float m_repairSpeed2;
    float m_repairValue1 = 0f;
    float m_repairValue2 = 0f;
    float m_repairStart1 = 0f;
    float m_repairLength1 = 0f;
    float m_repairStart2 = 0f;
    float m_repairLength2 = 0f;
    bool m_isRepairing1 = false;
    bool m_isRepairing2 = false;
    private Animator anim;
    float m_repairEndTime = 0f;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void repair1(float value) {
        float extra = 0f;
        if (m_isRepairing1) {
            float add = Mathf.Lerp(0f, m_repairValue1, Time.time - m_repairStart1 / m_repairLength1);
            extra = m_repairValue1 - add;
        }

        m_repairValue1 = value + extra;
        m_isRepairing1 = true;
        m_repairStart1 = Time.time;
        m_repairLength1 = m_repairValue1 / m_repairSpeed1;

        doRepairAnimation1(m_repairLength1);
        startScaleBubble(0f);
    }
    void repair2(float value) {
        float extra = 0f;
        if (m_isRepairing2) {
            float add = Mathf.Lerp(0f, m_repairValue2, Time.time - m_repairStart2 / m_repairLength2);
            extra = m_repairValue2 - add;
        }

        m_repairValue2 = value + extra;
        m_isRepairing2 = true;
        m_repairStart2 = Time.time;
        m_repairLength2 = m_repairValue2 / m_repairSpeed2;

        doRepairAnimation2(m_repairLength2);
        startScaleBubble(0f);
    }

    void doRepairAnimation1(float length) {

        anim.SetBool("Repair1", true);

    }
    void doRepairAnimation2(float length) {
        anim.SetBool("Repair2", true);
    }

    public void damage1(float value) {
        m_baseHealth1 += value;
    }
    public void damage2(float value) {
        m_baseHealth2 += value;
    }

    float m_healthDecayRate1 = 5f;
    float m_healthDecayRate2 = 5f;

    public ParticleSystem m_smokeEffect1;
    public ParticleSystem m_smokeEffect2;
    void updateHealth() {
        m_baseHealth1 -= Time.deltaTime * m_healthDecayRate1;
        m_baseHealth2 -= Time.deltaTime * m_healthDecayRate2;
        float repairValue1 = 0f;
        float repairValue2 = 0f;
        if (m_isRepairing1) {
            float t = (Time.time - m_repairStart1) / m_repairLength1;
            repairValue1 = Mathf.Lerp(0f, m_repairValue1, t);

            if (t >= 1) {
                m_isRepairing1 = false;
                m_repairEndTime = Time.time;
                anim.SetBool("Repair1", false);
            }
        }
        if (m_isRepairing2) {
            float t = (Time.time - m_repairStart2) / m_repairLength2;
            repairValue2 = Mathf.Lerp(0f, m_repairValue2, t);

            if (t >= 1) {
                m_isRepairing2 = false;
                m_repairEndTime = Time.time;
                anim.SetBool("Repair2", false);
            }
        }

        float prevHealth1 = m_curHealthValue1;
        float prevHealth2 = m_curHealthValue2;
        m_curHealthValue1 = m_baseHealth1 + repairValue1;
        m_curHealthValue2 = m_baseHealth2 + repairValue2;

        //smoke effect
        const float threshold1 = 90f;
        const float threshold2 = 70f;
        const float threshold3 = 50f;
        const float threshold4 = 20f;

        if(m_curHealthValue1 >= threshold1 && prevHealth1 < threshold1) {
            m_smokeEffect1.gameObject.SetActive(false);
        }
        else {
            if (prevHealth1 >= threshold1) m_smokeEffect1.gameObject.SetActive(true);
            //Debug.Log("smoke effect");
            var main = m_smokeEffect1.main;
            main.startLifetime = Mathf.Lerp(0f, 5f, (1 - (m_curHealthValue1 / threshold1)));            
        }
    }

    //engineer//
    public GameObject m_engineerObject;
    public Vector3 m_engineerOffset1;
    public Quaternion m_engineerRotation1;
    public Vector3 m_engineerOffset2;
    public Quaternion m_engineerRotation2;
    Vector3 m_engineerOffsetBase;
    Quaternion m_engineerRotationBase;
    bool m_doingHelpPrompt = false;
    const float m_helpPromptMargin = 3f;
    public Canvas m_canvas;
    public Image m_helpBubble1;
    public Image m_helpBubble2;
    RectTransform m_helpBubbleTransform1;
    RectTransform m_helpBubbleTransform2;

    float m_curBubbleScale = 0f;
    public float m_bubbleScaleSpeed = 5f;
    Coroutine m_curScaleBubbleCoroutine = null;
    void startScaleBubble(float targetScale) {
        if (m_curScaleBubbleCoroutine != null) StopCoroutine(m_curScaleBubbleCoroutine);
        m_curScaleBubbleCoroutine = StartCoroutine(scaleBubble(targetScale));
    }
    IEnumerator scaleBubble(float targetScale) {
        
        float startTime = Time.time;

        while(m_curBubbleScale != targetScale) {
            m_curBubbleScale = Mathf.Lerp(m_curBubbleScale, targetScale, Time.deltaTime * m_bubbleScaleSpeed);
            yield return new WaitForEndOfFrame();
        }

        if(m_curBubbleScale <= 0f) {
            m_helpBubble1.enabled = false;
            m_helpBubble2.enabled = false;
        }

        m_curScaleBubbleCoroutine = null;
        yield return 0;
    }
    void chooseHelpPrompt() {
        if(!m_isRepairing1 && !m_isRepairing2 && !m_doingHelpPrompt && Time.time - m_repairEndTime > m_helpPromptMargin) {
            doHelpPrompt();            
        }

        if (m_helpBubble1.IsActive() || m_helpBubble2.IsActive()) {
            Vector3 scale = new Vector3(m_curBubbleScale, m_curBubbleScale, m_curBubbleScale);
            m_helpBubbleTransform1.localScale = scale;
            m_helpBubbleTransform2.localScale = scale;
        }
    }
    void doHelpPrompt() {
        m_doingHelpPrompt = true;

        int choice = m_curHealthValue1 > m_curHealthValue2 ? 1 : 2;
        //start animation

        //speech bubble
        if (choice == 1) m_helpBubble1.enabled = true;
        else if (choice == 2) m_helpBubble2.enabled = true;
        startScaleBubble(1.2f);
    }
    void updateEngiePos() {
        Vector3 pos = m_engineerOffsetBase;
        Quaternion rot = m_engineerRotationBase;

        if (m_isRepairing1) {
            pos += m_engineerOffset1;
            rot *= m_engineerRotation1;
        }
        else if (m_isRepairing2) {
            pos += m_engineerOffset2;
            rot *= m_engineerRotation2;
        }

        m_engineerObject.transform.localPosition = pos;
        m_engineerObject.transform.localRotation = rot;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameController.setPlayerController(this);

        m_baseHealth1 = 100f;
        m_baseHealth2 = 100f;

        m_helpBubble1.enabled = false;
        m_helpBubble2.enabled = false;
        m_helpBubbleTransform1 = m_helpBubble1.gameObject.GetComponent<RectTransform>();
        m_helpBubbleTransform2 = m_helpBubble2.gameObject.GetComponent<RectTransform>();

        m_engineerOffsetBase = m_engineerObject.transform.localPosition;
        m_engineerRotationBase = m_engineerObject.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        chooseHelpPrompt();

        updateHealth();

        updateEngiePos();
    }
}
