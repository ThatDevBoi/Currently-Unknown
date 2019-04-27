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
    public bool stunned = false;
    public float stunTimer = 4f;
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
        // if this gameObject has not been ht with a bottle and the boolean hasnt ticked to true
        if(!stunned)
        {
            Regenerate_Stamina();   // Call regerate stamina Function
            if (DB_RefereeAI.NPC_Saw_Elbow == false & imDead == false)  // IF ref didnt see the illegal elbow and this GameObject has not got 0 HP
            {
                Movement(); // We can move
            }

            // if the pc isnt dead then he can fight
            if (!imDead && DB_RefereeAI.PC_Saw_Elbow == false)
                Melee_Combat(); // Call combat


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
            if (coreHealth <= 0)
            {
                KnockedOut();
            }
            // Always havce a reference for the current NPC Fighter Script
            DB_NPC_Fighter currentNPC = GameObject.FindGameObjectWithTag("NPC_Fighter").GetComponent<DB_NPC_Fighter>();
            if(currentNPC.knockedOut == true)
            {
                currentStamina = maxStamina;
                coreHealth = maxCore_Health;
            }
            
            base.Stamina_Montior();
            // For now this will work
            // This timer will be a monitor for our players hand colliders
            // we want it to tick down all the time because we dont want our players colliders to be punching the NPC in a none fight state
            turnOff_colliders -= Time.deltaTime;
            // So each time the monitor reacvhes 0 and beyond 
            if (turnOff_colliders <= 0)
            {
                // Just turn off the colliders
                leftHand_SC.enabled = false;
                rightHand_SC.enabled = false;
                elbow_SC.enabled = false;

                if (elbow_SC == null)
                    return;
                
            }
            // Turn off irrelevant colliders turn on relevant 1 when we press left mouse button
            if (Input.GetButton("Fire1"))
            {
                turnOff_colliders = 1;
                rightHand_SC.enabled = true;
                leftHand_SC.enabled = false;
                elbow_SC.enabled = false;
            }
            // Turn off irrelevant colliders turn on relevant 1 when we press right mouse button
            if (Input.GetButton("Fire2"))
            {
                turnOff_colliders = 1;
                elbow_SC.enabled = true;
                leftHand_SC.enabled = false;
                rightHand_SC.enabled = false;
                illegalElbow = true;
            }
            // Turn off irrelevant colliders turn on relevant 1 when we press space bar
            if (Input.GetButton("Jump"))
            {
                turnOff_colliders = 1;
                leftHand_SC.enabled = true;
                rightHand_SC.enabled = false;
                elbow_SC.enabled = false;
            }
        }
        else if(stunned)    // if the gameObject has been hit with a bottle and is stunned
        {
            anim.SetFloat("Speed", 0);      // We cant move
            stunTimer -= Time.deltaTime;        // Start a timer 
            if(stunTimer <= 0)          // When the timer is 0 or more
            {
                stunned = false;        // we are free to move again
                stunTimer = 4f;     // reset that timer
            }
        }
    }

    public void KnockedOut()
    {
        // When health is 0 or more
        if (coreHealth <= 0)
        {
            // Get the currentNPC fighter in the scene
            GameObject currentNPC = GameObject.FindGameObjectWithTag("NPC_Fighter");
            // Move that NPC to the middle of the ring
            currentNPC.transform.position = new Vector3(0, 0, 0);
            // Make sure that NPC is idle
            currentNPC.GetComponent<Animator>().SetFloat("Speed", 0);
            // Tick the imDead boolean to true
            imDead = true;
            // Activate im knocked out animation
            anim.SetBool("Knocked Out", true);
            // Remove the elbow collider
            elbow_SC = null;
        }
        else
            anim.SetBool("Knocked Out", false); // if not true and we have health then dont trigger the animation
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

    protected override void Regenerate_Stamina()
    {
        // if the stamina value of the fighter is less than 200
        if (currentStamina < 200)
        {
            // reset the timer
            increase_Stamina_timer = 5f;
            // Increase the current stamina value
            currentStamina += increase_Stamina;
        }
        // However if the current stamina is not less than the specific value
        else
        {
            //Reset the timer
            increase_Stamina_timer = 5f;
            // Return nothing else
            return;
        }
        base.Regenerate_Stamina();
    }

    private void OnTriggerEnter(Collider other)
    {
        // If we are hit by an object called NPC Hand
        if (other.gameObject.tag == "NPC_Hand")
        {
            anim.SetBool("Head_Hit", true); // Activate animation for head hit
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * pushBack);  // Push this gameObject back
            Fighter_Stamina();  // Take away stamina
        }
        // if hit by rHand tag
        if(other.gameObject.tag == "NPC_RHand")
        {
            anim.SetBool("Stomach_Hit", true);  // Stomach hit animation is true
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * pushBack);  // push gameObject back
            Fighter_Stamina();  // Take away stamina
        }

        if(other.gameObject.tag == "NPC_Elbow")     // if hit by tag elbow
        {
            anim.SetBool("Stomach_Hit", true);  // head hit true
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * pushBack);  // push gameObject back
            Fighter_Stamina();  // Take away stamina
        }

        if (other.gameObject.tag == "Bottle")   // if hit by the bottle GameObject 
            stunned = true;     // We are now stunned
    }

    private void OnTriggerExit(Collider other)
    {
        anim.SetBool("Head_Hit", false);
        anim.SetBool("Stomach_Hit", false);
    }
}
