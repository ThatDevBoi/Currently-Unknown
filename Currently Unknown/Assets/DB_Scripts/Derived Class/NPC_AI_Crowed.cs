using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_AI_Crowed : DB_Base_Class.AI_Crowed
{

    // Start is called before the first frame update
    void Start()
    {
        AttackTimer = time_between_throws;
        objectThrow_Position = GameObject.Find("ThrowPosition").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        base.ActivateTheThrow();
    }
}
