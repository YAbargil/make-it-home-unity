using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstRadarObj : MonoBehaviour
{
    [SerializeField] Image m_Image;
    // Start is called before the first frame update
    
    void Start()
    {
        Radar.AddRadarObj(this.gameObject,m_Image);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy() 
    {
        Radar.RemoveRadarObj(this.gameObject) ;   
    }
}
