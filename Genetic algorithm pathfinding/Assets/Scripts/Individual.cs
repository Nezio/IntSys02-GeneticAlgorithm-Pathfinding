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
    private float timeToFinish = 0;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        turnAngles.AddRange(new float[] { 0, 45, 90, 135, 180, 225, 270, 315 });

        // sharp turns
        for (int i = 0; i < maxTurns; i++)
        {
            //turns.Add(turnAngles[Random.Range(0, turnAngles.Count)]);
            turns.Add(Random.Range(0, 360));
        }
        
        // smooth turns
        //turns.Add(Random.Range(0, 360));
        //for (int i = 0; i < maxTurns - 1; i++)
        //{
        //    //turns.Add(turnAngles[Random.Range(0, turnAngles.Count)]);
        //    turns.Add(turns[0] + Random.Range(-5, +5));
        //}
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
        yield return new WaitForSeconds(0.25f);         // wait a bit
        currentThrust = thrust;                         // start moving
        StartCoroutine(MeasureTime());                  // measure time to finish (used to improve fitness)
        List<float> tmpTurns = new List<float>(turns);  // copy turns data
        while(tmpTurns.Count > 0)
        {
            rb.MoveRotation(tmpTurns[0]);
            tmpTurns.RemoveAt(0);

            // measure best fitness
            float newFitness = CalculateFitness();
            if (newFitness < fitness)
                fitness = newFitness;

            yield return new WaitForSeconds(stepLength);
        }

        currentThrust = 0;
        isDone = true;

        fitness = CalculateFitness();
        fitness += timeToFinish;        // improve final fitness with time taken to finish

        yield return 0;
    }

    private IEnumerator MeasureTime()
    {
        while (currentThrust != 0)
        {
            timeToFinish += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

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

    private float CalculateFitness()
    {
        return Vector2.Distance(gameObject.transform.position, GameObject.FindGameObjectWithTag("endPoint").transform.position);
    }

    public float GetFitness()
    {
        return fitness;
    }


}
