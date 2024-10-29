using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagCounter : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform bagPrefab;
    [SerializeField] private Transform bagSpawnPoint;
    [SerializeField] private Button button;
    private Bag bag;
    private bool hasBagSpawned;

    private void Start()
    {
        hasBagSpawned = false;

        button.OnButtonOn += Button_OnButtonOn;
    }

    private void Button_OnButtonOn(object sender, EventArgs e)
    {
        if (hasBagSpawned && bag != null)
        {
            if (!bag.CheckBagStatus()) // Bag is open
            {
                bag.CloseBag();
            }
        }
    }

    public void Interact(PlayerInteraction player)
    {
        if (hasBagSpawned) // There is a bag on the counter
        {

        }
        else // There is NO bag on the counter
        {
            SpawnBag();
        }
    }

    public void DisableOutline()
    {

    }

    public void EnableOutline()
    {

    }

    private void SpawnBag()
    {
        if (!hasBagSpawned)
        {
            Transform bagTransform = Instantiate(bagPrefab);

            bagTransform.transform.parent = bagSpawnPoint;
            bagTransform.transform.localRotation = Quaternion.identity;
            bagTransform.transform.localPosition = Vector3.zero;

            bag = bagTransform.GetComponent<Bag>();
            hasBagSpawned = true;
        }
    }
}
