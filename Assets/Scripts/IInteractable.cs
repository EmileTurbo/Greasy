using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact(PlayerInteraction player);
    public void DisableOutline();
    public void EnableOutline();

}
