using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject IndividualPrefab;
    public int generations;
    public int populationSize;
    public float mutatioProb;
    public float timeScale;

    private List<GameObject> population = new List<GameObject>();
    private Transform spawnPoint;
    private int genCount = 1;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MenuScene");
        }
    }

    private void Start()
    {
        // initialize settings
        generations = Settings.generations;
        populationSize = Settings.populationSize;
        mutatioProb = Settings.mutationProb;
        timeScale = Settings.speed;
        Time.timeScale = timeScale;

        // load level
        GameObject levels = GameObject.FindGameObjectWithTag("Levels");
        foreach(Transform level in levels.transform)
        {
            level.gameObject.SetActive(false);  // hide other levels
        }
        levels.transform.GetChild(Settings.level).gameObject.SetActive(true);   // unhide selected level
        
        // generate initial population
        spawnPoint = GameObject.FindGameObjectWithTag("spawnPoint").transform;
        for (int i = 0; i < populationSize; i++)
        {
            population.Add(Instantiate(IndividualPrefab, spawnPoint.position, spawnPoint.rotation));
        }

        // update generation counter text on UI
        UpdateGenCountText();

        StartSimulation();

        // wait for simulation to finish
        StartCoroutine(WaitForSimulation());
    }

    private void UpdateGenCountText()
    {
        GameObject.FindGameObjectWithTag("genCount").GetComponent<Text>().text = "Generation: " + genCount + "/" + Settings.generations;
    }

    private void StartSimulation()
    {
        for (int i = 0; i < population.Count; i++)
        {
            StartCoroutine(population[i].GetComponent<Individual>().RunTimer());
        }       
    }

    private IEnumerator WaitForSimulation()
    { // waiting for simulation to finish
        int numberDone = 0;     // how many individuals finished their run
        while (numberDone < population.Count)
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
        
        generations--;
        if (generations > 0)
        { // run GA and start simulation with new generation
            
            // generation counter text
            genCount++;
            UpdateGenCountText();

            List<GameObject> newPopulation = GeneticAlgorithm(population);
            
            // update population
            DestroyOldPopulation(population);
            population = newPopulation;

            StartSimulation();

            StartCoroutine(WaitForSimulation());
        }
        else
        { // display solution

            // move population to spawn
            List<GameObject> newPopulation = new List<GameObject>();
            for (int i = 0; i < population.Count; i++)
            {
                GameObject child = Instantiate(IndividualPrefab, spawnPoint.position, spawnPoint.rotation);

                int numTurns = child.GetComponent<Individual>().turns.Count;
                for (int j = 0; j < numTurns; j++)
                {
                        child.GetComponent<Individual>().turns[j] = population[i].GetComponent<Individual>().turns[j];
                }

                child.GetComponent<Individual>().normalizedFitness = population[i].GetComponent<Individual>().normalizedFitness;

                newPopulation.Add(child);
            }
            DestroyOldPopulation(population);
            population = newPopulation;

            // find best solution (individual)
            int bestIndividualIndex = 0;
            for (int i = 0; i < population.Count; i++)
            {
                if (population[i].GetComponent<Individual>().normalizedFitness > population[bestIndividualIndex].GetComponent<Individual>().normalizedFitness)
                    bestIndividualIndex = i;
            }
            GameObject bestIndividual = population[bestIndividualIndex];

            // visually hide all but the best individual
            for (int i = 0; i < population.Count; i++)
            {
                if (i != bestIndividualIndex)
                    population[i].GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            }

            // display it in simulation
            StartSimulation();
        }
    }
    
    private void DestroyOldPopulation(List<GameObject> oldPopulation)
    {
        foreach (GameObject individual in oldPopulation)
            Destroy(individual);
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
            if (Tools.ListMax(fitnessData) == Tools.ListMin(fitnessData))
                normalizedFitnessData.Add(1);
            else
                normalizedFitnessData.Add(1 - ((f - Tools.ListMin(fitnessData)) / (Tools.ListMax(fitnessData) - Tools.ListMin(fitnessData))));
        }

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
        padding = 0;    // comment out to enable padding

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
                c.turns[i] = Random.Range(0, 360);
        }
    }

}
