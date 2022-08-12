using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawn : MonoBehaviour
{
    public int NumberOfZombies= 10;
    [SerializeField] GameObject ZombiePrefab;
    float m_Radius;
    public bool SpawnOnStart=true;



    // Start is called before the first frame update
    void Start()
    {
        if(SpawnOnStart)
           
            SpawnZombies();

        }
    
    
    void SpawnZombies()
    {
         for (int i = 0; i < NumberOfZombies; i++)
            {
                Vector3 random=this.transform.position +( Random.insideUnitSphere * m_Radius);
                NavMeshHit hit;

                if ( NavMesh.SamplePosition(random,out hit,10.0f,NavMesh.AllAreas))
                {
                    Instantiate(ZombiePrefab,hit.position,Quaternion.identity);
                }
                else
                    i--;
    }
    
    }


    void OnTriggerEnter(Collider other) 
    {
        if(!SpawnOnStart && other.gameObject.tag=="Player")
            SpawnZombies();    
    }

}

    


