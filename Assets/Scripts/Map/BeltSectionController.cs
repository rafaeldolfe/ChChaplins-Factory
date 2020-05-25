using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BeltSectionController : MonoBehaviour
{
    public GameObject[] robots;
    public GameObject respawnPosition;

    public float speed;

    private GameObject robot;
    private GlobalEventManager gem;
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
        respawnPosition = GameObject.Find("ConveyorRespawn");
    }
    void Update()
    {
        transform.position += Vector3.right * Time.deltaTime * speed;
        if (robot != null)
        {
            robot.transform.position += Vector3.right * Time.deltaTime * speed;
        }
    }
    private void Start()
    {
        gem.StartListening("GrabbedRobot", Grabbed);
        gem.StartListening("Explode", Exploded);
    }
    private void OnDestroy()
    {
        gem.StopListening("GrabbedRobot", Grabbed);
        gem.StopListening("Explode", Exploded);
    }
    private void Exploded(GameObject target, List<object> parameters)
    {
        if (robot)
        {
            if (robot.GetComponent<Rigidbody>())
            {
                robot.GetComponent<Rigidbody>().constraints =
                    RigidbodyConstraints.FreezePositionX |
                    RigidbodyConstraints.FreezePositionZ |
                    RigidbodyConstraints.FreezeRotationX |
                    RigidbodyConstraints.FreezeRotationY |
                    RigidbodyConstraints.FreezeRotationZ;
                robot = null;
            }
        }
    }
    private void Grabbed(GameObject target, List<object> parameters)
    {
        if (robot != target)
        {
            return;
        }

        robot.GetComponent<Rigidbody>().constraints =
            RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;
        robot = null;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name == "ConveyorDespawner")
        {
            RespawnConveyor();
        }
    }

    private void RespawnConveyor()
    {
        Destroy(robot);
        transform.position = respawnPosition.transform.position;
        RollForRobot();
    }
    private void RollForRobot()
    {
        float rand = UnityEngine.Random.Range(0, 100);
        if (rand < SpawnRates.percentageBasicRobot)
        {
            robot = Instantiate(robots[0], transform.position + new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (rand < SpawnRates.percentagePowerfulRobot)
        {
            robot = Instantiate(robots[1], transform.position + new Vector3(0, 0, 0), Quaternion.identity);
        }
        else if (rand < SpawnRates.percentageEngineerRobot)
        {
            robot = Instantiate(robots[2], transform.position + new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
