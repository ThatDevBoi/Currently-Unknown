using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class DB_Camera_Movement : DB_Base_Class.Camera_Movement
{
    void Start()
    {
        cam = GetComponent<Camera>();
        targets[0] = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        targets[1] = GameObject.FindGameObjectWithTag("Referee").GetComponent<Transform>();
    }

    protected override void LateUpdate()
    {
        // We update this because there will be more than just one instance of the NPC and the NPC will be lost countless times
        // The NPC dies and respawns as a new opponent so if we place this in start() we wont find it again
        targets[2] = GameObject.FindGameObjectWithTag("NPC_Fighter").GetComponent<Transform>();
        base.LateUpdate();
    }

    protected override Vector3 GetCenterPoint()
    {
        return base.GetCenterPoint();
    }
}
