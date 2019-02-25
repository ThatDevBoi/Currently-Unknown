using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class DB_Camera_Movement : DB_Base_Class
{
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected override Vector3 GetCenterPoint()
    {
        return base.GetCenterPoint();
    }
}
