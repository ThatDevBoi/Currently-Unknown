using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_PC_Controller : DB_Base_Class
{
    public Transform NPC_target;

    public int damageToGive = 20;

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

    public void StaminaHurt(int staminaDamage, Vector3 direction)
    {
        currentStamina -= staminaDamage;
        KnockBack(direction);
    }
}
