using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sink : MonoBehaviour
{

    float whenToDestroy;
    void Start()
    {

        if(this.gameObject.tag=="Ragdoll")
        {
            Invoke("StartSink",5);
        }
        
    }


    public void StartSink()
    {
        whenToDestroy=Terrain.activeTerrain.SampleHeight(this.transform.position)- 10;
        Collider[] colList=this.transform.GetComponentsInChildren<Collider>();
        foreach(Collider c in colList)
        {
            Destroy(c);
        }
        InvokeRepeating("SinkToGround",Random.Range(6,10),0.1f);
        
        
    }
    void SinkToGround()
    {

        this.transform.Translate(0,-0.001f,0);
        if(whenToDestroy>=this.transform.position.y)
        {
            Destroy(this.gameObject);
        }
    }


}
