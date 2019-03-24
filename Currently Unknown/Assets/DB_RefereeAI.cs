using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_RefereeAI : DB_Base_Class.Referee
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Find the player fighter and hold its position in the world and store it in the Vector3
        vec_playerFighter = GameObject.FindGameObjectWithTag("Player").transform.position;
        // Find the NPC fighter so the Vector3 can hold its position in the world
        vec_NPCFighter = GameObject.FindGameObjectWithTag("NPC_Fighter").transform.position;
        // Call referee logic from base
        RefereeAI();
    }
}
