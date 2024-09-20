using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnts : MonoBehaviour
{

    public GameObject ant;
    public int numberOfAnts;

    void Start()
    {
        for (int i = 0; i < numberOfAnts; i++)
        {
            Instantiate(ant, gameObject.transform.position, Quaternion.identity);
        }
    }

}
