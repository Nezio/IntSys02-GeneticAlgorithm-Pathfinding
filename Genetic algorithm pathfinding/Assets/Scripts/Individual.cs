using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Individual : MonoBehaviour
{
    public float thrust;
    public float maxSpeed;
    public float stepLength = 1;    // how long to move forward before choosing a new angle
    public int maxTurns;

    [HideInInspector]
    public bool isDone = false;     // true when done with simulation
    [HideInInspector]
    public float normalizedFitness;
    [HideInInspector]
    public List<float> turns = new List<float>();
    [HideInInspector]
    public List<float> turnAngles = new List<float>();     // possible turn angles

    private float currentThrust = 0;
    private float fitness;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        turnAngles.AddRange(new float[] { 0, 45, 90, 135, 180, 225, 270, 315 });

        for (int i = 0; i < maxTurns; i++)
        {
            //turns.Add(turnAngles[Random.Range(0, turnAngles.Count)]);
            turns.Add(Random.Range(0, 360));
        }
    }

    private void Start()
    {        
        
        
    }

    private void FixedUpdate()
    {
        rb.AddForce(transform.up * currentThrust);
        
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    public IEnumerator RunTimer()
    {
        yield return new WaitForSeconds(0.5f);
        currentThrust = thrust;
        List<float> tmpTurns = new List<float>(turns);
        while(tmpTurns.Count > 0)
        {
            rb.MoveRotation(tmpTurns[0]);
            tmpTurns.RemoveAt(0);

            yield return new WaitForSeconds(stepLength);
        }

        currentThrust = 0;
        isDone = true;

        CalculateFitness();

        yield return 0;
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ignore collision with other individuals
        if (collision.gameObject.tag == "Individual")
        {
            Physics2D.IgnoreCollision(gameObject.GetComponent<Collider2D>(), collision.collider);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // freeze on collision with end point
        if (collision.gameObject.tag == "endPoint")
        {
            currentThrust = 0;
        }
    }

    private void CalculateFitness()
    {
        fitness = Vector2.Distance(gameObject.transform.position, GameObject.FindGameObjectWithTag("endPoint").transform.position);
        //Debug.Log(fitness);
    }

    public float GetFitness()
    {
        return fitness;
    }


}
