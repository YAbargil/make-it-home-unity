using UnityEngine;
using UnityEngine.UI;


public class PlayerController : MonoBehaviour

{
    [SerializeField] AudioSource[] m_Footsteps;
    [SerializeField] GameObject m_characterPrefab;
    [SerializeField] AudioSource m_emptyAmmo;
    [SerializeField] AudioSource[] m_Wounds;
    [SerializeField] Transform m_bulletDirection;
    [SerializeField] AudioSource m_Jump;
    [SerializeField] AudioSource m_Reload;
    [SerializeField] AudioSource m_Die;
    [SerializeField] AudioSource m_Land;
    [SerializeField] AudioSource m_Ammo,m_Medkit;
    [SerializeField] GameObject m_Cam;
    [SerializeField] Animator m_Animator;
    [SerializeField] GameObject m_Blood;
    [SerializeField] GameObject m_Splatter;
    [SerializeField] GameObject m_Canvas;
    [SerializeField] Slider m_healthBar;
    [SerializeField] Text m_TextAmmo;
    [SerializeField] Text m_TextMagazine;
    [SerializeField] GameObject m_GameOverText;
    [SerializeField] GameObject m_Crosshair;

    float canvasWidth,canvasHeight;
    float m_speed=0.1f;
    float m_sensitivity = 3f;
    Rigidbody m_rb;
    CapsuleCollider m_capsule;
    Quaternion m_CamRotation;
    Quaternion m_PlayerRotation;
    bool cursorIsLocked = true;
    bool lockCursor= true; 
    float x,z;
    int ammo=0,maxAmmo=50,ammoSize=15;
    int health=100,maxHealth=100,healthSize=25;
    int m_magazine=0,m_maxMagazine=10;
    static int sound=0;

     void Start()
     {
        canvasWidth=m_Canvas.GetComponent<RectTransform>().rect.width;
        canvasHeight=m_Canvas.GetComponent<RectTransform>().rect.height;
        m_rb=this.GetComponent<Rigidbody>();
        m_capsule=this.GetComponent<CapsuleCollider>();
        m_CamRotation=m_Cam.transform.localRotation;
        m_PlayerRotation=this.transform.localRotation;
        health=maxHealth;
        m_Crosshair.SetActive(false);
        m_healthBar.value=health;
        m_TextAmmo.text=ammo + "";
        m_TextMagazine.text=m_magazine+"";
     }

       void PlayWounds() 
    {
        AudioSource audioSource = new AudioSource();
        int n = Random.Range(1, m_Wounds.Length);
        audioSource = m_Wounds[n];
        audioSource.Play();
        m_Wounds[n] = m_Wounds[0];
        m_Wounds[0] = audioSource;
    }

    public void WoundPlayer(float amount)
    {
        health=(int)Mathf.Clamp(health-amount,0,maxHealth);
        m_healthBar.value=health;
        
        //Debug.Log("Health :" +health);
        if(health<=0)
        {
            m_Die.Play();
            Vector3 characterPos= new Vector3(this.transform.position.x,Terrain.activeTerrain.SampleHeight(this.transform.position),this.transform.position.z);
            GameObject m_Character= Instantiate(m_characterPrefab,characterPos,this.transform.rotation);
            m_Character.GetComponent<Animator>().SetTrigger("Death");
            GameStats.GameOver=true;
            Destroy(this.gameObject);
        }

        else    
            PlayWounds();
            GameObject bloodSplatter=Instantiate(m_Splatter);
            bloodSplatter.transform.SetParent(m_Canvas.transform);
            bloodSplatter.transform.position= new Vector3 (Random.Range(0,canvasWidth),Random.Range(0,canvasHeight),0);
            Destroy(bloodSplatter,3.0f);
            
            
    }

    void ShootZombie() 
    {
        
        RaycastHit hitInfo;

        if(Physics.Raycast(m_bulletDirection.position,m_bulletDirection.forward,out hitInfo,600))
        {
            if(hitInfo.collider.gameObject.tag=="Zombie")
            {
                GameObject blood=Instantiate(m_Blood,hitInfo.point,Quaternion.identity);
                blood.transform.LookAt(this.transform.position);
                Destroy(blood,0.7f);
                GameObject Zombie=hitInfo.collider.gameObject;
                if(Zombie.GetComponent<ZombieController>().isDead()) 
                {
                if(Random.Range(0,10) < 5) 
                {
                GameObject ragdoll_prefab=Zombie.GetComponent<ZombieController>().m_Ragdoll;
                GameObject new_ragdoll=Instantiate(ragdoll_prefab,Zombie.transform.position,Zombie.transform.rotation);
                new_ragdoll.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(m_bulletDirection.forward*10000);
                Destroy(Zombie);
                }

                else 
                {
                    Zombie.GetComponent<ZombieController>().ZombieKilled();
                }
                }

            }
            else if(hitInfo.collider.gameObject.tag=="Zombie1")
            {
                GameObject blood=Instantiate(m_Blood,hitInfo.point,Quaternion.identity);
                blood.transform.LookAt(this.transform.position);
                Destroy(blood,0.7f);
                GameObject Zombie=hitInfo.collider.gameObject;
                if(Zombie.GetComponent<ZombieController>().isDead())
                {
                if(Random.Range(0,10) < 5) 
                {
                GameObject ragdoll_prefab=Zombie.GetComponent<ZombieController>().m_Ragdoll;
                GameObject new_ragdoll=Instantiate(ragdoll_prefab,Zombie.transform.position,Zombie.transform.rotation);
                new_ragdoll.transform.Find("Zombie:Hips").GetComponent<Rigidbody>().AddForce(m_bulletDirection.forward*10000);
                Destroy(Zombie);
                }

                else 
                {
                    Zombie.GetComponent<ZombieController>().ZombieKilled();
                }
                }
            }

        }


    }


    
    void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.tag=="FinishSpot")
        {
            Vector3 pos= new Vector3(this.transform.position.x,Terrain.activeTerrain.SampleHeight(this.transform.position),this.transform.position.z);
            GameObject Character = Instantiate(m_characterPrefab,pos,this.transform.rotation);
            GameObject gameOver = Instantiate(m_GameOverText);
            gameOver.transform.SetParent(m_Canvas.transform);
            gameOver.transform.localPosition=Vector3.zero;
            Character.GetComponent<Animator>().SetTrigger("Dance");
            GameStats.GameOver=true;
            DontDestroyOnLoad(m_Canvas);
            Destroy(this.gameObject);

        }    
    }
    void Update()
     {
        x =Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        if(Input.GetKeyDown(KeyCode.F)) 
        {
            m_Animator.SetBool("arm",!m_Animator.GetBool("arm"));
            m_Crosshair.SetActive(!m_Crosshair.activeSelf);
        }
        
        if ((Mathf.Abs(x) >0 || Mathf.Abs(z) >0 ) && !m_Animator.GetBool("walk") )
        {
            m_Animator.SetBool("walk",true);
            //InvokeRepeating("PlayFootsteps",0.3f,1.1f);
        }
        else if (m_Animator.GetBool("walk") && !(Mathf.Abs(x) >0 || Mathf.Abs(z) >0 ))
        {
            m_Animator.SetBool("walk",false);
            //CancelInvoke("PlayFootsteps");
            //CancelInvoke("LoopFootsteps");
         }
        if(Input.GetKeyDown(KeyCode.R) && m_magazine<m_maxMagazine && ammo>0 && m_Animator.GetBool("arm"))
     {
            m_Animator.SetTrigger("reload");
            m_Reload.Play();
            if(m_magazine==0)
            {
                m_magazine= ammo>=m_maxMagazine?m_maxMagazine:ammo;
                m_TextMagazine.text=m_magazine+"";

                ammo-=m_magazine;
                m_TextAmmo.text=ammo + "";
                Debug.Log("Magazine :" +m_magazine);
                Debug.Log("Ammo:" + ammo);
            }
            else
            {
                int ammoNeeded=m_maxMagazine-m_magazine;
                m_magazine=ammo>=ammoNeeded?m_maxMagazine:m_magazine+ammo;
                m_TextMagazine.text=m_magazine+"";
                ammo=Mathf.Clamp(ammo-ammoNeeded,0,ammo);
                m_TextAmmo.text=ammo + "";
                Debug.Log("Magazine :" +m_magazine);
                Debug.Log("Ammo:" + ammo);
            }
     }
        if(Input.GetMouseButtonDown(0)  &&m_Animator.GetBool("arm") && GameStats.Shoot) // && !m_Animator.GetBool("fire")
            {
            // m_Shot.Play();
            if(m_magazine>0){
                m_Animator.SetTrigger("fire");
                ShootZombie();
                m_magazine--;
                m_TextMagazine.text=m_magazine+"";
                Debug.Log("Magazine:" +m_magazine);
                GameStats.Shoot=false;
            }
            else {
                m_emptyAmmo.Play();


            }
            }
         if(Input.GetKeyDown(KeyCode.Space) && isGrounded()) {
            m_Jump.Play();
            m_rb.AddForce(0,270,0);
            if(m_Animator.GetBool("walk"))
            {
              //CancelInvoke("PlayFootsteps");
            }
        }
        // else
        //     m_Animator.SetBool("fire",false);       
        
     }

/*
     void LoopFootsteps()
    {
        foreach(AudioSource a in m_Footsteps)
        {
            if(a.isPlaying==true)
                return;
        }
        sound=+Random.Range(1,10);
        m_Footsteps[sound%m_Footsteps.Length].Play();
        //AudioSource audioSource = new AudioSource();
        //audioSource=m_Footsteps[sound%m_Footsteps.Length];
        //audioSource.Play();
    }

    void stopFootsteps()
    {
        foreach(AudioSource  a in m_Footsteps)
        {
            a.Stop();
        }



    }
    void PlayFootsteps()
    {
        AudioSource audioSource = new AudioSource();
        int n = Random.Range(1, m_Footsteps.Length);
        audioSource = m_Footsteps[n];
        audioSource.Play();
        m_Footsteps[n] = m_Footsteps[0];
        m_Footsteps[0] = audioSource;
    }
*/
    void FixedUpdate() 
    {
        float yRotation= Input.GetAxis("Mouse X");
        float xRotation=Input.GetAxis("Mouse Y");
        m_CamRotation*=Quaternion.Euler(-xRotation*m_sensitivity,0,0);
        m_PlayerRotation*=Quaternion.Euler(0,yRotation*m_sensitivity,0);
        m_Cam.transform.localRotation=m_CamRotation;
        this.transform.localRotation=m_PlayerRotation;
       
        x =Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        transform.position += m_Cam.transform.forward * z *m_speed + m_Cam.transform.right * x*m_speed; //new Vector3(x * speed, 0, z * speed);
        UpdateCursorLock();

    }



    void OnCollisionEnter(Collision other) {

        


        if(other.gameObject.tag=="Med" && health<maxHealth)
        {
            health=Mathf.Clamp(health+healthSize,0,100);
            m_healthBar.value=health;
            Debug.Log("health:"+ health );
            m_Medkit.Play();
            Destroy(other.gameObject);
        }
        else if(other.gameObject.tag=="Ammo" && ammo<maxAmmo)
        {
            ammo=Mathf.Clamp(ammo+ammoSize,0,maxAmmo);
            m_TextAmmo.text=ammo + "";
            Debug.Log("Ammo" + ammo);
            m_Ammo.Play();

            Destroy(other.gameObject);
        }
        else if(isGrounded())
        {
            //m_Land.Play();    
            if(m_Animator.GetBool("walk"))
               InvokeRepeating("PlayFootsteps",0,0.7f);
        }
    }

    public void setCursorLock(bool b ) 
    {
        lockCursor = b;
        if(!lockCursor)
        {
            Cursor.lockState=CursorLockMode.None;
            Cursor.visible=true;
        }
    }

    public void UpdateCursorLock()
    {
        if(lockCursor)
             InternalLockUpdate();
    }

    public void  InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            cursorIsLocked = false;
        else if ( Input.GetMouseButtonUp(0) )
            cursorIsLocked = true;

        if (cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    bool isGrounded()
    {
        RaycastHit m_hitInfo;
        if(Physics.SphereCast(transform.position
                             ,m_capsule.radius,
                             new Vector3 (0,-1,0),
                              out m_hitInfo,
                              (m_capsule.height/2f)-m_capsule.radius + 0.1f))
        {
            return true;
        }
        return false;



    }
}
