using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrillVisual : MonoBehaviour
{
    [SerializeField] Grill grill;
    [SerializeField] GameObject particlesGameObject;
    [SerializeField] GameObject grillOnGameObject;

    private Dictionary<Transform, GameObject> activeParticles = new Dictionary<Transform, GameObject>();

    private void Start()
    {
        grill.OnItemFryingStarted += Grill_OnItemFryingStarted;
        grill.OnItemFryingStopped += Grill_OnItemFryingStopped;
        grill.OnPowerStateChanged += Grill_OnPowerStateChanged;
    }

    private void Grill_OnPowerStateChanged(bool obj)
    {
        if (obj)
        {
            grillOnGameObject.SetActive(true);
        }
        else
        {
            grillOnGameObject.SetActive(false);
        }
    }

    private void Grill_OnItemFryingStopped(Transform slot)
    {
        if (activeParticles.ContainsKey(slot))
        {
            Destroy(activeParticles[slot]);
            activeParticles.Remove(slot);
        }
    }

    private void Grill_OnItemFryingStarted(Transform slot)
    {
        if (!activeParticles.ContainsKey(slot))
        {
            GameObject particles = Instantiate(particlesGameObject, slot.position, Quaternion.identity, slot);
            activeParticles.Add(slot, particles);
        }
    }
}
