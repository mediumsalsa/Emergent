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
    public Transform target;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //No target, moves randomly
        Vector2 randomTarget = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;
        //Follows target
        Vector2 foodTarget = ((Vector2)target.position - rb.position).normalized;

        //Ant wants to go to current target
        desiredDirection = foodTarget;

        Vector2 avoidanceForce = CalculateAvoidanceForce();

        Vector2 desiredVelocity = (desiredDirection + avoidanceForce).normalized * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - rb.velocity) * steerStrength;
        Vector2 acceleration = Vector2.ClampMagnitude(desiredSteeringForce, steerStrength);


        //Calculated the ants speed
        rb.velocity = Vector2.ClampMagnitude(rb.velocity + acceleration * Time.deltaTime, maxSpeed);

        //Turns the calculated radius into a degree, to rotate the ant
        if (rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }
    }


    //Avoids colliding with other ants
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
