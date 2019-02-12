using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_PC_Controller : DB_Base_Class
{
    public Transform NPC_target;
    public float rotaionalSpeed = 10;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        // Keep PC within a distance Cant get to close the NPC
        Debug.DrawLine(NPC_target.position, transform.position, Color.yellow);  // Draws line between the Player and the NPC opponent
        if(canMove)
        {
            if (Vector3.Distance(transform.position, NPC_target.position) < 2)
            {
                canMove = false;
                transform.position = Vector3.MoveTowards(transform.position, NPC_target.position, Speed * Time.deltaTime);
            }
        }
        
        base.Update();
    }
}
