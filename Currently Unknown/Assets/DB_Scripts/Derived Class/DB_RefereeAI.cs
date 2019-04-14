using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DB_RefereeAI : DB_Base_Class.Referee
{
    public static bool saw_Elbow = false;
    [SerializeField]
    private int percentage;
    private Vector3 newPosition = Vector3.zero;
    public float timer = .3f;
    // Start is called before the first frame update
    void Start()
    {
        // Record the 2 fighters first ever position on the first frame
        Vector3 PC_start_pos = vec_playerFighter;
        Vector3 NPC_startPos = vec_NPCFighter;
    }

    // Update is called once per frame
    void Update()
    {
        // Find the player fighter and hold its position in the world and store it in the Vector3
        vec_playerFighter = GameObject.FindGameObjectWithTag("Player").transform.position;
        // Find the NPC fighter so the Vector3 can hold its position in the world
        vec_NPCFighter = GameObject.FindGameObjectWithTag("NPC_Fighter").transform.position;

        transform.position = new Vector3(vec_NPCFighter.x, transform.position.y, transform.position.z);
        // Call referee logic from base
        RefereeMovement();

        if(DB_NPC_Fighter.illegalElbow == true)
        {
            TakeAction();
        }
    }

    public void TakeAction()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = .3f;
            percentage = Random.Range(0, 13);
        }

        Debug.Log(percentage);
        if (percentage > 6)
        {
            saw_Elbow = true;
        }
        else
           saw_Elbow = false;
        

        if (saw_Elbow)
        {
            transform.position = new Vector3(0.05f, transform.position.y, transform.position.z);
            float time = 3;
            time -= Time.deltaTime;
            if(time <= 0)
            {
                //saw_Elbow = false;
                time = 3;
            }
        }
    }
}
