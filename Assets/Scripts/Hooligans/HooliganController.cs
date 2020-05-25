using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class HooliganController : MonoBehaviour
{
    private GlobalEventManager gem;

    public float speed;
    public float attackCooldown;
    public float damage;

    public GameObject attackTarget;
    public Flag walkTarget;
    private bool stopped = false;
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
        GetFirstFlag();
    }
    private void Start()
    {
        gem.StartListening("Stop", Stopped);
    }
    private void Stopped(GameObject target, List<object> parameters)
    {
        Debug.Log("Stopping!");
        stopped = true;
    }
    void Update()
    {
        if (stopped)
        {
            return;
        }
        if (attackTarget == null)
        {
            if (gameObject.name.Contains("Hooligan"))
            {
                SearchForMeleeAttackTargets();
            }
            else if (gameObject.name.Contains("HooliganShooter"))
            {
                SearchForRangedAttackTargets();
            }
        }
        if (attackTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, attackTarget.transform.position, Time.deltaTime * speed/10);
        }
        else if (walkTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, walkTarget.transform.position, Time.deltaTime * speed);
        }
        else
        {
        }
    }
    private void SearchForMeleeAttackTargets()
    {
        RaycastHit[] objectsHit = Physics.SphereCastAll(transform.position,3f,Vector3.forward);
        foreach (RaycastHit other in objectsHit)
        {
            if (other.collider.gameObject.CompareTag("Robot"))
            {
                attackTarget = other.collider.gameObject;
                Debug.Log("Attack robots!");

                StartCoroutine(Attack());
                break;
            }
            else if (other.collider.gameObject.CompareTag("EngineerObstacle"))
            {
                attackTarget = other.collider.gameObject;
                Debug.Log("Attack robots!");

                StartCoroutine(Attack());
                break;
            }
            else if (other.collider.gameObject.CompareTag("BombRoom"))
            {
                attackTarget = GameObject.Find("Door");
                Debug.Log("Attack bombroom!");

                StartCoroutine(Attack());
                break;
            }
        }
    }
    private void SearchForRangedAttackTargets()
    {
        RaycastHit[] objectsHit = Physics.SphereCastAll(transform.position, 10f, Vector3.forward);
        foreach (RaycastHit other in objectsHit)
        {
            if (other.collider.gameObject.tag == "Robot" )
            {
                if (other.collider.gameObject.GetComponent<TurnedFixed>().isFixed)
                {
                    attackTarget = other.collider.gameObject;
                    Debug.Log("Attack robots!");

                    StartCoroutine(Attack());
                    break;
                }
            }
            if (other.collider.gameObject.tag == "BombRoom")
            {
                attackTarget = GameObject.Find("Door");
                Debug.Log("Attack bombroom!");

                StartCoroutine(Attack());
                break;
            }
        }
    }
    private Coroutine attacking;
    private void OnTriggerEnter(Collider other)
    {
        if (attacking != null)
        {
            return;
        }
        Flag flag = other.transform.GetComponent<Flag>();
        if (flag != null)
        {
            if (flag.gameObject.name == "BombRoomCenterPosition")
            {
                gem.TriggerEvent("Explode", gameObject);
            }
            walkTarget = flag.nextFlag;
        }
    }
    private IEnumerator Attack()
    {
        if (gameObject.name.Contains("Hooligan"))
        {
            while (attackTarget != null)
            {
                yield return new WaitForSeconds(attackCooldown);
                gem.TriggerEvent("Attack", attackTarget, new List<object> { damage });
            }
        }
        else if (gameObject.name.Contains("HooliganShooter"))
        {
            while (attackTarget != null)
            {
                yield return new WaitForSeconds(attackCooldown);
                gem.TriggerEvent("Attack", attackTarget, new List<object> { damage });
            }
        }
    }
    private void GetFirstFlag()
    {
        walkTarget = GameObject.Find("Path0").GetComponent<Flag>();
    }
}
