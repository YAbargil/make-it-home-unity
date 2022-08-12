using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigatorController : MonoBehaviour
{

    [SerializeField] GameObject m_Player;
    [SerializeField] GameObject m_Target;
    [SerializeField] GameObject m_Arrow;
    [SerializeField] RectTransform m_Navigator;
    RectTransform rect;

    
    // Start is called before the first frame update
    void Start()
    {
        rect=m_Arrow.GetComponent<RectTransform>();
    }

    // Update is called once per frame


       void Update()
    {
        Vector3[] corners = new Vector3[4];
        m_Navigator.GetLocalCorners(corners);
        float pointerScale = Vector3.Distance(corners[1], corners[2]);
        Vector3 direction = m_Target.transform.position - m_Player.transform.position;
        float angleToTarget = Vector3.SignedAngle(m_Player.transform.forward, direction, m_Player.transform.up);
        angleToTarget = Mathf.Clamp(angleToTarget, -90, 90) / 180.0f * pointerScale;
        rect.localPosition = new Vector3(angleToTarget, rect.localPosition.y, rect.localPosition.z);
    }
/*
    void Update1()
    {
        Vector3[] corners = new Vector3[4];
        m_Navigator.GetLocalCorners(corners);
        float navScale=Vector3.Distance(corners[1],corners[2]);
        Vector3 direction = m_Target.transform.position - m_.transform.position;
        float angleToTarget=Vector3.SignedAngle(m_.transform.forward,direction,m_.transform.up);
        angleToTarget = Mathf.Clamp(angleToTarget,-90,90) / 180.0f * navScale;
        m_rect.localPosition=new Vector3(angleToTarget,m_rect.localPosition.y,m_rect.localPosition.z);

        
    }
    */
}
