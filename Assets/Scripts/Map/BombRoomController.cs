using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BombRoomController : MonoBehaviour
{
    public List<Board> boards;
    public GameObject bombWallDoor;
    public GameObject bombWallDoorPosition;
    public AudioClip boardBreakSound;
    public AudioClip explosionSound;
    private GlobalEventManager gem;

    public float maxHealth;
    private float health;
    private void Awake()
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

    private void Start()
    {
        gem.StartListening("Attack", Attacked);
        gem.StartListening("Explode", Exploded);
    }
    private void Attacked(GameObject target, List<object> parameters)
    {
        bool found = false;
        foreach(Transform transform in transform)
        {
            if (target == transform.gameObject)
            {
                found = true;
                break;
            }
        }
        if (!found)
        {
            return;
        }
        if (parameters.Count == 0)
        {
            throw new Exception("No damage parameter for attack event.");
        }
        float damageTaken = (float) parameters[0];

        TakeDamage(damageTaken);
    }
    private void TakeDamage(float damageTaken)
    {
        health -= damageTaken;
        if (health < 0)
        {
            DestroyDoor();
        }
        for (int i = 0; i < boards.Count; i++)
        {
            if (boards[i] != null && health / maxHealth <= (float)(boards.Count - i) / (float)boards.Count)
            {
                boards[i].RipOffBoard();
                AudioSource.PlayClipAtPoint(boardBreakSound, boards[i].transform.position, 5);
                boards[i] = null;
            }
        }
    }
    private void DestroyDoor()
    {
        Destroy(bombWallDoor);
        bombWallDoorPosition.GetComponent<Flag>().attackTarget = false;
    }

    private void Exploded(GameObject target, List<object> parameters)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        PushEverythingAway();
    }

    private void PushEverythingAway()
    {
        gem.TriggerEvent("Stop", gameObject);
        List<GameObject> gos = (FindObjectsOfType(typeof(GameObject)) as GameObject[]).ToList();
        foreach(GameObject go in gos)
        {
            if (go.transform.CompareTag("Robot") || go.transform.CompareTag("Hooligans") || go.transform.CompareTag("Player"))
            {
                Vector3 diff = transform.position - go.transform.position;

                Rigidbody rigidbody = go.GetComponent<Rigidbody>();
                rigidbody.constraints = 0;
                rigidbody.AddExplosionForce(5000, transform.position, 100);
            }
        }
    }
}
