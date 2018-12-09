using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject generations;


    public void StartSimulation()
    {
        SetAllSettings();

        SceneManager.LoadScene("GameScene");
    }

    public void SetAllSettings()
    {
        Settings.generations = int.Parse(generations.GetComponent<InputField>().text);
    }


    public void Exit()
    {
        Application.Quit();
    }

}
