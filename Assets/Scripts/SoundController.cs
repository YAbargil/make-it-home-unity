using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] public AudioSource m_Shot;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire() 
    {
        m_Shot.Play();
    }

    public void CanShoot()
    {
        GameStats.Shoot=true;
    }
}
