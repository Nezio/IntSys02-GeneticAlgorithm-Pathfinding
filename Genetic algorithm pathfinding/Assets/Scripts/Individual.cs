using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Individual : MonoBehaviour
{
    public float speed;
    public int steps;
    public float stepLength = 1;    // how long to move forward before choosing a new angle

    [HideInInspector]
    public bool isDone = false;     // true when done with simulation
    [HideInInspector]
    public float normalizedFitness;
    [HideInInspector]
    public List<float> turns = new List<float>();    
    [HideInInspector]
    public float currentSpeed = 0;
    [HideInInspector]
    public float collisionSpeedModifier = 1;    // 0 if collision is detected, else 1

    private float fitness;
    private float timeToFinish = 0;

    private void Awake()
    {
        // initialize settings
        steps = Settings.steps;
        stepLength = Settings.stepLength;

        // sharp turns
        for (int i = 0; i < steps; i++)
        {
            //turns.Add(turnAngles[Random.Range(0, turnAngles.Count)]);
            turns.Add(Random.Range(0, 360));
        }
        
        // smooth turns
        /*turns.Add(Random.Range(0, 360));
        for (int i = 0; i < maxTurns - 1; i++)
        {
            //turns.Add(turnAngles[Random.Range(0, turnAngles.Count)]);
            turns.Add(turns[0] + Random.Range(-5, +5));
        }*/
    }
    
    private void Update()
    {
        gameObject.transform.Translate(Vector3.up * Time.deltaTime * currentSpeed * collisionSpeedModifier);
    }

    public IEnumerator RunTimer()
    {
        yield return new WaitForSeconds(0.25f);         // wait a bit
        currentSpeed = speed;                           // start moving
        StartCoroutine(MeasureTime());                  // measure time to finish (used to improve fitness)
        List<float> tmpTurns = new List<float>(turns);  // copy turns data

        while (tmpTurns.Count > 0)
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, tmpTurns[0]));   // rotate to new angle

            tmpTurns.RemoveAt(0);   // remove angle that was just used up

            // measure best fitness
            float newFitness = CalculateFitness();
            if (newFitness < fitness)
                fitness = newFitness;

            yield return new WaitForSeconds(stepLength);
        }   // all movement done

        currentSpeed = 0;   // freeze individual
        isDone = true;      // individual is done with movement in this generation

        fitness = CalculateFitness();
        fitness += timeToFinish;        // improve final fitness with time taken to finish

        yield return 0;
    }

    private IEnumerator MeasureTime()
    { // measure time to finish (used to improve fitness)
        while (!isDone)
        {
            timeToFinish += 0.1f;
            yield return new WaitForSeconds(0.01f);
        }

        yield return 0;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // freeze on collision with end point
        if (collision.gameObject.tag == "endPoint")
        {
            currentSpeed = 0;
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
