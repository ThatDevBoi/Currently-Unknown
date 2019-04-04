using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_NPC_Fighter : DB_Base_Class
{
    // Value that subtracts from the players stamina 
    public int damageToGive = 5;
    public Transform opponent;
    public float Stopping_Distance;
    public bool beenHit = false;
    public float coolDown_fromhit = 1f;

    // Start is called before the first frame update
    protected override void Start()
    {
        // Find the player 
        opponent = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
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
    }

    protected override void Movement()
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
                    anim.SetBool("Jabbing", true);
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

        base.Movement();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PC_Hand")
        {
            currentStamina -= damageToGive;
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
            currentStamina -= damageToGive;
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
        anim.SetBool("HeadHit", false);
        anim.SetBool("BodyHit", false);
    }
}
