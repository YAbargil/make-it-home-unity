using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RadarObject 
{
    public Image image {get;set;}
    public GameObject radarObj {get;set;}
}
public class Radar : MonoBehaviour
{

    [SerializeField] Transform m_PlayerPoisition;
    public float mapScale=1.3f;
    public static List<RadarObject> radarObjs=new List<RadarObject>();
    public static void AddRadarObj(GameObject o,Image i)
    {
        Image Inst_image=Instantiate(i);
        radarObjs.Add(new RadarObject { image=Inst_image,radarObj=o});
    }
  
    public static void RemoveRadarObj(GameObject o)
    {
      List<RadarObject> newList = new List<RadarObject>();
        for (int i = 0; i < radarObjs.Count; i++)
        {
            if (radarObjs[i].radarObj == o)
            {
                Destroy(radarObjs[i].image);
                continue;
            }
            else
                newList.Add(radarObjs[i]);
        }

        radarObjs.RemoveRange(0, radarObjs.Count);
        radarObjs.AddRange(newList);

    }
    // Update is called once per frame
    void Update()
    {
        if (m_PlayerPoisition == null) return;
        foreach (RadarObject ro in radarObjs)
        {
            Vector3 radarPos = ro.radarObj.transform.position - m_PlayerPoisition.position;
            float distToObject = Vector3.Distance(m_PlayerPoisition.position, ro.radarObj.transform.position) * mapScale;

            float deltay = Mathf.Atan2(radarPos.x, radarPos.z) * Mathf.Rad2Deg - 270 - m_PlayerPoisition.eulerAngles.y;
            radarPos.x = distToObject * Mathf.Cos(deltay * Mathf.Deg2Rad) * -1;
            radarPos.z = distToObject * Mathf.Sin(deltay * Mathf.Deg2Rad);


            ro.image.transform.SetParent(this.transform);
            RectTransform rt = this.GetComponent<RectTransform>();
            ro.image.transform.position = new Vector3(radarPos.x + rt.pivot.x, radarPos.z + rt.pivot.y, 0) 
                       + this.transform.position;
    }
    }
}
