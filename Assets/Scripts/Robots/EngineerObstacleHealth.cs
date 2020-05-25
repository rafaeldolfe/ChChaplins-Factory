using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EngineerObstacleHealth : MonoBehaviour
{
    private GlobalEventManager gem;

    public float health;
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

    private void Start()
    {
        gem.StartListening("Attack", Attacked);
    }
    private void OnDestroy()
    {
        gem.StopListening("Attack", Attacked);
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

}
