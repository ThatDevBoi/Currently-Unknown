using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool close_to_hit = false;
    // A boolean that allows the AI referee to start checking for illegal moves
    public static bool illegalElbow = false;
    // Timer which tells the NPC we can attack again when at 0
    public float time_Between_Attacks;
    // This is what the value will be between attacks
    public float attacktimer = 1.5f;
    public float time_for_elbow;
    public int canElbow = 0;
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

    // Start is called before the first frame update
    protected override void Start()
    {
        // Find the player 
        opponent = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        // Lets decide what fighter the player goes against
        // Run this before base. If not the new values never get registered as base start tells max to be a value we want to change
        TypeOfFighter();
        base.Start();
        // Turn off colliders attached to hands
        leftHand_SC.enabled = false;
        rightHand_SC.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (coreHealth <= 0)
            knockedOut = true;

        if (knockedOut)
            KnockedOut();

        base.Stamina_Montior();
        Debug.Log(illegalElbow);
        if(DB_RefereeAI.NPC_Saw_Elbow == false)
        {
            Movement();
        }
        Regenerate_Stamina();
        Stamina_Montior();
        #region placeholder movement (Invalid now)
        // PlaceHolder for how movement will work 
        //if (Vector3.Distance(transform.position, opponent.position) > Stopping_Distance)
        //{
        //    speed = 1;
        //    transform.LookAt(opponent.position);
        //    transform.Translate(0, 0, speed * Time.deltaTime);
        //}
        //else
        //{
        //    speed = 0;
        //}
        #endregion

        if(!close_to_hit)
        {
            leftHand_SC.enabled = false;
            rightHand_SC.enabled = false;
        }

        if(DB_RefereeAI.NPC_Saw_Elbow == false)
        {
            if (close_to_hit)
            {
                // Fighter throws standard jabs and body punches
                CleanFighting();
                // Stamina needs to meet a value for this to be called
                Dirty_Fighting(); 
            }
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

    public IEnumerator KnockedOut()
    {
        // Make the animation play so the NPC is knocked out

        // We need to wait a few seconds 

        // We need to respawn the NPC Fighter via GM

        // We need to reset boolean so its not updating all the time goes back to being dormant

        yield break;    // End of logic
    }

    protected override void Fighter_Stamina()
    {
        base.Fighter_Stamina();
    }

    public void CleanFighting()
    {
        if(canElbow == 0)
        {
            time_Between_Attacks -= Time.deltaTime;
            if (time_Between_Attacks <= 0)
            {
                // For clean Fighting 
                int randomAttack = Random.Range(1, 21);
                // jabbing
                if (randomAttack > 1)
                {
                    anim.SetBool("Jabbing", true);
                    anim.SetBool("Body Punch", false);
                    leftHand_SC.enabled = true;
                    rightHand_SC.enabled = false;
                }
                else
                    leftHand_SC.enabled = false;

                if (randomAttack > 10)
                {
                    anim.SetBool("Body Punch", true);
                    anim.SetBool("Jabbing", false);
                    rightHand_SC.enabled = true;
                    leftHand_SC.enabled = false;
                }
                else
                    rightHand_SC.enabled = false;

                //anim.SetInteger("Attack", randomAttack);
                time_Between_Attacks = attacktimer;

            }
        }
    }

    public void Dirty_Fighting()
    {
        if (currentStamina < 100)
        {
            canElbow = 1;
        }
        else
            canElbow = 0;
    }

    protected override void Regenerate_Stamina()
    {
        base.Regenerate_Stamina();
    }

    protected override void Stamina_Montior()
    {
        base.Stamina_Montior();
    }

    protected override void Movement()
    {
        if(DB_RefereeAI.NPC_Saw_Elbow == false)
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

    #region Choose Fighter
    public void TypeOfFighter()
    {
        fighterType = Random.Range(0, 2);
        // Use cases so there will be 6 fighters with different stats (in other words different values for speed,force etc)
        // We gain all control of player stats
        switch (fighterType)
        {
            // Prototype of how random fighter states will be generated
            // The Heavy Hitter     // Hard
            case 1:
                print("The Heavy Hitter");
                speed = 2;
                pushBack = 80;
                pushForce = 20;
                maxStamina = 100;
                maxCore_Health = 500;
                damage_Stamina = 10;
                increase_Stamina = 20;
                increase_Stamina_timer = 6;
                // Change colour of fighter so there is a visual difference
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[0];
                break;
            // Feather weight   // Easy
            case 2:
                print("Feather weight");
                speed = 4;
                pushBack = 140;
                pushForce = 30;
                maxStamina = 300;
                maxCore_Health = 500;
                damage_Stamina = 5;
                increase_Stamina = 30;
                increase_Stamina_timer = 8;
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[1];
                break;
            // The Under-dog    // Meduim
            case 3:
                print("The Under-dog");
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[2];
                break;
            // The Urban Champion   // Meduim
            case 4:
                print("The Urban Champion");
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[3];
                break;
            // Heavy Weight Champion    // Hardest
            case 5:
                print("The Heavy Weight Champion");
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[4];
                break;
            // The Charity Match
            case 6:
                print("The Charity Match");
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
            #region Removed
            // Removed for it didnt work the way i would have wanted it to
            //Vector3 pushDirection = other.transform.position - transform.position;
            //pushDirection = -pushDirection.normalized;
            #endregion
            anim.SetBool("BodyHit", true);
            // Could increase this value later?
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * pushBack);
        }
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
