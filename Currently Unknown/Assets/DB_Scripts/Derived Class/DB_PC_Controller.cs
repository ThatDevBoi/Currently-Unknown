using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_PC_Controller : DB_Base_Class
{
    public Transform NPC_target;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void Melee_Combat()
    {
        base.Melee_Combat();
    }

    protected override void OnControllerColliderHit(ControllerColliderHit hit)
    {
        base.OnControllerColliderHit(hit);
    }
}
