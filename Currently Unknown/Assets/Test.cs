using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : DB_Base_Class
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PC_Hand")
        {
            Debug.Log("BeenHit");
            Vector3 pushDirection = other.transform.position - transform.position;
            pushDirection = -pushDirection.normalized;
            GetComponent<Rigidbody>().AddForce(pushDirection * pushForce * 100);
            Debug.Log("Im Being Called");
        }
    }
}
