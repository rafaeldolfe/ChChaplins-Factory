using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private Rigidbody rigidbody;

    public float fadeOutIntervals;
    public void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    public void RipOffBoard()
    {
        rigidbody.constraints = 0;
        rigidbody.useGravity = true;
        rigidbody.AddTorque(new Vector3(0, 0, 5));
        rigidbody.AddForce(new Vector3(5, 0, 0));
        StartCoroutine(Despawn());
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(gameObject);
    }
}
