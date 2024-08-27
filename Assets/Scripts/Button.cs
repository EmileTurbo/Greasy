using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    private bool isOn;
    private Outline outline;

    public event EventHandler OnButtonOn;
    public event EventHandler OnButtonOff;

    private void Start()
    {
        outline = GetComponentInChildren<Outline>();
        DisableOutline();

        isOn = false;
    }

    public void Interact(PlayerInteraction player)
    {
        if (isOn)
        {
            OnButtonOff?.Invoke(this, EventArgs.Empty);
            Debug.Log("Its Off");
            isOn = false;
        }
        else
        {
            OnButtonOn?.Invoke(this, EventArgs.Empty);
            Debug.Log("Its On");
            isOn = true;
        }
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
