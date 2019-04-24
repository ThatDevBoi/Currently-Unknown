using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DB_NPC_Fighter : DB_Base_Class
{
    // Find the player
    // Need is so this gameObject knows where its going 
    private Transform opponent;
    // We set a value to allow the gameObject to stop so its not always pushing the player back
    public float Stopping_Distance;
    // Boolean that triggers when the gameObject it hit. 
    // Used for when hit gameObject cannot move
    private bool beenHit = false;
    // This value will tick down so the beenHit boolean can tick back to false 
    // Its just a timer to say how long this gamneObject is dazed 
    public float coolDown_fromhit = 1f;
    // Boolean which tells the gameObject they can hit the PC when in range
    public bool close_to_hit = false;
    // A boolean that allows the AI referee to start checking for illegal moves
    public static bool illegalElbow = false;
    // Bool which makes the NPC fighter stop doing anything so theyre stunned
    // Being stunned is triggered by another object
    public bool imStunned = false;
    // Timer which ticks down as its the amount of time the fighter can move fight or do anything
    public float stunTimer = 4f;
    // Timer which tells the NPC we can attack again when at 0
    public float time_Between_Attacks;
    // This is what the value will be between attacks
    public float attacktimer = 1.5f;
    public float time_for_elbow;
    public int canElbow = 0;    // Int which changes to allow the NPC choose between fighting techniques
    // stats variables
    // Value which will be set when the game starts and a function is called
    // It chooses the type of fighter
    private int fighterType;
    // List of materials that change the gameObjects colour depending when the fighter changes
    public Material[] visualMaterials;
    // The GameObject which changes colour. GameObject which has the skinned mesh renderer component
    public GameObject bodymat;
    // Reset fighter been knocked out variables 
    public bool knockedOut = false;
    // UI
    public Slider staminaSlider, healthSlider;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Find the player 
        opponent = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        staminaSlider = GameObject.FindGameObjectWithTag("NPC_SliderS").GetComponent<Slider>();
        healthSlider = GameObject.FindGameObjectWithTag("NPC_SliderH").GetComponent<Slider>();
        // Lets decide what fighter the player goes against
        // Run this before base. If not the new values never get registered as base start tells max to be a value we want to change
        TypeOfFighter();
        base.Start();
        healthSlider.gameObject.SetActive(false);   // Turn off Healthbar we have found it so now we dont need it We need it when stamina is 0
        // Turn off colliders attached to hands
        leftHand_SC.enabled = false;
        rightHand_SC.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If the gameObject hasnt been hit with a bottle and the boolean is false
        if(!imStunned)
        {
            // if we have no more core health
            if (coreHealth <= 0)
                knockedOut = true;  // this gameObject is offcially knocked out onto the floor

            if (knockedOut) // If our fighter has been knocked out (Health = 0)
            {
                head_Collider.enabled = false;
                body_Collider.enabled = false;
                GM GMscript = GameObject.Find("GM").GetComponent<GM>(); // Find the GameManager with out local variable
                StartCoroutine(GMscript.KnockedOut());    // Call from the GM lets get a new opponent
            }

            staminaSlider.value = currentStamina;   // current stamina value of the fighter will be shown on the stamina bar slider value
            healthSlider.value = coreHealth;     // core health value of the fighter will be shown on the stamina bar slider value
            if (currentStamina <= 0)
            {
                // Turn on the health bar we have no stamina
                healthSlider.gameObject.SetActive(true);
                // we have no stamina so turn that stamina bar off
                staminaSlider.gameObject.SetActive(false);
            }
            else if (currentStamina > 0)
            {
                // Turn off Health Bar. We have some stamina
                healthSlider.gameObject.SetActive(false);
                // Turn on stamina bar. We need to show the stamina we have
                staminaSlider.gameObject.SetActive(true);
            }
            // If the ref isnt pulling both fighters aside for an illegal move then we can move
            if (DB_RefereeAI.NPC_Saw_Elbow == false)
            {
                Movement(); // We can move
            }
            Regenerate_Stamina();   // Update stamina Regen so the stamina value regenerates over time 

            if (!knockedOut)    // if the fighter hasnt been knocked out
            {
                if (!close_to_hit)  // and if the fighter isnt close enough to hit the player
                {
                    // Turn off colliders
                    leftHand_SC.enabled = false;
                    rightHand_SC.enabled = false;
                    elbow_SC.enabled = false;
                    // All animations for fighting are off
                    // Be weird if the NPC was swinging at air
                    anim.SetBool("Jabbing", false);
                    anim.SetBool("Body Punch", false);
                    anim.SetBool("Elbow", false);
                }
            }
            else return;

            if (DB_RefereeAI.NPC_Saw_Elbow == false)
            {
                // When we are close enough to hit the PC
                if (close_to_hit)
                {
                    // Fighter throws standard jabs and body punches
                    CleanFighting();
                    // Stamina needs to meet a value for this to be called
                    Dirty_Fighting();
                }
                else    // If not and the booleans arent valid 
                    close_to_hit = false;   // then close to hit is false we cant call these functions
            }
            if (DB_RefereeAI.NPC_Saw_Elbow == false)
            {
                if (canElbow == 1)
                {
                    time_Between_Attacks = attacktimer; // Reset clean attack timer
                    time_for_elbow -= Time.deltaTime;
                    if (time_for_elbow <= 0)
                    {
                        time_for_elbow = 6;
                        // Clean and dirty Fighting
                        int randomAttacks = Random.Range(1, 27);

                        // Jabbing
                        if (randomAttacks > 1)
                        {
                            anim.SetBool("Jabbing", true);
                            anim.SetBool("Body Punch", false);
                            leftHand_SC.enabled = true;
                            rightHand_SC.enabled = false;
                        }
                        else
                            leftHand_SC.enabled = false;

                        // Body Punch
                        if (randomAttacks > 10)
                        {
                            anim.SetBool("Body Punch", true);
                            anim.SetBool("Jabbing", false);
                            rightHand_SC.enabled = true;
                            leftHand_SC.enabled = false;
                        }
                        else
                            rightHand_SC.enabled = false;

                        // illegal Elbow
                        if (randomAttacks > 21)
                        {
                            anim.SetBool("Elbow", true);
                            anim.SetBool("Body Punch", false);
                            anim.SetBool("Jabbing", false);
                            illegalElbow = true;
                        }
                        else
                        {
                            anim.SetBool("Elbow", false);
                            illegalElbow = false;
                        }
                        // Reset the value 
                        if (illegalElbow)
                            randomAttacks = 0;

                        //anim.SetInteger("Attack", randomAttacks);
                        time_Between_Attacks = attacktimer;
                    }
                }
            }
            else
                return;
        }
        else if(imStunned)  //  When the gameObject is stunned
        {
            anim.SetFloat("Speed", 0);  // We make our gameObject Idle
            stunTimer -= Time.deltaTime;    // Tick down the timer so the player slowlys starts to not be stunned
            if(stunTimer <= 0)      // When the timer is 0 or greater
            {
                stunTimer = 4;      // Reset the timer value
                imStunned = false;  // Reset the boolean
            }
        }
    }

    public void CleanFighting()
    {
        if(canElbow == 0)   // When the int is 0
        {
            time_Between_Attacks -= Time.deltaTime; // Tick down the timer
            if (time_Between_Attacks <= 0)      // When the timer is 0
            {
                // For clean Fighting 
                int randomAttack = Random.Range(1, 21);     // Generate a random value
                // jabbing
                if (randomAttack > 1)   // When the value is greater than 1
                {
                    anim.SetBool("Jabbing", true);  // We can Jab
                    anim.SetBool("Body Punch", false);  // We can body punch
                    leftHand_SC.enabled = true;     // Turn on relevant collider on the hand
                    rightHand_SC.enabled = false;   // Turn off irrelevant collider
                }
                else        // If the value is not greater than 1
                    leftHand_SC.enabled = false;    // Turn off the collider for the left hand

                if (randomAttack > 10)      // If the value is greater than 10
                {
                    anim.SetBool("Body Punch", true);   // We can body punch
                    anim.SetBool("Jabbing", false);     // We cannot Jab
                    rightHand_SC.enabled = true;        // We turn on relevant collider
                    leftHand_SC.enabled = false;        // We turn off irelevant collider
                }
                else        // If the value isnt over 10
                    rightHand_SC.enabled = false;   // Turn off the collider;
                time_Between_Attacks = attacktimer; // Reset the timer
            }
        }
    }

    public void Dirty_Fighting()
    {
        // When the current stamina is less than 100
        if (currentStamina < 100)
        {
            canElbow = 1;   // Change the int value so we can fight dirty
        }
        else        // If it aint under 100
            canElbow = 0;   // Turn the int value back to 0 so we only clean fight
    }
    #region Base Override Functions
    protected override void Regenerate_Stamina()
    {
        base.Regenerate_Stamina();
    }

    protected override void Fighter_Stamina()
    {
        base.Fighter_Stamina();
    }
    #endregion

    #region Movement Functions
    protected override void Movement()
    {
        // Make local variable for player script
        DB_PC_Controller playerScript = opponent.GetComponent<DB_PC_Controller>();
        if(playerScript.imDead == false)
        {
            if (!knockedOut)
            {
                if (DB_RefereeAI.NPC_Saw_Elbow == false)
                {
                    // when the value is 1 or more
                    if (coolDown_fromhit <= 1)
                    {
                        // When the boolean in this class is false
                        if (!beenHit)
                        {
                            // when the transform of this gameObject is still grater than the stopping point value
                            if (Vector3.Distance(transform.position, opponent.position) > Stopping_Distance)
                            {
                                close_to_hit = false;
                                // Let there be speed
                                speed = 1;
                                // Allow the moveDirection to be forward 
                                moveDirection = (transform.position += transform.forward * speed * Time.deltaTime);
                                anim.SetFloat("Speed", speed);
                                // Make sure the moveDirection value isnt more than 1 
                                moveDirection = moveDirection.normalized * speed;
                                // zero out the gravity we dont need it
                                moveDirection.y = moveDirection.y + Physics.gravity.y;
                                // Move that GameObject towards its target using the moveDirection
                                playerRB.velocity = moveDirection;
                            }
                            else if (Vector3.Distance(transform.position, opponent.position) < Stopping_Distance)
                            {
                                // Pick a number between 1 and 3
                                close_to_hit = true;
                                //anim.SetBool("Jabbing", true);
                            }
                        }
                        // when the boolean strikes true
                        else if (beenHit)
                        {
                            // Start the cooldown timer
                            coolDown_fromhit -= Time.deltaTime;
                            // when the cooldown is greater or is 0
                            if (coolDown_fromhit <= 0)
                            {
                                //anim.SetBool("Jabbing", false);
                                // revert boolean back to orginal state
                                beenHit = false;
                                // reset the value of cooldown
                                coolDown_fromhit = 1;
                            }
                            // turn off that speed
                            speed = 0;
                        }
                    }
                    // Make sure the AI moves with the opponets X values
                    transform.position = new Vector3(opponent.position.x, transform.position.y, transform.position.z);
                    // Adding some of the physics from the base into the AI
                    base.Movement();
                }
            }
        }
       
    }
    #endregion

    #region Choose Fighter
    public void TypeOfFighter()
    {
        fighterType = Random.Range(1, 7);
        Debug.Log(fighterType);
        // Use cases so there will be 6 fighters with different stats (in other words different values for speed,force etc)
        // We gain all control of player stats
        switch (fighterType)
        {
            // Prototype of how random fighter states will be generated
            // The Heavy Hitter     // Hard
            case 1:
                gameObject.name = "The Heavy Hitter";
                knockedOut = false;
                Stopping_Distance = .7f;
                pushBack = 100;
                pushForce = 20;
                maxStamina = 100;
                currentStamina = maxStamina;
                staminaSlider.maxValue = currentStamina;
                staminaSlider.value = currentStamina;
                maxCore_Health = 250;
                coreHealth = maxCore_Health;
                healthSlider.maxValue = coreHealth;
                healthSlider.value = coreHealth;
                increase_Stamina = 4;
                increase_Stamina_timer = 6;
                attacktimer = 5;
                // Change colour of fighter so there is a visual difference
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[0];
                break;
            // Feather weight   // Easy
            case 2:
                gameObject.name = "The Feather weight";
                knockedOut = false;
                Stopping_Distance = .7f;
                pushBack = 140;
                pushForce = 30;
                maxStamina = 250;
                currentStamina = maxStamina;
                staminaSlider.maxValue = currentStamina;
                staminaSlider.value = currentStamina;
                maxCore_Health = 50;
                coreHealth = maxCore_Health;
                healthSlider.maxValue = coreHealth;
                healthSlider.value = coreHealth;
                increase_Stamina = 20;
                increase_Stamina_timer = 20;
                attacktimer = 4;
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[1];
                break;
            // The Under-dog    // Meduim
            case 3:
                gameObject.name = "The Under-dog";
                knockedOut = false;
                Stopping_Distance = .7f;
                maxStamina = 120;
                pushBack = 100;
                pushForce = 20;;
                currentStamina = maxStamina;
                staminaSlider.maxValue = currentStamina;
                staminaSlider.value = currentStamina;
                maxCore_Health = 160;
                coreHealth = maxCore_Health;
                healthSlider.maxValue = coreHealth;
                healthSlider.value = coreHealth;
                increase_Stamina = 5;
                increase_Stamina_timer = 6;
                attacktimer = 2;
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[2];
                break;
            // The Urban Champion   // Very Hard
            case 4:
                gameObject.name = "The Urban Champion";
                knockedOut = false;
                Stopping_Distance = .7f;
                maxStamina = 300;
                pushBack = 80;
                pushForce = 30;
                maxStamina = 500;
                currentStamina = maxStamina;
                staminaSlider.maxValue = currentStamina;
                staminaSlider.value = currentStamina;
                maxCore_Health = 60;
                coreHealth = maxCore_Health;
                healthSlider.maxValue = coreHealth;
                healthSlider.value = coreHealth;
                increase_Stamina = 30;
                increase_Stamina_timer = 20;
                attacktimer = 2.5f;
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[3];
                break;
            // Heavy Weight Champion    // Hard
            case 5:
                gameObject.name = "The Heavy Weight Champion";
                knockedOut = false;
                Stopping_Distance = .7f;
                pushBack = 140;
                pushForce = 30;
                maxStamina = 400;
                currentStamina = maxStamina;
                staminaSlider.maxValue = currentStamina;
                staminaSlider.value = currentStamina;
                maxCore_Health = 100;
                coreHealth = maxCore_Health;
                healthSlider.maxValue = coreHealth;
                healthSlider.value = coreHealth;
                increase_Stamina = 10;
                increase_Stamina_timer = 12;
                attacktimer = 1;
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[4];
                break;
            // The Charity Match    Meduim
            case 6:
                gameObject.name = "The Charity Match";
                knockedOut = false;
                Stopping_Distance = .7f;
                pushBack = 100;
                pushForce = 50;
                maxStamina = 100;
                currentStamina = maxStamina;
                staminaSlider.maxValue = currentStamina;
                staminaSlider.value = currentStamina;
                maxCore_Health = 140;
                coreHealth = maxCore_Health;
                healthSlider.maxValue = coreHealth;
                healthSlider.value = coreHealth;
                increase_Stamina = 20;
                increase_Stamina_timer = 14;
                attacktimer = 2;
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[5];
                break;
        }
    }
    #endregion

    #region On Trigger Enter/Exit
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PC_Hand")
        {
            damage_Stamina = 5;
            Fighter_Stamina();
            beenHit = true;
            #region Removed
            // Removed for it didnt work the way i would have wanted it to
            //Vector3 pushDirection = other.transform.position - transform.position;
            //pushDirection = -pushDirection.normalized;
            #endregion
            anim.SetBool("HeadHit", true);
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * pushBack);
            
        }

        if(other.gameObject.tag == "PC_RHand")
        {
            // Change damage value for body punch
            damage_Stamina = 10;
            Fighter_Stamina();
            beenHit = true;
            anim.SetBool("BodyHit", true);
            // Could increase this value later?
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * pushBack);
        }

        if(other.gameObject.tag == "PC_Elbow")
        {
            // Change damage value for body punch
            damage_Stamina = 20;
            Fighter_Stamina();
            beenHit = true;
            anim.SetBool("HeadHit", true);
            // Could increase this value later?
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * pushBack);
        }

        if (other.gameObject.tag == "Bottle")
            imStunned = true;
    }
    public void OnTriggerExit(Collider other)
    {
        // Reset damage stamina value to default
        //damage_Stamina = 5;
        anim.SetBool("HeadHit", false);
        anim.SetBool("BodyHit", false);
    }
    #endregion
}
