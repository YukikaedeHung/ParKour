using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastScript : MonoBehaviour
{
    public LayerMask layerMask;
    public bool bObjInArea;

    public void FixedUpdate()
    {
        DrawRay();
        Raycast();
    }
    void DrawRay()
    {
        Vector3 direction = transform.TransformDirection(Vector3.up);
        float distance = 1000;
        //Debug.DrawRay(transform.position, (direction * distance), new Color(1, 0, 0)); //畫出線條並顯示
    }
    void Raycast()
    {
        Vector3 origin = transform.position;
        Vector3 direction = transform.TransformDirection(Vector3.up);

        if (Physics.Raycast(origin, direction))
        {
            bObjInArea = true;
        }
        else
        {
            bObjInArea = false;
        }
    }
}
