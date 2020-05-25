using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBounds : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < -2)
        {
            transform.position.Set(0,
                                   1,
                                   0);
        }
        else if (transform.position.y > 2)
        {
            transform.position.Set(0,
                                   1,
                                   0);
        }
    }
}
