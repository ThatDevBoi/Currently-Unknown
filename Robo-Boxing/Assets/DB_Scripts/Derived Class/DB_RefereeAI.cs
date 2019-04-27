using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_RefereeAI : DB_Base_Class.Referee
{
    #region Variables
    // Monitoring NPC and PC positions
    private Vector3 vec_player_start;
    private Vector3 vec_NPC_start;
    private Transform trans_players;
    private Transform trans_npc;

    // Stunned Variables 
    private bool imStunned = false;     // Boolean which makes the gameObject stop doing anything for however long the stunTimer equals
    public float stunTimer = 8f;    // This needs to be public just to edit the time for better gameplay.

    // Decision Variable
    private int percentage; // int that will randomly generate a number. We do this so the ref may or may not see an attack

    // Reset Referee state variables
    // These variables stop the referee from stopping the fight ever 1 second
    //NPC Variables
    private float NPCtimer = 4;
    private float NPCresetBool = 3f;
    private float NPCresetRef_Timer = 1f;
    private bool NPCResetRef = false;
    public static bool NPC_Saw_Elbow = false;
    // PC Variables
    private float PCtimer = 4;
    private float PCresetBool = 3f;
    private float PCresetRef_Timer = 1f;
    private bool PCResetRef = false;
    public static bool PC_Saw_Elbow = false;
    public bool FindnewNPC = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Find the IDE transform components
        trans_players = GameObject.Find("Player_Fighter").GetComponent<Transform>();
        trans_npc = GameObject.FindGameObjectWithTag("NPC_Fighter").GetComponent<Transform>();
        // Record the 2 fighters first ever position on the first frame
        // allow the Vector to be called once so we gain the players start pos and NPC
        vec_player_start = trans_players.position; 
        vec_NPC_start = trans_npc.position;     
    }

    // Update is called once per frame
    void Update()
    {
        if (trans_npc == null)  // if there is no reference
        {
            FindnewNPC = true;  // find the new NPC that just spawned boolean is true
            if (FindnewNPC)     // When boolean is true
            {
                // Lets find that new NPC 
                trans_npc = GameObject.FindGameObjectWithTag("NPC_Fighter").GetComponent<Transform>();
            }
        }
        else   // However if the Reference is not null
            FindnewNPC = false; // Tick boolean to false

        if (!imStunned)
        {
            // Find the player fighter and hold its position in the world and store it in the Vector3
            vec_playerFighter = GameObject.FindGameObjectWithTag("Player").transform.position;
            // Find the NPC fighter so the Vector3 can hold its position in the world
            vec_NPCFighter = GameObject.FindGameObjectWithTag("NPC_Fighter").transform.position;

            transform.position = new Vector3(vec_NPCFighter.x, transform.position.y, transform.position.z);
            // Call referee logic from base
            RefereeMovement();
            // if a static boolean in DB_NPC_Fighter is true
            if (DB_NPC_Fighter.illegalElbow)
            {
                // Call void
                NPC_TakeAction();
            }

            if (DB_PC_Controller.illegalElbow)
            {
                PC_TakeAction();
            }
        }
        else if(imStunned)
        {
            gameObject.transform.position = new Vector3(-3, .5f, 0);
            NPC_Saw_Elbow = false;
            PC_Saw_Elbow = false;
            percentage = 0;
            // reset the timer value
            NPCresetBool = 3f;
            NPCresetRef_Timer = 1f;
            NPCResetRef = false;   // tick boolean back to false so ref can be back in orginal state

            PCresetBool = 3f;
            PCresetRef_Timer = 1f;
            PCResetRef = false;
            stunTimer -= Time.deltaTime;
            if(stunTimer <= 0)
            {
                imStunned = false;
                stunTimer = 8;
            }
        }
    }

    #region NPC Version
    public void NPC_TakeAction()
    {
        // if the ref sees an illegal elbow
        if (NPC_Saw_Elbow)
            NPCresetBool -= Time.deltaTime;    // Tick down the reset timer for the bool
        if(NPCresetBool <= 0)  // When float is more than or equal to 0
        {
            NPC_Saw_Elbow = false;  // boolean is now false
            NPCResetRef = true;    // Next state for ref boolean is now true

            if(NPCResetRef)    // When the reset state boolean for ref is true
            {
                // We use the timer to gain more control over when the next state happens
                NPCresetRef_Timer -= Time.deltaTime;   // Decrease ref timer
                
                if (NPCresetRef_Timer <= 0)    // When float value is more or equal to 0
                {
                    percentage = 0;
                    // reset the timer value
                    NPCresetBool = 3f; 
                    NPCresetRef_Timer = 1f;
                    NPCResetRef = false;   // tick boolean back to false so ref can be back in orginal state
                }
            }
        }
        // Allowing the ref to decide if he sees the illegal attack by generating the percentage number randomly
        NPCtimer -= Time.deltaTime;    // Decrease timer
        if (NPCtimer <= 0)     // When timer is 0 or beyond
        {
            NPCtimer = 4f; // Reset timer
            if(!NPC_Saw_Elbow && !NPCResetRef) // if the relevant booleans are false
            {
                percentage = Random.Range(0, 13);   // Decide a random number between 0 & 13
            }
        }

        Debug.Log(NPC_Saw_Elbow);
        if (percentage > 6) // if random percentage value is greater than 6 so 7, 8, 9, 10, 11 and 12
        {
            NPC_Saw_Elbow = true;   // Boolean is true the ref saw those sketchy elbow attacks
        }
        else     // if its not over 6
           NPC_Saw_Elbow = false;   // ref didnt see it, he dont care
        
        // When the ref sees that sketchy elbow
        if (NPC_Saw_Elbow)
        {
            // Ref stands between both fighters
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
            // Ref pushes fighters into there corners
            trans_players.position = vec_player_start;   
            trans_npc.position = vec_NPC_start;
        }
    }
#endregion

    #region PC Version
    public void PC_TakeAction()
    {
        PCtimer -= Time.deltaTime;    // Decrease timer

        if (PCtimer <= 0)     // When timer is 0 or beyond
        {
            PCtimer = 4f; // Reset timer
            if (!PC_Saw_Elbow && !PCResetRef) // if the relevant booleans are false
            {
                percentage = Random.Range(0, 13);   // Decide a random number between 0 & 13
            }
        }
        // Allowing the ref to decide if he sees the illegal attack by generating the percentage number randomly
        if (percentage > 6) // if random percentage value is greater than 6 so 7, 8, 9, 10, 11 and 12
        {
            PC_Saw_Elbow = true;   // Boolean is true the ref saw those sketchy elbow attacks
        }
        else     // if its not over 6
            PC_Saw_Elbow = false;   // ref didnt see it, he dont care

        // When the ref sees that sketchy elbow
        if (PC_Saw_Elbow)
        {
            // Ref stands between both fighters
            transform.position = new Vector3(0, transform.position.y, transform.position.z);
            // Ref pushes fighters into there corners
            trans_players.position = vec_player_start;
            trans_npc.position = vec_NPC_start;
        }

        // if the ref sees an illegal elbow
        if (PC_Saw_Elbow)
            PCresetBool -= Time.deltaTime;    // Tick down the reset timer for the bool
        if (PCresetBool <= 0)  // When float is more than or equal to 0
        {
            PC_Saw_Elbow = false;  // boolean is now false
            PCResetRef = true;    // Next state for ref boolean is now true

            if (PCResetRef)    // When the reset state boolean for ref is true
            {
                // We use the timer to gain more control over when the next state happens
                PCresetRef_Timer -= Time.deltaTime;   // Decrease ref timer

                if (PCresetRef_Timer <= 0)    // When float value is more or equal to 0
                {
                    percentage = 0;
                    // reset the timer value
                    PCresetBool = 3f;
                    PCresetRef_Timer = 1f;
                    PCtimer = 4f;
                    PCResetRef = false;   // tick boolean back to false so ref can be back in orginal state
                    DB_PC_Controller.illegalElbow = false;
                }
            }
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        // Object which will stun the Ref so he cant move or monitor
        if (other.gameObject.tag == "Bottle")
            imStunned = true;
    }
}
