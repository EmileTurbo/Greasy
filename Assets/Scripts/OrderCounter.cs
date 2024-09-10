using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Grill;

public class OrderCounter : MonoBehaviour
{
    [SerializeField] private List<Transform> waitingLinePointsList;
    [SerializeField] private Transform spawnLocation;
    [SerializeField] private Transform customerNPCPrefab;
    [SerializeField] private float spawnTimer = 15f;
    public List<RecipeSO> recipes;
    private Dictionary<Transform, CustomerNPC> customers = new Dictionary<Transform, CustomerNPC>();
    private float timer = 0f;


    private void Update()
    {
        UpdateCustomerNPCTargetWaitingSpot();
        SpawnCustomerNPC();
        HandleCustomerAtCounter();
    }

    private Transform GetWaitingLinePoint()
    {
        if (customers.Count >= waitingLinePointsList.Count)
        {
            return null;
        }

        foreach (var spot in waitingLinePointsList)
        {
            if (!customers.ContainsKey(spot))
            {
                return spot;
            }
        }

        return null;
    }

    private void SpawnCustomerNPC()
    {
        if (customers.Count < waitingLinePointsList.Count)
        {
            timer += Time.deltaTime;

            if (timer >= spawnTimer)
            {
                Debug.Log("spawn");
                Transform waitingSpot = GetWaitingLinePoint();

                Transform npcTransform = Instantiate(customerNPCPrefab);
                npcTransform.position = spawnLocation.position;
                npcTransform.rotation = Quaternion.identity;

                CustomerNPC customerNPC = npcTransform.GetComponent<CustomerNPC>();
                customerNPC.SetTargetWaitingSpot(waitingSpot);
                customers[waitingSpot] = customerNPC;

                timer = 0f;
            }
            
        }
        else
        {
            timer = 0f;
        }
    }

    private void UpdateCustomerNPCTargetWaitingSpot()
    {
        List<Transform> availableSpots = new List<Transform>();

        // Find all available spots in order
        foreach (Transform spot in waitingLinePointsList)
        {
            if (!customers.ContainsKey(spot))
            {
                availableSpots.Add(spot);
            }
        }

        // Reassign NPCs to the nearest available spots
        foreach (Transform spot in waitingLinePointsList)
        {
            if (customers.ContainsKey(spot))
            {
                CustomerNPC customerNPC = customers[spot];
                Transform newSpot = availableSpots.Find(s => waitingLinePointsList.IndexOf(s) < waitingLinePointsList.IndexOf(spot));

                if (newSpot != null)
                {
                    customerNPC.SetTargetWaitingSpot(newSpot);
                    customers.Remove(spot);
                    customers[newSpot] = customerNPC;
                    availableSpots.Remove(newSpot);
                }
            }
        }
    }

    private void HandleCustomerAtCounter()
    {
        foreach (CustomerNPC customerNPC in customers.Values)
        {
            if (customerNPC.GetTargetWaitingSpot() == waitingLinePointsList[0])
            {
                if (customerNPC.GetCurrentTransform() != null)
                {
                    if (Vector3.Distance(customerNPC.GetCurrentTransform().position, waitingLinePointsList[0].position) < 0.1f)
                    {
                        customerNPC.isReadyToOrder = true;
                    }
                    else
                    {
                        customerNPC.isReadyToOrder = false;
                    }
                }
            }
            else
            {
                customerNPC.isReadyToOrder = false;
            }
        }
    }

    public void RemoveCustomerNPCFromDictionary(CustomerNPC customerNPC)
    {
        Transform targetWaitingSpot = customerNPC.GetTargetWaitingSpot();
        customers.Remove(targetWaitingSpot, out customerNPC);
        
    }

    
}
