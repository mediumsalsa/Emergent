using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuadTree : MonoBehaviour
{
    private QuadTree qt;

    private void Awake()
    {
        qt = gameObject.GetComponent<QuadTree>();

        if (qt == null)
        {
            Debug.LogError("QuadTree component not found on this GameObject.");
            qt = gameObject.AddComponent<QuadTree>();
        }

        qt.Initialize(0, new Rect(-20, -20, 40, 40));
    }
     
    private void Update()
    {
            List<GameObject> existingAnts = GetExistingAnts();
            qt.OrganizeObjects(existingAnts);
            qt.DrawTree(); 

    }

    private List<GameObject> GetExistingAnts()
    {
        return new List<GameObject>(GameObject.FindGameObjectsWithTag("Ant"));
    }

}
