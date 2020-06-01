
using UnityEngine;

public class TurretAnimationController : MonoBehaviour
{

    public Animator turretAnim;
    // Start is called before the first frame update
    void Start()
    {
        turretAnim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()

    {
        if(Input.GetKeyDown("q"))
        {
            turretAnim.Play("CINEMA_4D_Main");
        }

        if (Input.GetKeyDown("e"))
        {
            turretAnim.Play("Default");
        }
    }
}
