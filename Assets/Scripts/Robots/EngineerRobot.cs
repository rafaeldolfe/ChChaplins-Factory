using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class EngineerRobot : BasicRobot
{
    public float throwCooldown = 7.0f;
    public GameObject obstacle;

    private Coroutine throwBoards;
    void Update()
    {
        if (pickedUp)
        {
            StopThrowing();
            Transform tip = hand.transform;
            transform.position = tip.position;
        }
        else if (turnFixed.isFixed)
        {
            if (target)
            {
                PointAtTarget();
                if (throwBoards == null)
                {
                    throwBoards = StartCoroutine(ThrowBoardsAtTarget());
                }
            }
            else
            {
                StopThrowing();
                FindTarget();
            }
        }
        else
        {
            StopThrowing();
        }
    }
    private void FindTarget()
    {
        RaycastHit[] objectsHit = Physics.SphereCastAll(transform.position, seeRadius, Vector3.forward, 
            float.PositiveInfinity, 
            Physics.DefaultRaycastLayers, 
            QueryTriggerInteraction.Collide);
        List<Transform> transformArray = objectsHit.Select(hit => hit.transform).ToList();
        transformArray.Sort((hit1, hit2) =>
        {
            float dist1 = Vector3.Distance(hit1.transform.position, transform.position);
            float dist2 = Vector3.Distance(hit2.transform.position, transform.position);
            return dist1.CompareTo(dist2);
        });
        foreach (Transform transform in transformArray)
        {
            if (transform.gameObject.CompareTag(tagTarget))
            {
                Debug.Log("Found Path");
                target = transform.gameObject;
                break;
            }
        }
    }

    private IEnumerator ThrowBoardsAtTarget()
    {
        while(true)
        {
            yield return new WaitForSeconds(throwCooldown);
            GameObject boardClone = Instantiate(obstacle, transform.position + Vector3.up * 2, Quaternion.identity);
            Vector3 dist = target.transform.position - transform.position;
            boardClone.GetComponent<Rigidbody>().AddForce(dist * 75);
        }
    }

    private void StopThrowing()
    {
        if (throwBoards != null)
        {
            StopCoroutine(throwBoards);
        }
        throwBoards = null;
        target = null;
    }
    protected override void StopShooting()
    {
        target = null;
    }
}
