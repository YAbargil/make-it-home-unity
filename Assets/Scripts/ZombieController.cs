using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{

    Animator m_anim;
    public int Health;
    [SerializeField] float m_Damage=10f;
    [SerializeField] GameObject m_target;
    [SerializeField] public GameObject m_Ragdoll;
    NavMeshAgent m_agent;
    float walkingSpeed,runningSpeed;

    enum STATE { IDLE,WANDER,CHASE,ATTACK,DEAD}
    STATE currentState=STATE.IDLE;
    void Start()
    {
        m_anim=this.GetComponent<Animator>();
        m_agent=this.GetComponent<NavMeshAgent>();
        walkingSpeed=m_agent.speed;   
        runningSpeed=walkingSpeed*1.3f; 
        //m_anim.SetBool("walk",true);
    }

    //change "Hips" to Zombie:Hips on zombieman and change tag.
    public void DamagePlayer() 
    {
        if(m_target!=null)
            m_target.GetComponent<PlayerController>().WoundPlayer(m_Damage);

    }

    public bool isDead() 
    {
        Health--;
        if (Health<=0)
            return true;
        return false;
    }
    public void PlayAnim(string s)
    {
        m_anim.SetBool("walk",false);
        m_anim.SetBool("attack",false);
        m_anim.SetBool("running",false);
        m_anim.SetBool("death",false);
        m_anim.SetBool(s,true);
    }

    public void ZombieKilled() 
    {
        PlayAnim("death");
        currentState=STATE.DEAD;

    }
    float DistanceToPlayer() {
        if(GameStats.GameOver==true)
        {
            return Mathf.Infinity;
        }
         return Vector3.Distance(this.transform.position,m_target.transform.position);
         }

    bool PlayerInSight() 
    {
        if(DistanceToPlayer() <10 )
            return true;
        return false;
    }

    void Update()
    {

        if(m_target==null && !GameStats.GameOver)
        {
            m_target=GameObject.FindWithTag("Player");
            return ;
        }
        switch(currentState){

            case STATE.IDLE:
            if(PlayerInSight())
                currentState=STATE.CHASE;
            else
                currentState=STATE.WANDER;

            break;
            case STATE.WANDER:
            if(!m_agent.hasPath) 
            {

                float wanderX=Random.Range(-5,5)+this.transform.position.x;
                float wanderZ=Random.Range(-5,5)+this.transform.position.z;
                float wanderY=Terrain.activeTerrain.SampleHeight(
                                                    new Vector3(wanderX,0,wanderZ));
                Vector3 wanderDest=new Vector3 (wanderX,wanderY,wanderZ);
                m_agent.SetDestination(wanderDest);
                m_agent.stoppingDistance=0;
                m_agent.speed=walkingSpeed;
                PlayAnim("walk");
            }
            if(PlayerInSight())
                currentState=STATE.CHASE;

            break;
            case STATE.CHASE:
            if(GameStats.GameOver==true)
            {
                PlayAnim("walk");
                currentState=STATE.WANDER;
                return;
            }
            m_agent.SetDestination(m_target.transform.position);
            m_agent.stoppingDistance=2.2f;
            PlayAnim("running");
            m_agent.speed=runningSpeed;
            if(m_agent.remainingDistance<=m_agent.stoppingDistance && !m_agent.pathPending)
                currentState=STATE.ATTACK;
            
            if(DistanceToPlayer() > 20)
            {
                currentState=STATE.WANDER;
                m_agent.ResetPath();
            }
            break;
            case STATE.ATTACK:
             if(GameStats.GameOver==true)
            {
                PlayAnim("walk");
                currentState=STATE.WANDER;
                return;
            }

            PlayAnim("attack");
            this.transform.LookAt(m_target.transform.position);
            if(DistanceToPlayer() > m_agent.stoppingDistance + 2)
                currentState=STATE.CHASE;
            break;
            case STATE.DEAD:
            Destroy(m_agent);
            this.GetComponent<Sink>().StartSink();

            break;


        }




        /*
        m_agent.SetDestination(m_target.transform.position);
        if(m_agent.remainingDistance>m_agent.stoppingDistance)
        {
             m_anim.SetBool("walk",true);
             m_anim.SetBool("attack",false);

        }
        else
        {
            m_anim.SetBool("walk",false);
            m_anim.SetBool("attack",true);

        }
        /*
        if(Input.GetKey(KeyCode.W))
        {
            m_anim.SetBool("walk",true);
        }
        else
        {
            m_anim.SetBool("walk",false);
        }
        if(Input.GetKey(KeyCode.Z))
        {
            m_anim.SetBool("death",true);
        }
        else
        {
            m_anim.SetBool("death",false);
        }
        if(Input.GetKey(KeyCode.X))
        {
            m_anim.SetBool("attack",true);
        }
        else
        {
            m_anim.SetBool("attack",false);
        }
        if(Input.GetKey(KeyCode.R))
        {
            m_anim.SetBool("running",true);
        }
        else
        {
           m_anim.SetBool("running",false);
        } 
        */             
    }
}
