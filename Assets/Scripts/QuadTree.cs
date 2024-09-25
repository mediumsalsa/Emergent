using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree : MonoBehaviour
{

    public const int maxObjects = 2;
    public const int maxSubdivides = 5;

    private int currentSubdivide;

    private List<GameObject> ants;

    private Rect bounds;

    private QuadTree[] subdivides;


}
