using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree : MonoBehaviour
{
    public const int maxObjects = 2;
    public const int maxSubdivisions = 10;

    private int level;
    private List<GameObject> ants;
    private Rect bounds;
    private QuadTree[] subdividions;

    public int AntsCount => ants.Count;

    public void Initialize(int currentSubdividion, Rect bounds)
    {
        this.level = currentSubdividion;
        this.bounds = bounds;
        this.ants = new List<GameObject>();
        this.subdividions = new QuadTree[4];
    }

    private void Subdivide()
    {
       // if (subdividions[0] != null) return;

        Debug.Log("Subdivided");

        float subWidth = bounds.width / 2f;
        float subHeight = bounds.height / 2f;
        float x = bounds.x;
        float y = bounds.y;

        for (int i = 0; i < subdividions.Length; i++)
        {
            GameObject subQuadTreeObject = new GameObject($"QuadTree_Level_{level + 1}_Index_{i}");
            subdividions[i] = subQuadTreeObject.AddComponent<QuadTree>();
        }

        // Initialize each subdivision with the correct bounds
        subdividions[0].Initialize(level + 1, new Rect(x + subWidth, y, subWidth, subHeight));
        subdividions[1].Initialize(level + 1, new Rect(x, y, subWidth, subHeight));
        subdividions[2].Initialize(level + 1, new Rect(x, y + subHeight, subWidth, subHeight));
        subdividions[3].Initialize(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight));

    }

    public void OrganizeObjects(List<GameObject> existingObjects)
    {
        ants.Clear();

        foreach (var obj in existingObjects)
        {
            if (bounds.Contains(obj.transform.position))
            {
                ants.Add(obj);
            }
        }

        if (ants.Count > maxObjects && level < maxSubdivisions)
        {
            Subdivide();
            for (int i = ants.Count - 1; i >= 0; i--)
            {
                GameObject obj = ants[i];
                int index = GetIndex(obj);
                if (index != -1)
                {
                    subdividions[index].ants.Add(obj);
                    ants.RemoveAt(i);
                }
            }
        }
    }

    private int GetIndex(GameObject obj)
    {
        Vector3 pos = obj.transform.position;
        bool topQuadrant = (pos.y > bounds.y + bounds.height / 2);
        bool bottomQuadrant = (pos.y < bounds.y + bounds.height / 2);

        if (pos.x < bounds.x + bounds.width / 2)
        {
            if (topQuadrant) return 1; // Top left
            else if (bottomQuadrant) return 2; // Bottom left
        }
        else
        {
            if (topQuadrant) return 0; // Top right
            else if (bottomQuadrant) return 3; // Bottom right
        }

        return -1; // Not in any quadrant
    }

    public void DrawTree()
    {
        // Draw the boundaries of the current QuadTree node
        Debug.DrawLine(new Vector2(bounds.x, bounds.y), new Vector2(bounds.x + bounds.width, bounds.y), Color.green);
        Debug.DrawLine(new Vector2(bounds.x, bounds.y), new Vector2(bounds.x, bounds.y + bounds.height), Color.green);
        Debug.DrawLine(new Vector2(bounds.x + bounds.width, bounds.y), new Vector2(bounds.x + bounds.width, bounds.y + bounds.height), Color.green);
        Debug.DrawLine(new Vector2(bounds.x, bounds.y + bounds.height), new Vector2(bounds.x + bounds.width, bounds.y + bounds.height), Color.green);

        //Draw subdivisions if they exist
        for (int i = 0; i < subdividions.Length; i++)
        {
            if (subdividions[i] != null)
            {
                subdividions[i].DrawTree();
            }
        }
    }
}
