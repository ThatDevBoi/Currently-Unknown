using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Bottle : DB_Base_Class.AI_Crowed
{

    // Start is called before the first frame update
    void Start()
    {
        // Finding the relevant IDE Components from GameObjects in the scene
        objectThrow_Position = GameObject.FindGameObjectWithTag("ThrowPos").GetComponent<Transform>();
        Ref = GameObject.FindGameObjectWithTag("Referee").GetComponent<Transform>();
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        npc = GameObject.FindGameObjectWithTag("NPC_Fighter").GetComponent<Transform>();
        target = Random.Range(0, 4);    // Generate a number
    }

    // Update is called once per frame
    void Update()
    {
        // call Function
        PhysicsObject();
    }

    protected override void PhysicsObject()
    {
        // if the value is 2 and Ref didnt see elbow from the NPC
        if(target == 2 && DB_RefereeAI.NPC_Saw_Elbow == false)
        {
            // Make another number generate
            Random.Range(0, 4);
        }
        // if the value is 1 and the Ref knows the PC didnt do anything illegal
        if(target == 1 && DB_RefereeAI.PC_Saw_Elbow == false)
        {
            // Generate another number
            Random.Range(0, 4);
        }
        base.PhysicsObject();   // Call from base so we can move this gameObject
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy when hitting an object with these relevant triggers
        if(other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }

        if(other.gameObject.tag == "Referee")
        {
            Destroy(gameObject);
        }

        if(other.gameObject.tag == "NPC_Fighter")
        {
            Destroy(gameObject);
        }
    }
}