using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnedFixed : MonoBehaviour
{
    public bool isFixed = false;
    public GameObject smokePS;
    public GameObject[] sounds;
    public int maxRepair = 5;
    private int currentRepair;
    private Color original;
    private BasicRobot basicRobot;
    private EngineerRobot engineerRobot;
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
        basicRobot = GetComponent<BasicRobot>();
        currentRepair = maxRepair;
        if (GetComponent<BasicRobot>() == null)
        {
            engineerRobot = GetComponent<EngineerRobot>();
        }
    }
    void Start()
    {
        gem.StartListening("RobotBreak", Break);
    }
    void OnDestroy()
    {
        gem.StopListening("RobotBreak", Break);
    }

    private void Break(GameObject robot, List<object> parameters)
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
    }
    private void ResetValues()
    {
        isFixed = false;
        currentRepair = maxRepair;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isFixed)
        {
            TurnedFix();
            ControlSmoke();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        bool pickedup = false;
        if (basicRobot != null)
        {
            pickedup = basicRobot.pickedUp;
        }
        if (engineerRobot != null)
        {
            if (pickedup)
            {
                pickedup = engineerRobot.pickedUp;
            }
        }
        if (other.gameObject.tag == "Wrench" && !isFixed && !pickedup)
        {
            currentRepair -= 1;
            NewSoundEffect();
        }
    }

    private void TurnedFix()
    {
        if(currentRepair < 1)
        {
            isFixed =true;
            ControlSmoke();
        }
    }

    private void ControlSmoke()
    {
        smokePS.SetActive(!isFixed);
    }

    private void NewSoundEffect()
    {
        Instantiate(sounds[UnityEngine.Random.Range(0, sounds.Length)],
                    transform.position,
                    transform.rotation);
    }
}
