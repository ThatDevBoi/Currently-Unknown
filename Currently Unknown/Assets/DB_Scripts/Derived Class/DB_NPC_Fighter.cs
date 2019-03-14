using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_NPC_Fighter : DB_Base_Class
{
    // Value that subtracts from the players stamina 
    public int damageToGive = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Vector3 pushDirection = other.transform.position - transform.position;
            pushDirection = -pushDirection.normalized;
            GetComponent<Rigidbody>().AddForce(pushDirection * pushForce * 100);
            Debug.Log("Im Being Called");
        }
    }
}
