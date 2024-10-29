using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CupDispenser : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemSO itemSO;
    [SerializeField] private GameObject cupVisual;
    private Outline outline;

    private void Start()
    {
        outline = GetComponentInChildren<Outline>();
        DisableOutline();
    }

    public void Interact(PlayerInteraction player)
    {
        if (player != null)
        {
            if (!player.HasItem())
            {
                if (itemSO.prefab != null)
                {
                    // Apply a random Y rotation to the cupVisual
                    float randomYRotation = Random.Range(0f, 360f); // Get a random rotation between 0 and 360 degrees
                    cupVisual.transform.localRotation = Quaternion.Euler(0f, randomYRotation, 0f); // Apply the random Y rotation

                    Item.SpawnItem(itemSO, player);                   
                }
            }
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
