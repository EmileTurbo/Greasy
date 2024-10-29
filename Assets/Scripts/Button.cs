using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    private Outline outline;

    public event EventHandler OnButtonOn;

    private void Start()
    {
        outline = GetComponentInChildren<Outline>();
        DisableOutline();
    }

    public void Interact(PlayerInteraction player)
    {
        OnButtonOn?.Invoke(this, EventArgs.Empty);
        Debug.Log("Its clicked");
    }

    public void DisableOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void EnableOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }
}
