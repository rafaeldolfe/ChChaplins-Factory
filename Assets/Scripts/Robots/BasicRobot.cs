using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BasicRobot : MonoBehaviour
{
    protected GlobalEventManager gem;

    protected TurnedFixed turnFixed;
    public float speed = 10f;
    public float seeRadius = 10f;
    public float maxHealth = 30;
    protected float health;
    public GameObject target;
    public string tagTarget;
    protected Vector3 lastPosition = Vector3.zero;
    protected Quaternion lookRotation;
    public bool pickedUp = false;
    public GameObject hand;
    public BoxCollider boxCollider;

    public bool collidingWithRobot = false;

    protected void Awake()
    {
        List<Type> depTypes = ProgramUtils.GetMonoBehavioursOnType(this.GetType());
        List<MonoBehaviour> deps = new List<MonoBehaviour>
        {
            (gem = FindObjectOfType(typeof(GlobalEventManager)) as GlobalEventManager)
        };
        if (deps.Contains(null))
        {
            throw ProgramUtils.DependencyException(deps, depTypes);
        }
        health = maxHealth;
    }
    protected void Start()
    {
        gem.StartListening("Attack", Attacked);
        gem.StartListening("HooliganDie", LoseTarget);
        gem.StartListening("RobotBreak", Break);
        gem.StartListening("PickUp", PickedUp);
        gem.StartListening("PutDown", PutDown);
        turnFixed = GetComponent<TurnedFixed>();
        boxCollider = GetComponent<BoxCollider>();
    }
    protected void OnDestroy()
    {
        gem.StopListening("Attack", Attacked);
        gem.StopListening("HooliganDie", LoseTarget);
        gem.StopListening("RobotBreak", Break);
        gem.StopListening("PickUp", PickedUp);
        gem.StopListening("PutDown", PutDown);
    }

    private void PickedUp(GameObject robot, List<object> parameters)
    {
        if (gameObject != robot)
        {
            return;
        }
        if (parameters.Count == 0)
        {
            throw new Exception("Should have one parameter");
        }
        ResetValues();
        hand = (GameObject) parameters[0];
        pickedUp = true;
        boxCollider.enabled = false;
    }
    private void PutDown(GameObject robot, List<object> parameters)
    {
        if (gameObject != robot)
        {
            return;
        }
        if (parameters.Count > 0)
        {
            throw new Exception("Should have no parameters");
        }
        ResetValues();
        hand = null;
        pickedUp = false;
        boxCollider.enabled = true;
    }
    private void Break(GameObject hooligan, List<object> parameters)
    {
        ResetValues();
    }
    private void ResetValues()
    {
        StopShooting();
        target = null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Robot"))
        {
            Debug.Log("Collision Enter..." + other.transform.name);
            collidingWithRobot = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Robot"))
        {
            Debug.Log("Collision Exit..." + other.transform.name);
            collidingWithRobot = false;
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Robot"))
    //    {
    //        Debug.Log("Collision Enter..." + collision.transform.name);
    //        collidingWithRobot = true;
    //    }
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Robot"))
    //    {
    //        Debug.Log("Collision Exit..." + collision.transform.name);
    //        collidingWithRobot = false;
    //    }
    //}
    void Update()
    {
        if(pickedUp)
        {
            StopShooting();
            Transform tip = hand.transform;
            transform.position = tip.position;
        }
        else if(turnFixed.isFixed)
        {
            if(target)
            {
                PointAtTarget();
                ShootAtTarget();
            }
            else
            {
                StopShooting();
                FindTarget();
            }
        }
        else
        {
            StopShooting();
        }
    }
    private void LoseTarget(GameObject hooligan, List<object> parameters)
    {
        if (target != hooligan)
        {
            return;
        }
        target = null;
    }
    private void Attacked(GameObject target, List<object> parameters)
    {
        if (gameObject != target)
        {
            return;
        }
        if (parameters.Count == 0)
        {
            throw new Exception("No damage parameter for attack event.");
        }
        if (parameters.Count != 2)
        {
            throw new Exception("Inappropriate amount of parameters");
        }
        float damageTaken = (float)parameters[0];
        bool destroy = (bool)parameters[1];

        TakeDamage(damageTaken, destroy);
    }
    private void TakeDamage(float damageTaken, bool destroy)
    {
        health -= damageTaken;
        if (health < 0)
        {
            if (destroy)
            {
                Destroy(gameObject);
            }
            else
            {
                gem.TriggerEvent("RobotBreak", gameObject);
                health = maxHealth;
            }
        }
    }
    protected void PointAtTarget()
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
        objectsHit.Where(hit =>
        {
            float dist = Vector3.Distance(hit.transform.position, transform.position);
            return dist < 2f;
        }).ToArray();
        foreach (RaycastHit other in objectsHit)
        {
            if(other.collider.gameObject.CompareTag(tagTarget) && other.collider.gameObject.layer != 8)
            {
                target = other.collider.gameObject;
                break;
            }
        }
    }

    private void ShootAtTarget()
    {
        transform.GetChild(1).gameObject.SetActive(true);
    }

    protected virtual void StopShooting()
    {
        transform.GetChild(1).gameObject.SetActive(false);
        target = null;
    }
}
