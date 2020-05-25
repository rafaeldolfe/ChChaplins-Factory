using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PickUpRobot : MonoBehaviour
{
    private int isCarrying = 1;
    private BasicRobot pickedUpRobot;
    // Start is called before the first frame update
    private GlobalEventManager gem;
    public GameObject hand;
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
    }

    // Update is called once per frame
    void Update()
    {
        if(isCarrying == 2 && Input.GetKey(KeyCode.Space))
        {
            if (!pickedUpRobot.collidingWithRobot)
            {
                PutDown();
            }
        }
    }

    //void OnCollisionStay(Collision other)
    //{
    //    Debug.Log("Picking up robot: " + other.gameObject.name);
    //    if(isCarrying == 1 && Input.GetKey(KeyCode.Space) && other.gameObject.tag == "Robot")
    //    {
    //        pickedUpRobot = other.gameObject.GetComponent<BasicRobot>();
    //        if (pickedUpRobot != null)
    //        {
    //            gem.TriggerEvent("PickUp", other.gameObject, new List<object> { hand });
    //        }
    //        else if (other.gameObject.GetComponent<EngineerRobot>() != null)
    //        {
    //            gem.TriggerEvent("PickUp", other.gameObject, new List<object> { hand });
    //        }
           
    //        isCarrying = 0;
    //        gem.TriggerEvent("GrabbedRobot", other.gameObject);
    //        StartCoroutine("PickingUp");
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        if (isCarrying == 1 && Input.GetKey(KeyCode.Space) && other.gameObject.tag == "Robot")
        {
            Debug.Log("Triggering by: " + other.gameObject.name);
            pickedUpRobot = other.gameObject.GetComponent<BasicRobot>();
            if (pickedUpRobot != null)
            {
                gem.TriggerEvent("PickUp", other.gameObject, new List<object> { hand });
            }
            else if (other.gameObject.GetComponent<EngineerRobot>() != null)
            {
                gem.TriggerEvent("PickUp", other.gameObject, new List<object> { hand });
            }

            isCarrying = 0;
            gem.TriggerEvent("GrabbedRobot", other.gameObject);
            StartCoroutine("PickingUp");
        }
    }

    private void PutDown()
    {
        if (pickedUpRobot != null)
        {
            gem.TriggerEvent("PutDown", pickedUpRobot.gameObject);
            isCarrying = 0;
            pickedUpRobot = null;
        }
        else
        {
            isCarrying = 0;
        }
        StartCoroutine("PuttingDown");
    }

    IEnumerator PickingUp()
    {
        yield return new WaitForSeconds(0.2f);
        isCarrying = 2;
    }

    IEnumerator PuttingDown()
    {
        yield return new WaitForSeconds(0.2f);
        isCarrying = 1;
    }
}
