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
        if (activeCheckpoint)
        {
            if (GameObject.ReferenceEquals(newCheckpoint, activeCheckpoint))
            { 
                return;
            }
            activeCheckpoint.GetComponent<Checkpoint>().Deactivate();
        }

        Debug.Log("Activando nuevo checkpoint");

        activeCheckpoint = newCheckpoint;
        activeCheckpoint.GetComponent<Checkpoint>().Activate();
    }

    public void MoveCameraToRespawn()
    {
        Vector2 newPosition = activeCheckpoint.transform.parent.transform.position;
        MoveCamera(newPosition);
    }

    public void MoveCamera(Vector2 newPosition) 
    {
        Camera.main.transform.position = new Vector3(newPosition.x, newPosition.y, Camera.main.transform.position.z);
    }
}
