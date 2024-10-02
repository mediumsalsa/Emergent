using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadtree : MonoBehaviour
{

    [SerializeField] private int height = 100;
    [SerializeField] private int width = 100;

    private Vector2 ne;
    private Vector2 nw;
    private Vector2 se;
    private Vector2 sw;

    private Rect currentQuad;

    private int numberOfSplits;

    private void Start()
    {
        ne = new Vector2(-width, height);
        nw = new Vector2(width, height);
        se = new Vector2(-width, -height);
        sw = new Vector2(width, -height);

        currentQuad = new Rect(ne.x, ne.y, nw.x, se.y);
    }

    private void SplitTree()
    {
        ne.x /= 2; ne.y /= 2;
        nw.x /= 2; nw.y /= 2;
        se.x /= 2; se.y /= 2;
        sw.x /= 2; sw.y /= 2;
    }


    private void OnDrawGizmos()
    {
        ne = new Vector2(-width, height);
        nw = new Vector2(width, height);
        se = new Vector2(-width, -height);
        sw = new Vector2(width, -height);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(ne, nw);
        Gizmos.DrawLine(ne, se);
        Gizmos.DrawLine(se, sw);
        Gizmos.DrawLine(sw, nw);
    }

}
