using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int trinketsToVictory = 3;
    public GameObject activeCheckpoint;
    public Text trinketCount;
    public Text trinketText;

    private int trinketsCollected = 0;

 
    public void AddTrinket() 
    {
        trinketsCollected++;
        
        // Comprobamos si hemos conseguido el objetivo.
        if (CheckVictory() == false) 
        { 
            trinketCount.text = trinketsCollected.ToString();
        }
        else
        {
            trinketCount.text = "";
            trinketText.text = "All trinkets collected!";
        }
    }

    private bool CheckVictory() 
    {
        return (trinketsCollected == trinketsToVictory);
    }

    public void ActivateCheckpoint(GameObject newCheckpoint) 
    {
        // Si había un checkpoint antiguo lo desactivamos.
        if (activeCheckpoint)
        {
            activeCheckpoint.GetComponent<Checkpoint>().Deactivate();
        }

        // Marcamos el checkpoint como activo.
        activeCheckpoint = newCheckpoint;
        activeCheckpoint.GetComponent<Checkpoint>().Activate();
    }

    public void MoveCameraToRespawn()
    {
        // Damos por hecho que el respawn es hijo de una habitación.
        Vector2 newPosition = activeCheckpoint.transform.parent.transform.position;
        MoveCamera(newPosition);
    }

    public void MoveCamera(Vector2 newPosition) 
    {
        Camera.main.transform.position = new Vector3(newPosition.x, newPosition.y, Camera.main.transform.position.z);
    }
}
