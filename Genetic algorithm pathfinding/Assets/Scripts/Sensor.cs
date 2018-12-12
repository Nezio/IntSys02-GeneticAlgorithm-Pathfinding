using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    private float oldCurrentSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            oldCurrentSpeed = gameObject.transform.parent.gameObject.GetComponent<Individual>().currentSpeed;
            gameObject.transform.parent.gameObject.GetComponent<Individual>().currentSpeed = 0;

            //gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            gameObject.transform.parent.gameObject.GetComponent<Individual>().currentSpeed = oldCurrentSpeed;

            //gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);
        }
    }
}
