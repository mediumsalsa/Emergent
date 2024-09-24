using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    public bool gizmoVisual = true;
    private bool holdingFood = false;

    public float maxSpeed = 6;
    public float steerStrength = 20;
    public float wanderStrength = 0.1f;
    public float avoidanceDistance = 6f; 
    public float detectionRadius = 6f;
    public float foodDetectionDistance;
    public float foodDetectionRadius;

    public GameObject trailPrefab;
    public Color trailColour;
    public float trailInterval = 0.1f; 
    private float lastTrailTime;


    private Vector2 desiredDirection;
    private Rigidbody2D rb;
    private Transform target;

    private Transform homeTarget;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        target = null;
        rb = GetComponent<Rigidbody2D>();
        homeTarget = GameObject.FindGameObjectWithTag("Home")?.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailColour = Color.blue;
    }

    private void Update()
    {
        CheckForFood();

        // Calculate avoidance force
        Vector2 avoidanceForce = CalculateAvoidanceForce();

        // No target, moves randomly
        Vector2 randomTarget = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

        // Follows target
        Vector2 foodTarget = target != null ? ((Vector2)target.position - rb.position).normalized : randomTarget;

        // Set the desired direction based on the presence of a target
        if (target != null)
        {
            desiredDirection = foodTarget;
        }
        else
        {
            desiredDirection = randomTarget;
            holdingFood = false;
        }

        // Calculate steering
        Vector2 desiredVelocity = desiredDirection.normalized * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - rb.velocity) * steerStrength;

        // Combine steering force with avoidance force
        Vector2 steeringForce = desiredSteeringForce + avoidanceForce * 4;

        // Apply steering force
        Vector2 acceleration = Vector2.ClampMagnitude(steeringForce, steerStrength);

        // Update the ant's speed
        rb.velocity = Vector2.ClampMagnitude(rb.velocity + acceleration * Time.deltaTime, maxSpeed);

        // Rotate the ant based on its velocity
        if (rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }

        // Ant color change logic
        spriteRenderer.color = holdingFood && target != null ? Color.red : Color.black;

        if (Time.time - lastTrailTime > trailInterval)
        {
            lastTrailTime = Time.time;
            CreateTrail(trailColour);
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

    void CheckForFood()
    {
        if (!holdingFood)
        {
            GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("Food");

            foreach (GameObject food in foodObjects)
            {
                Vector2 directionToFood = (Vector2)(food.transform.position - transform.position);
                float distanceToFood = directionToFood.magnitude;

                if (distanceToFood <= foodDetectionDistance / 2)
                {
                    float angle = Vector2.Angle(transform.right, directionToFood);

                    if (angle <= foodDetectionRadius)
                    {
                        target = food.transform;
                        holdingFood = true;
                        trailColour = Color.red;
                    }
                }
            }
        }
    }

    void CreateTrail(Color color)
    {
        GameObject trail = Instantiate(trailPrefab, transform.position, Quaternion.identity);
        trail.GetComponent<SpriteRenderer>().color = color;
    }


    void OnDrawGizmos()
    {
        if (gizmoVisual)
        {
            // Draw the food detection distance
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, foodDetectionDistance);

            // Draw the avoidance distance
            Gizmos.color = Color.red; // Change color for avoidance distance
            Gizmos.DrawWireSphere(transform.position, detectionRadius);

            // Draw the food detection direction
            Vector3 forward = transform.right * foodDetectionDistance;
            Vector3 rightBoundary = Quaternion.Euler(0, 0, foodDetectionRadius / 2) * forward;
            Vector3 leftBoundary = Quaternion.Euler(0, 0, -foodDetectionRadius / 2) * forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + forward);
            Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
            Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            target = homeTarget;
        }

        if (other.CompareTag("Home"))
        {
            holdingFood = false;
            target = null;
        }
    }
}
