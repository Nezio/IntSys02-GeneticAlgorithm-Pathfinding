using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject IndividualPrefab;
    public int iterations;
    public int populationSize;
    public float mutatioProb;


    private List<GameObject> population = new List<GameObject>();
    private Transform spawnPoint;

    private void Start()
    {
        // generate initial population
        spawnPoint = GameObject.FindGameObjectWithTag("spawnPoint").transform;
        for (int i = 0; i < populationSize; i++)
        {
            population.Add(Instantiate(IndividualPrefab, spawnPoint.position, spawnPoint.rotation));
        }

        StartSimulation();

        // wait for simulation to finish
        StartCoroutine(WaitForSimulation());

    }

    private void StartSimulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            StartCoroutine(population[i].GetComponent<Individual>().RunTimer());
        }
            
    }

    private IEnumerator WaitForSimulation()
    { // waiting for simulation to finish
        int numberDone = 0;     // how many individuals finished their run
        while (numberDone < populationSize)
        {
            numberDone = 0;
            foreach (GameObject individual in population)
            {
                if (individual.GetComponent<Individual>().isDone)
                    numberDone++;
            }

            yield return new WaitForSeconds(0.5f);
        }

        //Debug.Log("Sim Done!");

        RunGA();

        yield return 0;
    }

    private void RunGA()
    {
        NormalizeFitness();

        List<GameObject> newPopulation = GeneticAlgorithm(population);

        // debug
        /*Debug.Log(newPopulation[0].GetComponent<Individual>().turns[0]);
        for(int i = 0; i < population.Count; i++)
        {
            Individual ind = population[i].GetComponent<Individual>();
            Individual newInd = newPopulation[i].GetComponent<Individual>();
            string oldTurns = "";
            string newTurns = "";
            foreach (float f in ind.turns)
            {
                oldTurns += f.ToString() + " ";
            }
            foreach (float f in newInd.turns)
            {
                newTurns += f.ToString() + " ";
            }
            Debug.Log("old_" + i + " : " + oldTurns);
            Debug.Log("new_" + i + " : " + newTurns);
        }*/

        DestroyOldPopulation(population);
        population = newPopulation;

        // restart sim if needed
        iterations--;
        if (iterations > 0)
        {
            StartSimulation();

            StartCoroutine(WaitForSimulation());
        }
        else
        {
            // display solution

        }
    }
    
    private void DestroyOldPopulation(List<GameObject> oldPopulation)
    {
        foreach (GameObject g in oldPopulation)
            Destroy(g);
    }

    private void RestartSimulation()    // DEPRICATED
    {
        foreach (GameObject individual in population)
        {
            individual.transform.position = spawnPoint.position;
            individual.transform.rotation = spawnPoint.rotation;

            individual.GetComponent<Individual>().isDone = false;
            StartCoroutine(individual.GetComponent<Individual>().RunTimer());
        }

        StartCoroutine(WaitForSimulation());
    }

    private void NormalizeFitness()
    { // normalize to range 0 - 100
        List<float> fitnessData = new List<float>();
        List<float> normalizedFitnessData = new List<float>();

        // copy fitness from individuals to list
        foreach (GameObject individual in population)
        {
            fitnessData.Add(individual.GetComponent<Individual>().GetFitness());
        }

        // normalize fitness data and save to new list
        foreach(float f in fitnessData)
        {
            normalizedFitnessData.Add(1 - ((f - Tools.ListMin(fitnessData)) / (Tools.ListMax(fitnessData) - Tools.ListMin(fitnessData))));
        }

        //for(int i = 0; i < fitnessData.Count; i++)
        //{ Debug.Log(fitnessData[i] + " -> " + normalizedFitnessData[i]); }

        // return normalized data to each individual
        for(int i = 0; i < population.Count; i ++)
        {
            population[i].GetComponent<Individual>().normalizedFitness = normalizedFitnessData[i];
        }
        
    }

    private List<GameObject> GeneticAlgorithm(List<GameObject> population)
    {
        List<GameObject> newPopulation = new List<GameObject>();

        for(int i = 0; i < population.Count; i++)
        {
            GameObject x = RandomFitIndividual(population);
            GameObject y = RandomFitIndividual(population);

            GameObject child = Reproduce(x, y);
            Mutate(child);

            newPopulation.Add(child);
        }

        return newPopulation;
    }
    
    private GameObject RandomFitIndividual(List<GameObject> population)
    {
        float random = Random.value;
        GameObject individual = null;
        
        while(individual == null)
        {
            int randomIdx = Random.Range(0, population.Count);
            if (population[randomIdx].GetComponent<Individual>().normalizedFitness >= random)
                individual = population[randomIdx];
        }

        return individual;
    }

    private GameObject Reproduce(GameObject x, GameObject y)
    {
        GameObject child = Instantiate(IndividualPrefab, spawnPoint.position, spawnPoint.rotation);

        int numTurns = child.GetComponent<Individual>().turns.Count;    // how many turns each individual performs during its lifespan
        int padding = (int)Mathf.Floor(numTurns * 0.2f);    // used so splitIndex isn't too close to boundaries (always take from both parents)
        int splitIndex = Random.Range(0+padding, numTurns-padding);

        for(int i = 0; i < numTurns; i++)
        {
            if (i < splitIndex)
                child.GetComponent<Individual>().turns[i] = x.GetComponent<Individual>().turns[i];
            else
                child.GetComponent<Individual>().turns[i] = y.GetComponent<Individual>().turns[i];
        }

        return child;
    }

    private void Mutate(GameObject child)
    {
        Individual c = child.GetComponent<Individual>();

        for (int i = 0; i < c.turns.Count; i++)
        {
            if (mutatioProb > Random.value)
                c.turns[i] = c.turnAngles[Random.Range(0, c.turnAngles.Count)];
        }
        
    }

}
