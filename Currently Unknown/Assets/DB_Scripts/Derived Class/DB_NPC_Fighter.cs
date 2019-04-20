using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_NPC_Fighter : DB_Base_Class
{
    // Value that subtracts from the players stamina 
    //public int damageToGive = 5;
    public Transform opponent;
    public float Stopping_Distance;
    public bool beenHit = false;
    public float coolDown_fromhit = 1f;
    private bool close_to_hit = false;
    // A boolean that allows the AI referee to start checking for illegal moves
    public static bool illegalElbow = false;
    public float time_Between_Attacks;
    public float attacktimer = 1.5f;
    public float time_for_elbow;
    public int canElbow = 0;
    // stats variables
    public int fighterType;
    public Material[] visualMaterials;
    public GameObject bodymat;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Find the player 
        opponent = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        base.Start();
        // Turn off colliders attached to hands
        leftHand_SC.enabled = false;
        rightHand_SC.enabled = false;
        // Lets decide what fighter the player goes against
        TypeOfFighter();
    }

    // Update is called once per frame
    void Update()
    {
        base.Stamina_Montior();
        // NPC Dead
        // We have this so there is a continue state 
        if (coreHealth <= 0)
            anim.SetBool("KnockedOut", true);
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

                    //anim.SetInteger("Attack", randomAttacks);
                    time_Between_Attacks = attacktimer;
                }
            }
        }
        else
            return;
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
    private void TypeOfFighter()
    {
        fighterType = Random.Range(0, 7);
        // Use cases so there will be 6 fighters with different stats (in other words different values for speed,force etc)
        // We gain all control of player stats
        switch (fighterType)
        {
            // Prototype of how random fighter states will be generated
            case 1:
                print("Hello");
                // Change colour of fighter so there is a visual difference
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[0];
                break;
            case 2:
                print("Hello there");
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[1];
                break;
            case 3:
                print("Hello there my");
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[2];
                break;
            case 4:
                print("Hello there my friend");
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[3];
                break;
            case 5:
                print("Hello there my friend Joe");
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[4];
                break;
            case 6:
                print("Yo Joe You Want Blow");
                bodymat.GetComponent<SkinnedMeshRenderer>().material = visualMaterials[5];
                break;
        }
    }
    #endregion

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
}
