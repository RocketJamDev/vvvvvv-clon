using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trinket : MonoBehaviour
{

    public GameController gameController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            gameController.AddTrinket();
        }
    }
}
