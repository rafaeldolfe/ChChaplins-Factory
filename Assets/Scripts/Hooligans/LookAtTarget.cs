using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform hips;
    private HooligansV2 hooligansV2;
    private HooliganDying dying;
    // Start is called before the first frame update
    void Start()
    {
        hooligansV2 = GetComponent<HooligansV2>();
        dying = GetComponent<HooliganDying>();
    }

    // Update is called once per frame
    void Update()
    {
        if(dying.isAlive && hooligansV2.attackTarget)
        {
            RotateTowards();
        }
    }

    private void RotateTowards()
    {
        Debug.Log("rotating");
        Quaternion lookRotation = Quaternion.LookRotation(hooligansV2.attackTarget.transform.position - transform.position);

        hips.eulerAngles = hooligansV2.attackTarget.transform.position - transform.position;
        
        // Quaternion.Slerp(transform.rotation,
        //                                       lookRotation,
        //                                       .1f);
    }
}
