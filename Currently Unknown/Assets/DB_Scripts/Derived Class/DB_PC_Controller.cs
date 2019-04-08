using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_PC_Controller : DB_Base_Class
{
    public Transform NPC_target;
    // Hand colliders monitor
    public float turnOff_colliders = 1f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // Just turn off colliders on start
        leftHand_SC.enabled = false;
        rightHand_SC.enabled = false;
    }

    private void FixedUpdate()
    {
        Movement();
        Melee_Combat();
        Regenerate_Stamina();
        // For now this will work
        // This timer will be a monitor for our players hand colliders
        // we want it to tick down all the time because we dont want our players colliders to be punching the NPC in a none fight state
        turnOff_colliders -= Time.deltaTime;
        // So each time the monitor reacvhes 0 and beyond 
        if(turnOff_colliders <= 0)
        {
            // Just turn off the colliders
            leftHand_SC.enabled = false;
            rightHand_SC.enabled = false;
        }

        if (Input.GetButton("Fire1"))
        {
            turnOff_colliders = 1;
            leftHand_SC.enabled = false;
            rightHand_SC.enabled = true;
        }
        
        if (Input.GetButton("Jump"))
        {
            turnOff_colliders = 1;
            rightHand_SC.enabled = false;
            leftHand_SC.enabled = true;
        }
        

    }

    protected override void Movement()
    {
        // Animations being detected
        anim.SetFloat("Speed", Vertical * speed);      // Value in script of speed is now the animation float value of speed
        // Allow for animator parameter to use side step so blend tree knows when to change animation left = -1 | idle = 0 | right = 1
        anim.SetFloat("Side Speed", Horizontal * speed);

        // if the A, D, Left Arrow or Right Arrow are pushed down well a Animator Parameter boolean sets to true
        if (Input.GetButtonDown("Strafe"))
        {
            // The gameObjects animator in dervied classes finds boolean parameter and sets it true
            anim.SetBool("SideStrife", true);
        }
        // However any of the A, D, Left Arrowor Right Arrow are no longer held down
        else if (Input.GetButtonUp("Strafe"))
        {
            // parameter animator boolean is not true and new animation plays depending on what you set next
            anim.SetBool("SideStrife", false);
        }
        // Call the movement Logic in Base class
        base.Movement();
    }

    protected override void Fighter_Stamina()
    {
        base.Fighter_Stamina();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NPC_Hand")
        {
            //Vector3 pushDirection = other.transform.position - transform.position;
            //pushDirection = -pushDirection.normalized;
            anim.SetBool("Head_Hit", true);
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * 100);
            Fighter_Stamina();
            Debug.Log("Im Being Called");
        }

        if(other.gameObject.tag == "NPC_RHand")
        {
            anim.SetBool("Stomach_Hit", true);
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * 100);
            Fighter_Stamina();
            Debug.Log("Im Being Called");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        anim.SetBool("Head_Hit", false);
        anim.SetBool("Stomach_Hit", false);
    }
}
