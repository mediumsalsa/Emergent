using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : MonoBehaviour
{
    [Header("Gizmo Settings")]
    public bool gizmoVisual = true;

    [Header("Movement Settings")]
    public float maxSpeed = 6;
    public float steerStrength = 20;
    public float wanderStrength = 0.1f;

    [Header("Detection Settings")]
    public float foodDetectionDistance;
    public float foodDetectionRadius;
    public int framesBetweenFoodCheck = 5;
    private int frameCounter = 0;

    [Header("Trail Settings")]
    public GameObject foodTrailPrefab;
    public GameObject homeTrailPrefab;
    public Color trailColour;
    public float trailInterval = 0.1f; 
    private float lastTrailTime;

    private Vector2 desiredDirection;
    private Rigidbody2D rb;
    private Transform target;

    private Transform homeTarget;
    private SpriteRenderer spriteRenderer;

    private bool holdingFood = false;

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
        frameCounter++;

        if (frameCounter >= framesBetweenFoodCheck)
        {
            CheckForFood();
            frameCounter = 0;
        }

        Vector2 randomTarget = (desiredDirection + Random.insideUnitCircle * wanderStrength).normalized;

        Vector2 foodTarget = target != null ? ((Vector2)target.position - rb.position).normalized : randomTarget;

        //Set the desired direction based on the presence of a target
        if (target != null)
        {
            desiredDirection = foodTarget;
        }
        else
        {
            desiredDirection = randomTarget;
        }

        //Calculate steering
        Vector2 desiredVelocity = desiredDirection.normalized * maxSpeed;
        Vector2 desiredSteeringForce = (desiredVelocity - rb.velocity) * steerStrength;

        Vector2 steeringForce = desiredSteeringForce;

        Vector2 acceleration = Vector2.ClampMagnitude(steeringForce, steerStrength);

        //Update the ant's speed
        rb.velocity = Vector2.ClampMagnitude(rb.velocity + acceleration * Time.deltaTime, maxSpeed);

        //Rotate the ant based on its velocity
        if (rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }

        //Ant color change logic
        if (holdingFood)
        {
            trailColour = Color.red;
            if (Time.time - lastTrailTime > trailInterval)
            {
                lastTrailTime = Time.time;
                CreateTrail(trailColour, foodTrailPrefab);
            }
        }
        else
        {
            trailColour = Color.blue;
            if (Time.time - lastTrailTime > trailInterval)
            {
                lastTrailTime = Time.time;
                CreateTrail(trailColour, homeTrailPrefab);
            }
        }

    }

    void CreateTrail(Color color, GameObject trailPrefab)
    {
        GameObject trail = Instantiate(trailPrefab, transform.position, Quaternion.identity);
        trail.GetComponent<SpriteRenderer>().color = color;
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
                    }
                }
            }
        }
    }



    void CheckForFoodTrail()
    {
        if (!holdingFood )
        {
            if (target != null && target.tag == "Food") return;

            GameObject[] trailObjects = GameObject.FindGameObjectsWithTag("FoodTrail");
            Transform closestTrail = null;
            float lowestRemainingLifetime = float.MaxValue;

            //If no food is detected, check for the trail

                foreach (GameObject trail in trailObjects)
                {
                    Vector2 directionToTrail = (Vector2)(trail.transform.position - transform.position);
                    float distanceToTrail = directionToTrail.magnitude;

                    if (distanceToTrail <= foodDetectionDistance / 2)
                    {
                        float angle = Vector2.Angle(transform.right, directionToTrail);

                        if (angle <= foodDetectionRadius)
                        {
                            //Get the Fade component to access remaining lifetime
                            Fade fadeComponent = trail.GetComponent<Fade>();
                            float remainingLifetime = fadeComponent.lifetime - (Time.time - fadeComponent.spriteRenderer.color.a);

                            //Check if this trail has the lowest remaining lifetime so far
                            if (remainingLifetime < lowestRemainingLifetime)
                            {
                                lowestRemainingLifetime = remainingLifetime;
                                closestTrail = trail.transform; // Update the target
                            }
                        }
                    }
                }
            
            if (closestTrail != null)
            {
                target = closestTrail;
            }
        }
    }


    void OnDrawGizmos()
    {
        if (gizmoVisual)
        {
            //Draw the food detection distance
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, foodDetectionDistance);

            //Draw the food detection direction
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
            if (holdingFood == false)
            { 
                Destroy(other.gameObject);
            }
            holdingFood = true;
            target = homeTarget;
        }

        if (other.CompareTag("Home"))
        {
            if (holdingFood == true)
            {
                holdingFood = false;
                target = null;
            }

            float tempRadius = foodDetectionRadius;
            foodDetectionRadius = 360;

            CheckForFoodTrail();

            foodDetectionRadius = tempRadius;
        }

        if (other.CompareTag("FoodTrail"))
        {
            if (!holdingFood)
            {
                CheckForFoodTrail();
            }
        }
    }


}
