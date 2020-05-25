using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingTowards : MonoBehaviour
{
    private TurnedFixed turnFixed;
    public float speed = 10f;
    public float seeRadius = 10f;
    public GameObject target;
    public string tagTarget;
    private Vector3 lastPosition = Vector3.zero;
    private Quaternion lookRotation;
    public GameObject shootingPrefab;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(target)
        {
            PointAtTarget();
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
            FindTarget();
        }
    }

    private bool SetTarget(GameObject newTarget)
    {
        if(!newTarget)
        {
            return false;
        }
        else
        {
            target = newTarget;
            return true;
        }
    }

    private void PointAtTarget()
    {
        float distance = (transform.position - target.transform.position).magnitude;
        if(distance < seeRadius)
        {
            if(lastPosition != target.gameObject.transform.position)
            {
                lastPosition = target.gameObject.transform.position;
                lookRotation = Quaternion.LookRotation(lastPosition - transform.position);
            }
            if(lookRotation != transform.rotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                              lookRotation,
                                                              speed * Time.deltaTime);
            }
        }
        else
        {
            target = null;
        }
    }

    private void FindTarget()
    {
        RaycastHit[] objectsHit = Physics.SphereCastAll(transform.position,
                                                        seeRadius,
                                                        Vector3.forward);
        foreach(RaycastHit other in objectsHit)
        {
            Debug.Log(other.collider.gameObject);
            if(other.collider.gameObject.tag == tagTarget)
            {
                Debug.Log("found target");
                target = other.collider.gameObject;
                break;
            }
        }
    }
}
