using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Color color;
    public Color activeColor;

    private SpriteRenderer spriteRenderer;
    [HideInInspector]
    public bool isActive;

    void Start()
    {
        isActive = false;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
    }

    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            spriteRenderer.color = activeColor;
        }
    }

    public void Deactivate()
    {
        if(isActive)
        {
            isActive = false;
            spriteRenderer.color = color;
        }
    }
}
