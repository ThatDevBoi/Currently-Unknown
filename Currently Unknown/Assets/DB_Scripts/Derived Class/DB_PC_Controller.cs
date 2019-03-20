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



    public void StaminaHurt(int hurt, Vector3 direction)
    {
        currentStamina -= hurt;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "NPC_Fighter")
        {
            Vector3 pushDirection = other.transform.position - transform.position;
            pushDirection = -pushDirection.normalized;
            GetComponent<Rigidbody>().AddForce(pushDirection * pushForce * 100);
            Debug.Log("Im Being Called");
        }
    }
}
