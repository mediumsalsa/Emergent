using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnts : MonoBehaviour
{
    public GameObject ant;
    public int numberOfAnts;
    public float spawnRadius = 1.0f; // Adjust the radius as needed

    void Start()
    {
        for (int i = 0; i < numberOfAnts; i++)
        {
            // Generate a random position within a sphere
            Vector3 randomPosition = gameObject.transform.position + Random.insideUnitSphere * spawnRadius;

            Instantiate(ant, randomPosition, Quaternion.identity);
        }
    }
}
