using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class EngineerRobotBackup : MonoBehaviour
{
    private GlobalEventManager gem;

    private TurnedFixed turnFixed;
    public float speed = 10f;
    public float seeRadius = 10f;
    public float health = 30;
    public float throwCooldown = 3.0f;
    public GameObject target;
    public string tagTarget;
    private Vector3 lastPosition = Vector3.zero;
    private Quaternion lookRotation;
    public bool pickedUp = false;
    public GameObject hand;
    public BoxCollider boxCollider;
    public GameObject obstacle;


    void Awake()
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
    }
    void Start()
    {
        gem.StartListening("Attack", Attacked);
        turnFixed = GetComponent<TurnedFixed>();
        boxCollider = GetComponent<BoxCollider>();
    }
    private void OnDestroy()
    {
        gem.StopListening("Attack", Attacked);
    }
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
        float damageTaken = (float)parameters[0];

        TakeDamage(damageTaken);
    }
    private void TakeDamage(float damageTaken)
    {
        health -= damageTaken;
        if (health < 0)
        {
            Destroy(gameObject);
        }
    }

    private void PointAtTarget()
    {
        float distance = (transform.position - target.transform.position).magnitude;
        if (distance < seeRadius)
        {
            if (lastPosition != target.gameObject.transform.position)
            {
                lastPosition = target.gameObject.transform.position;
                lookRotation = Quaternion.LookRotation(lastPosition - transform.position);
            }
            if (lookRotation != transform.rotation)
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
        RaycastHit[] objectsHit = Physics.SphereCastAll(transform.position, seeRadius, Vector3.forward, 
            float.PositiveInfinity, 
            Physics.DefaultRaycastLayers, 
            QueryTriggerInteraction.Collide);
        //objectsHit.Sort((hit1, hit2) => {
        //    Vector3 distance = Vector3.Distance(hit1.transform.position, hit2.transform.position);
        //    return 
        //})
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
            //foreach (Transform transform in boardClone.transform)
            //{
            //    Rigidbody rigidbody = transform.GetComponent<Rigidbody>();
            //    if (rigidbody != null)
            //    {
            //        rigidbody.AddForce((transform.position + transform.forward * 2) - target.transform.position);
            //    }
            //}
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
}
