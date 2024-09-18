using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public float maxSpeed = 2;
    public float steerStrength = 2;
    public float wanderStrength = 1;
    public float avoidanceDistance = 1.5f; 
    public float detectionRadius = 2.0f; 

    private Vector2 desiredDirection;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        desiredDirection = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

        Vector2 avoidanceForce = CalculateAvoidanceForce();

        Vector2 desiredVelocity = (desiredDirection + avoidanceForce).normalized * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - rb.velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength);

        rb.velocity = Vector2.ClampMagnitude(rb.velocity + acceleration * Time.deltaTime, maxSpeed);

        if (rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
    }

    private Vector2 CalculateAvoidanceForce()
    {
        Vector2 avoidanceForce = Vector2.zero;
        GameObject[] ants = GameObject.FindGameObjectsWithTag("Ant");

        foreach (var ant in ants)
        {
            if (ant != gameObject)
            {
                Vector2 directionToAnt = (Vector2)ant.transform.position - rb.position;
                float distance = directionToAnt.magnitude;

                if (distance < avoidanceDistance)
                {
                    avoidanceForce -= directionToAnt.normalized / distance; 
                }
            }
        }

        return avoidanceForce;
    }
}
