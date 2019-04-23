using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DB_PC_Controller : DB_Base_Class
{
    // Hand colliders monitor
    public float turnOff_colliders = 1f;
    public bool imDead = false;
    public static bool illegalElbow = false;
    // UI
    public Slider pcHealth, pcStamina;
    // Start is called before the first frame update
    protected override void Start()
    {
        // Find UI Sliders For health and stamina feedback
        pcHealth = GameObject.FindGameObjectWithTag("PC_SliderH").GetComponent<Slider>();
        pcStamina = GameObject.FindGameObjectWithTag("PC_SliderS").GetComponent<Slider>();
        base.Start();
        // Just turn off colliders on start
        leftHand_SC.enabled = false;
        rightHand_SC.enabled = false;
        pcStamina.maxValue = currentStamina;
        pcHealth.maxValue = coreHealth;
        pcHealth.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (DB_RefereeAI.NPC_Saw_Elbow == false & imDead == false)
        {
            Movement();
        }

        // if the pc isnt dead then he can fight
        if (imDead == false)
            Melee_Combat();
        // Set the stamina and health values to there slider health bars
        pcStamina.value = currentStamina;
        pcHealth.value = coreHealth;
        // when there is no stamina 
        if (currentStamina <= 0)
        {
            pcHealth.gameObject.SetActive(true); // we need the players real health to turn on
            pcStamina.gameObject.SetActive(false);
        }
        else if (currentStamina > 0)    // however if the stamina has some value that is over 0
        {
            pcHealth.gameObject.SetActive(false);   // Turn the real health back off
            pcStamina.gameObject.SetActive(true);
        }
            
        // Functions to be called
        Regenerate_Stamina();
        if(coreHealth <= 0)
        {
            KnockedOut();
        }
        base.Stamina_Montior();
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
            elbow_SC.enabled = false;
        }

        if (Input.GetButton("Fire1"))
        {
            turnOff_colliders = 1;
            rightHand_SC.enabled = true;
            leftHand_SC.enabled = false;
            elbow_SC.enabled = false;
        }

        if (Input.GetButton("Fire2"))
        {
            turnOff_colliders = 1;
            elbow_SC.enabled = true;
            leftHand_SC.enabled = false;
            rightHand_SC.enabled = false;
            illegalElbow = true;
        }

        if (Input.GetButton("Jump"))
        {
            turnOff_colliders = 1;
            leftHand_SC.enabled = true;
            rightHand_SC.enabled = false;
            elbow_SC.enabled = false;
        }
    }

    public void KnockedOut()
    {
        if (coreHealth <= 0)
        {
            GameObject currentNPC = GameObject.FindGameObjectWithTag("NPC_Fighter");
            currentNPC.transform.position = new Vector3(0, 0, 0);
            currentNPC.GetComponent<Animator>().SetFloat("Speed", 0);
            imDead = true;
            anim.SetBool("Knocked Out", true);
            elbow_SC = null;
        }
        else
            anim.SetBool("Knocked Out", false);
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
            anim.SetBool("Head_Hit", true);
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * pushBack);
            Fighter_Stamina();
        }

        if(other.gameObject.tag == "NPC_RHand")
        {
            anim.SetBool("Stomach_Hit", true);
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * pushBack);
            Fighter_Stamina();
        }

        if(other.gameObject.tag == "NPC_Elbow")
        {
            anim.SetBool("Stomach_Hit", true);
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * pushBack);
            Fighter_Stamina();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        anim.SetBool("Head_Hit", false);
        anim.SetBool("Stomach_Hit", false);
    }
}
