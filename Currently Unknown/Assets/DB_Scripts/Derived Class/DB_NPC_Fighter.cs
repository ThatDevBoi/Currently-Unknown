using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_NPC_Fighter : MonoBehaviour
{

    public int damageToGive = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Vector3 hitDirection = other.transform.position - transform.position;
            hitDirection = hitDirection.normalized;
            Debug.Log("Player is Here");
            // To see where the player will land
            Debug.Log(other.transform.position - transform.position);
            other.gameObject.GetComponent<DB_PC_Controller>().StaminaHurt(damageToGive, hitDirection);
        }
    }
}
