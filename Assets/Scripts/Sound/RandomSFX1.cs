using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSFX1 : MonoBehaviour
{
    public GameObject[] sounds;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(sounds[Random.Range(0, sounds.Length)],
                    transform.position,
                    transform.rotation);
    }
}
