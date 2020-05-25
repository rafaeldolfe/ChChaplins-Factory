using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HooliganDying : MonoBehaviour
{
    public int healthPoints = 30;
    public bool isAlive = true;

    private Animator animator;
    private GlobalEventManager gem;

    void Awake()
    {
        List<Type> depTypes = ProgramUtils.GetMonoBehavioursOnType(this.GetType());
        List<MonoBehaviour> deps = new List<MonoBehaviour>
        {
            (gem = FindObjectOfType(typeof(GlobalEventManager)) as GlobalEventManager),
        };
        if (deps.Contains(null))
        {
            throw ProgramUtils.DependencyException(deps, depTypes);
        }
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        gem.StartListening("HooliganDie", Died);
    }
    private void OnDestroy()
    {
        gem.StopListening("HooliganDie", Died);
    }
    void Update()
    {
        if(healthPoints < 1 && dying == null)
        {
            gem.TriggerEvent("HooliganDie", gameObject);
        }
    }
    private void Died(GameObject target, List<object> parameters)
    {
        if (gameObject != target)
        {
            return;
        }
        isAlive = false;
        animator.SetBool("isAlive", false);
        dying = StartCoroutine(Dying());
        gameObject.layer = 8;
        //CapsuleCollider[] caps = GetComponents<CapsuleCollider>();
        //foreach(CapsuleCollider cap in caps)
        //{
        //    Destroy(cap);
        //}
    }

    private Coroutine dying = null;
    private IEnumerator Dying()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
        if (other.transform.name == "Powerful Gun")
        {
            healthPoints -= 10;
        }
        if (other.transform.name == "Standard Gun PS")
        {
            healthPoints -= 1;
        }
    }
}
