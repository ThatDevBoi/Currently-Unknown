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

    // Start is called before the first frame update
    protected override void Start()
    {
        // Find the player 
        opponent = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        base.Start();
        leftHand_SC.enabled = false;
        rightHand_SC.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(illegalElbow);
        Movement();
        Regenerate_Stamina();
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

        if(DB_RefereeAI.saw_Elbow == false)
        {
            if (close_to_hit)
            {
                time_Between_Attacks -= Time.deltaTime;
                if (time_Between_Attacks <= 0)
                {
                    // For clean Fighting 
                    int randomAttack = Random.Range(1, 3);
                    if (randomAttack == 1)
                    {
                        leftHand_SC.enabled = true;
                    }
                    else
                        leftHand_SC.enabled = false;
                    if (randomAttack == 2)
                    {
                        rightHand_SC.enabled = true;
                    }
                    else
                        rightHand_SC.enabled = false;
                    anim.SetInteger("Attack", randomAttack);
                    time_Between_Attacks = attacktimer;

                    if (currentStamina < 100)
                    {
                        // Clean and dirty Fighting
                        int randomAttacks = Random.Range(1, 4);
                        if (randomAttacks == 3)
                            illegalElbow = true;
                        else
                            illegalElbow = false;
                        anim.SetInteger("Attack", randomAttacks);
                        time_Between_Attacks = attacktimer;
                    }
                    else
                        return;
                }
            }
        }
    }

    protected override void Fighter_Stamina()
    {
        base.Fighter_Stamina();
    }

    protected override void Regenerate_Stamina()
    {
        base.Regenerate_Stamina();
    }

    protected override void Movement()
    {
        if(DB_RefereeAI.saw_Elbow == false)
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
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * 100);
            
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
            GetComponent<Rigidbody>().AddForce(-transform.forward * pushForce * 100);
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
