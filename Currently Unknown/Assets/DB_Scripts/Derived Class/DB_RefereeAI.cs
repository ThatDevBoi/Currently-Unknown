using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_RefereeAI : DB_Base_Class.Referee
{
    #region Variables
    public static bool NPC_Saw_Elbow = false;
    public static bool PC_Saw_Elbow = false;
    [SerializeField]
    private int percentage; // int that will randomly generate a number. We do this so the ref may or may not see an attack
    public float timer = .3f;
    [SerializeField]
    private Vector3 vec_player_start;
    [SerializeField]
    private Vector3 vec_NPC_start;
    [SerializeField]
    private Transform trans_players;
    [SerializeField]
    private Transform trans_npc;
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
        // Find the player fighter and hold its position in the world and store it in the Vector3
        vec_playerFighter = GameObject.FindGameObjectWithTag("Player").transform.position;
        // Find the NPC fighter so the Vector3 can hold its position in the world
        vec_NPCFighter = GameObject.FindGameObjectWithTag("NPC_Fighter").transform.position;

        transform.position = new Vector3(vec_NPCFighter.x, transform.position.y, transform.position.z);
        // Call referee logic from base
        RefereeMovement();
        // if a static boolean in DB_NPC_Fighter is true
        if(DB_NPC_Fighter.illegalElbow)
        {
            // Call void
            NPC_TakeAction();
        }

        if(DB_PC_Controller.illegalElbow)
        {
            PC_TakeAction();
        }
    }
    // Reset Referee state variables
    // These variables stop the referee from stopping the fight ever 1 second
    public float resetBool = 3f;
    public float resetRef_Timer = 1f;
    public bool ResetRef = false;
    #region NPC Version
    public void NPC_TakeAction()
    {
        // if the ref sees an illegal elbow
        if (NPC_Saw_Elbow)
            resetBool -= Time.deltaTime;    // Tick down the reset timer for the bool
        if(resetBool <= 0)  // When float is more than or equal to 0
        {
            NPC_Saw_Elbow = false;  // boolean is now false
            ResetRef = true;    // Next state for ref boolean is now true

            if(ResetRef)    // When the reset state boolean for ref is true
            {
                // We use the timer to gain more control over when the next state happens
                resetRef_Timer -= Time.deltaTime;   // Decrease ref timer
                
                if (resetRef_Timer <= 0)    // When float value is more or equal to 0
                {
                    percentage = 0;
                    // reset the timer value
                    resetBool = 3f; 
                    resetRef_Timer = 1f;
                    ResetRef = false;   // tick boolean back to false so ref can be back in orginal state
                }
            }
        }
        // Allowing the ref to decide if he sees the illegal attack by generating the percentage number randomly
        timer -= Time.deltaTime;    // Decrease timer
        if (timer <= 0)     // When timer is 0 or beyond
        {
            timer = 4f; // Reset timer
            if(!NPC_Saw_Elbow && !ResetRef) // if the relevant booleans are false
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
        // if the ref sees an illegal elbow
        if (PC_Saw_Elbow)
            resetBool -= Time.deltaTime;    // Tick down the reset timer for the bool
        if (resetBool <= 0)  // When float is more than or equal to 0
        {
            DB_PC_Controller.illegalElbow = false;
            PC_Saw_Elbow = false;  // boolean is now false
            ResetRef = true;    // Next state for ref boolean is now true

            if (ResetRef)    // When the reset state boolean for ref is true
            {
                // We use the timer to gain more control over when the next state happens
                resetRef_Timer -= Time.deltaTime;   // Decrease ref timer

                if (resetRef_Timer <= 0)    // When float value is more or equal to 0
                {
                    percentage = 0;
                    // reset the timer value
                    resetBool = 3f;
                    resetRef_Timer = 1f;
                    ResetRef = false;   // tick boolean back to false so ref can be back in orginal state
                }
            }
        }
        // Allowing the ref to decide if he sees the illegal attack by generating the percentage number randomly
        timer -= Time.deltaTime;    // Decrease timer
        if (timer <= 0)     // When timer is 0 or beyond
        {
            timer = 4f; // Reset timer
            if (!PC_Saw_Elbow && !ResetRef) // if the relevant booleans are false
            {
                percentage = Random.Range(0, 13);   // Decide a random number between 0 & 13
            }
        }

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
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bottle")
        {
            // Do Something
        }
    }
}
