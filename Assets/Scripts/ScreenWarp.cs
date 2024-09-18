using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class ScreenWarp : MonoBehaviour
{

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        float rightSideOfScreen = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).x;
        float leftSideOfScreen = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).x;

        float topSideOfScreen = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height)).y;
        float bottomSideOfScreen = Camera.main.ScreenToWorldPoint(new Vector2(0f, 0f)).y;

        //Moves through left of screen
        if (screenPos.x <= 0 && rb.velocity.x < 0)
        {
            transform.position = new Vector2(rightSideOfScreen, transform.position.y);
        }
        //Moves through right of screen
        else if (screenPos.x >= Screen.width && rb.velocity.x > 0)
        {
            transform.position = new Vector2(leftSideOfScreen, transform.position.y);
        }
        //Moves through top of screen
        else if (screenPos.y >= Screen.height && rb.velocity.y > 0)
        {
            transform.position = new Vector2(transform.position.x, bottomSideOfScreen);
        }
        else if (screenPos.y <= 0 && rb.velocity.y < 0)
        {
            transform.position = new Vector2(transform.position.x, topSideOfScreen);
        }

    }

}
