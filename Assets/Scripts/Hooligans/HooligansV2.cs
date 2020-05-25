using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class HooligansV2 : MonoBehaviour
{
    private GlobalEventManager gem;

    public float speed;
    public float attackCooldown;
    public float damage;

    public GameObject attackTarget;
    public GameObject walkTarget;
    public AudioClip dyingSound;
    private bool stopped = false;
    private Animator animator;
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
        animator = GetComponent<Animator>();
        GetFirstFlag();
    }
    private void Start()
    {
        gem.StartListening("Explode", Died);
        gem.StartListening("HooliganDie", Died);
        gem.StartListening("RobotBreak", RemoveTarget);
        gem.StartListening("PickUp", RemoveTarget);
    }
    private void OnDestroy()
    {
        gem.StopListening("Explode", Died);
        gem.StopListening("HooliganDie", Died);
        gem.StopListening("PickUp", RemoveTarget);
    }
    private void Died(GameObject target, List<object> parameters)
    {
        if (gameObject != target)
        {
            return;
        }
        AudioSource.PlayClipAtPoint(dyingSound, transform.position, 3f);
        stopped = true;
        animator.SetBool("isAlive", false);
        if (attacking != null)
        {
            StopCoroutine(attacking);
        }
    }
    private void RemoveTarget(GameObject robot, List<object> parameters)
    {
        if (attackTarget != robot)
        {
            return;
        }
        attackTarget = null;
    }
    void Update()
    {
        if (stopped)
        {
            return;
        }
        if (attackTarget == null)
        {
            animator.SetBool("isAttacking", false);
            if (gameObject.name.Contains("HooliganMelee"))
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
            animator.SetBool("isAttacking", true);
            Vector3 attackTargetPos = attackTarget.transform.position;
            attackTargetPos.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, attackTargetPos, Time.deltaTime * speed/10);
            transform.LookAt(attackTargetPos);
        }
        else if (walkTarget != null)
        {
            Vector3 walkTargetPos = walkTarget.transform.position;
            walkTargetPos.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, walkTargetPos, Time.deltaTime * speed);
            LookTowards();
        }
    }
    private void SearchForMeleeAttackTargets()
    {
        RaycastHit[] objectsHit = Physics.SphereCastAll(transform.position,4f,Vector3.forward);
        objectsHit.Where(hit =>
        {
            float dist = Vector3.Distance(hit.collider.transform.position, transform.position);
            return dist < 4f;
        }).ToArray();
        foreach (RaycastHit other in objectsHit)
        {
            if (other.collider.gameObject.CompareTag("Robot"))
            {
                if (Vector3.Distance(other.collider.transform.position, transform.position) < 2f)
                {
                    attackTarget = other.collider.gameObject;

                    StartCoroutine(Attack());
                    break;
                }
            }
            else if (other.collider.gameObject.CompareTag("EngineerObstacle"))
            {
                if (Vector3.Distance(other.collider.transform.position, transform.position) < 2f)
                {
                    attackTarget = other.collider.gameObject;

                    StartCoroutine(Attack());
                    break;
                }
            }
            else if (other.collider.gameObject.CompareTag("BombRoom"))
            {
                if (Vector3.Distance(other.collider.transform.position, transform.position) < 4f)
                {
                    attackTarget = GameObject.Find("Door");
                    animator.SetBool("isAttacking", true);

                    StartCoroutine(Attack());
                    break;
                }
            }
        }
    }
    private void SearchForRangedAttackTargets()
    {
        RaycastHit[] objectsHit = Physics.SphereCastAll(transform.position, 15f, Vector3.forward);
        foreach (RaycastHit other in objectsHit)
        {
            if (other.collider.gameObject.tag == "Robot" )
            {
                if (other.collider.gameObject.GetComponent<TurnedFixed>().isFixed)
                {
                    if (Vector3.Distance(other.collider.transform.position, transform.position) < 15f)
                    {
                        attackTarget = other.collider.gameObject;
                        Debug.Log("Attack robots!");

                        StartCoroutine(Attack());
                        break;
                    }
                }
            }
            if (other.collider.gameObject.tag == "BombRoom")
            {
                if (Vector3.Distance(other.collider.transform.position, transform.position) < 15f)
                {
                    attackTarget = GameObject.Find("Door");
                    Debug.Log("Attack bombroom!");

                    StartCoroutine(Attack());
                    break;
                }
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
            if (flag.nextFlag.gameObject.name == "BombRoomCenterPosition")
            {
                walkTarget = flag.nextFlag.gameObject;
            }
            else
            {
                walkTarget = flag.nextFlag.transform.GetChild(UnityEngine.Random.Range(0, transform.childCount)).gameObject;
            }
        }
    }
    private IEnumerator Attack()
    {
        if (gameObject.name.Contains("HooliganMelee"))
        {
            while (attackTarget != null)
            {
                yield return new WaitForSeconds(attackCooldown);
                gem.TriggerEvent("Attack", attackTarget, new List<object> { damage, true });
            }
        }
        else if (gameObject.name.Contains("HooliganShooter"))
        {
            while (attackTarget != null)
            {
                yield return new WaitForSeconds(attackCooldown);
                gem.TriggerEvent("Attack", attackTarget, new List<object> { damage, false });
            }
        }
    }
    private void GetFirstFlag()
    {
        walkTarget = GameObject.Find("Path0");
    }

    private void LookTowards()
    {
        Quaternion lookRotation = Quaternion.LookRotation(walkTarget.transform.position - transform.position);

        lookRotation = new Quaternion(0, lookRotation.y, 0, lookRotation.w);

        transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  lookRotation,
                                                  .1f);
    }
}
