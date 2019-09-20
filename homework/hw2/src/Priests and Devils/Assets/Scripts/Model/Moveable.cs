using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour
{

    readonly float speed = 20;

    int status;  // 0->not moving, 1->moving to middle, 2->moving to dest
    Vector3 dest;
    Vector3 middle;

    void Update()
    {
        if (status == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, middle, speed * Time.deltaTime);
            if (transform.position == middle)
            {
                status = 2;
            }
        }
        else if (status == 2)
        {
            transform.position = Vector3.MoveTowards(transform.position, dest, speed * Time.deltaTime);
            if (transform.position == dest)
            {
                status = 0;
            }
        }
    }
    public void SetDestination(Vector3 _dest)
    {
        dest = _dest;
        middle = _dest;
        if (_dest.y == transform.position.y)
        {  // 船移动中
            status = 2;
        }
        else if (_dest.y < transform.position.y)
        {  
            middle.y = transform.position.y;
        }
        else
        {                            
            middle.x = transform.position.x;
        }
        status = 1;
    }

    public void Reset()
    {
        status = 0;
    }
}