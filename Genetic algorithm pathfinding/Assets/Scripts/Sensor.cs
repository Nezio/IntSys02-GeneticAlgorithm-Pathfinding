using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    private float oldCurrentSpeed;
    private Individual individual;
    private SpriteRenderer sensorSprite;
    private bool allowMove = true;

    private void Start()
    {
        individual = gameObject.transform.parent.gameObject.GetComponent<Individual>();
        sensorSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (allowMove)
            individual.collisionSpeedModifier = 1;
        else
            individual.collisionSpeedModifier = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            allowMove = false;

            sensorSprite.color = new Color(255, 0, 0);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            allowMove = true;

            sensorSprite.color = new Color(0, 255, 0);
        }
    }
}
