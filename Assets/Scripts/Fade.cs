using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public float lifetime = 0.5f; // Duration before fading

    [HideInInspector]
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        Color color = spriteRenderer.color;
        color.a -= Time.deltaTime / lifetime; 
        spriteRenderer.color = color;

        if (color.a <= 0)
        {
            color.a = 0; 
            spriteRenderer.color = color;
        }
    }
}
