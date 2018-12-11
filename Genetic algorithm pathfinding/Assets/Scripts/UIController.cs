using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject generations;
    public GameObject populationSize;
    public GameObject mutationProb;
    public GameObject speed;
    public GameObject steps;
    public GameObject stepLength;
    public GameObject level;


    private void Start()
    {
        // initialize input fields with default values
        generations.GetComponent<InputField>().text = Settings.generations.ToString();
        populationSize.GetComponent<InputField>().text = Settings.populationSize.ToString();
        mutationProb.GetComponent<InputField>().text = Settings.mutationProb.ToString();
        speed.GetComponent<InputField>().text = Settings.speed.ToString();
        steps.GetComponent<InputField>().text = Settings.steps.ToString();
        stepLength.GetComponent<InputField>().text = Settings.stepLength.ToString();
        level.GetComponent<Dropdown>().value = Settings.level;
    }

    public void StartSimulation()
    {
        SetAllSettings();

        SceneManager.LoadScene("GameScene");
    }

    public void SetAllSettings()
    {
        Settings.generations = int.Parse(generations.GetComponent<InputField>().text);
        Settings.populationSize = int.Parse(populationSize.GetComponent<InputField>().text);
        Settings.mutationProb = float.Parse(mutationProb.GetComponent<InputField>().text);
        Settings.speed = float.Parse(speed.GetComponent<InputField>().text);
        Settings.steps = int.Parse(steps.GetComponent<InputField>().text);
        Settings.stepLength = float.Parse(stepLength.GetComponent<InputField>().text);
        Settings.level = level.GetComponent<Dropdown>().value;
    }


    public void Exit()
    {
        Application.Quit();
    }

}
