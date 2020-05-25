using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public List<GameObject> hooligans;
    public List<GameObject> hooliganSpawnLocations;
    public float offset = 0;

    private bool[] spawnTriggers = new bool[20];
    private bool[] robotGuaranteedsTriggers = new bool[20];

    private int spawnCounter = 0;
    private int robotCounter = 0;
    void Update()
    {
        RobotSpawnTriggers();
        HooliganSpawnTriggers();
        WinTrigger();
    }
    private void RobotSpawnTriggers()
    {
        float time = Time.time + offset;
        if (time > 0 && !robotGuaranteedsTriggers[0])
        {
            DisableRobotTrigger();
            SpawnRates.AddBasicSpawnBoost(100);
        }
        if (time > 100 && !robotGuaranteedsTriggers[1])
        {
            DisableRobotTrigger();
            SpawnRates.AddBasicSpawnBoost(100);
            SpawnRates.AddPowerfulSpawnBoost(100);
        }
        if (time > 150 && !robotGuaranteedsTriggers[2])
        {
            DisableRobotTrigger();
            SpawnRates.AddPowerfulSpawnBoost(100);
            SpawnRates.AddEngineerSpawnBoost(100);
        }
        if (time > 200 && !robotGuaranteedsTriggers[3])
        {
            DisableRobotTrigger();
            SpawnRates.AddPowerfulSpawnBoost(100);
            SpawnRates.AddPowerfulSpawnBoost(100);
            SpawnRates.AddEngineerSpawnBoost(100);
            SpawnRates.AddEngineerSpawnBoost(100);
        }
        if (time > 300 && !robotGuaranteedsTriggers[4])
        {
            DisableRobotTrigger();
            SpawnRates.AddPowerfulSpawnBoost(100);
            SpawnRates.AddPowerfulSpawnBoost(100);
            SpawnRates.AddPowerfulSpawnBoost(100);
            SpawnRates.AddPowerfulSpawnBoost(100);
            SpawnRates.AddPowerfulSpawnBoost(100);
            SpawnRates.AddPowerfulSpawnBoost(100);
            SpawnRates.AddPowerfulSpawnBoost(100);
        }
    }
    private void HooliganSpawnTriggers()
    {
        float time = Time.time + offset;
        if (time > 0 && !spawnTriggers[0])
        {
            DisableSpawnTrigger();

            StartCoroutine(SpawnHooligansWithDelay(3, 1, "Melee"));
            StartCoroutine(SpawnHooligansWithDelay(3, 1, "Melee"));
        }
        if (time > 50 && !spawnTriggers[1])
        {
            DisableSpawnTrigger();

            StartCoroutine(SpawnHooligansWithDelay(5, 1, "Melee"));
            StartCoroutine(SpawnHooligansWithDelay(2, 1, "Shooter"));
        }
        if (time > 90 && !spawnTriggers[2])
        {
            DisableSpawnTrigger();

            StartCoroutine(SpawnHooligansWithDelay(7, 1, "Melee"));
            SpawnHooligans(3, "Shooter");
        }
        if (time > 140 && !spawnTriggers[3])
        {
            DisableSpawnTrigger();

            StartCoroutine(SpawnHooligansWithDelay(7, 2, "Melee"));
            StartCoroutine(SpawnHooligansWithDelay(3, 2, "Shooter"));
        }
        if (time > 200 && !spawnTriggers[4])
        {
            DisableSpawnTrigger();

            StartCoroutine(SpawnHooligansWithDelay(10, 2, "Melee"));
            StartCoroutine(SpawnHooligansWithDelay(8, 4, "Shooter"));
        }
        if (time > 250 && !spawnTriggers[5])
        {
            DisableSpawnTrigger();

            SpawnHooligans(25, "Melee");
            StartCoroutine(SpawnHooligansWithDelay(20, 2, "Melee"));
            StartCoroutine(SpawnHooligansWithDelay(4, 6, "Shooter"));
        }
        if (time > 320 && !spawnTriggers[6])
        {
            DisableSpawnTrigger();

            StartCoroutine(SpawnHooligansWithDelay(10, 3, "Melee"));
            StartCoroutine(SpawnHooligansWithDelay(3, 2, "Shooter"));
        }
        if (time > 390 && !spawnTriggers[7])
        {
            DisableSpawnTrigger();

            StartCoroutine(SpawnHooligansWithDelay(10, 7, "Melee"));
            StartCoroutine(SpawnHooligansWithDelay(8, 3, "Shooter"));
        }
        if (time > 480 && !spawnTriggers[8])
        {
            DisableSpawnTrigger();

            SpawnHooligans(25, "Melee");
            StartCoroutine(SpawnHooligansWithDelay(20, 2, "Melee"));
            StartCoroutine(SpawnHooligansWithDelay(4, 6, "Shooter"));
        }
    }
    private void WinTrigger()
    {

    }
    private void DisableRobotTrigger()
    {
        robotGuaranteedsTriggers[robotCounter] = true;
        robotCounter += 1;
    }
    private void DisableSpawnTrigger()
    {
        spawnTriggers[spawnCounter] = true;
        spawnCounter += 1;
    }

    private IEnumerator SpawnHooligansWithDelay(int iter, int amount, string type)
    {
        for (int i = 0; i < iter; i++)
        {
            SpawnHooligans(amount, type);
            yield return new WaitForSeconds(1);
        }
    }
    private void SpawnHooligans(int amount, string type)
    {
        for (int i = 0; i < amount; i++)
        {
            if (type == "Melee")
            {
                SpawnMeleeHooligan();
            }
            else if(type == "Shooter")
            {
                SpawnShooterHooligan();
            }
        }
    }
    private void SpawnMeleeHooligan()
    {
        GameObject meleeHooligan = hooligans[0];
        if (!meleeHooligan.name.Contains("HooliganMelee"))
        {
            throw new System.Exception("Spawning wrong hooligan, expected melee hooligan first in list");
        }
        GameObject clone = Instantiate(meleeHooligan, hooliganSpawnLocations[Random.Range(0, 2)].transform.position, Quaternion.identity);
        Rigidbody rigidbody = clone.GetComponent<Rigidbody>();
        rigidbody.AddForce(new Vector3(Random.Range(1, 4), 0, Random.Range(1, 4)) * 100);
    }
    private void SpawnShooterHooligan()
    {
        GameObject shooterHooligan = hooligans[1];
        if (!shooterHooligan.name.Contains("HooliganShooter"))
        {
            throw new System.Exception("Spawning wrong hooligan, expected shooter hooligan second in list");
        }
        GameObject clone = Instantiate(shooterHooligan, hooliganSpawnLocations[Random.Range(0, 2)].transform.position, Quaternion.identity);
        Rigidbody rigidbody = clone.GetComponent<Rigidbody>();
        rigidbody.AddForce(new Vector3(Random.Range(1, 4), 0, Random.Range(1, 4)) * 100);
    }
    private IEnumerator ReAddConstraints(GameObject clone, RigidbodyConstraints cons)
    {
        yield return new WaitForSeconds(1);
        if (clone != null)
        {
            Rigidbody rigidbody = clone.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.constraints = cons;
            }
        }
    }
}
