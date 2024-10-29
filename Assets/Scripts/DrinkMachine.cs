using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Grill;
using static UnityEditor.Progress;

public class DrinkMachine : MonoBehaviour, IInteractable, IItemParent
{
    [SerializeField] private List<Transform> spotsList;
    [SerializeField] private ItemSO emptyCupSO;
    [SerializeField] private ItemSO fountainDrinkSO;
    [SerializeField] private float timeToFillCup = 8f;
    private List<Transform> availableSpotsList;
    private Dictionary<Transform, Item> occupiedSpots;
    private Dictionary<Transform, GameObject> sodaGameObjects;
    private Dictionary<Transform, float> spotTimers;
    private Outline outline;
    [SerializeField] private int currentLevel;
    [SerializeField] private Transform delicieuxSodaPrefab;

    private void Start()
    {
        outline = GetComponentInChildren<Outline>();
        DisableOutline();
        occupiedSpots = new Dictionary<Transform, Item>();
        sodaGameObjects = new Dictionary<Transform, GameObject>();
        availableSpotsList = new List<Transform>();
        spotTimers = new Dictionary<Transform, float>();
        SetAvailableSpotsByLevel();
    }

    private void Update()
    {
        foreach (var spot in availableSpotsList)
        {
            if (occupiedSpots.ContainsKey(spot)) // This spot is occupied by something
            {
                if (occupiedSpots[spot].GetItemSO() == emptyCupSO) // The item is an empty cup
                {
                    spotTimers[spot] += Time.deltaTime;

                    if (spotTimers[spot] >= timeToFillCup)
                    {
                        SpawnFountainDrink(spot);
                        spotTimers[spot] = 0f;
                    }
                }
                else if (sodaGameObjects.ContainsKey(spot) && occupiedSpots[spot].GetItemSO() != emptyCupSO)
                {
                    GameObject.Destroy(sodaGameObjects[spot]);
                    sodaGameObjects.Remove(spot);
                }
            }
            else if (sodaGameObjects.ContainsKey(spot) && !occupiedSpots.ContainsKey(spot))
            {
                GameObject.Destroy(sodaGameObjects[spot]);
                sodaGameObjects.Remove(spot);
            }
        }
    }

    public void Interact(PlayerInteraction player)
    {
        if (player != null && availableSpotsList != null)
        {
            if (player.HasItem()) // Player has an item
            {
                if (player.GetItem(player.GetItemFollowTransform()).GetItemSO() == emptyCupSO) // Player has an empty cup
                {
                    if (!HasItem()) // Is not full
                    {
                        Transform spot = GetRandomizedAvailableSpot();
                        Item playerItem = player.GetItem(player.GetItemFollowTransform());
                        PlaceEmptyCup(spot, playerItem);
                    }
                }
            }
        }
    }

    private Transform GetRandomizedAvailableSpot()
    {
        List<Transform> possibleSpots = new List<Transform>();

        foreach (Transform spot in availableSpotsList)
        {
            if (!occupiedSpots.ContainsKey(spot))
            {
                possibleSpots.Add(spot);
            }
        }

        if (possibleSpots.Count > 0)
        {
            Transform randomSpot = possibleSpots[Random.Range(0, possibleSpots.Count)];

            return randomSpot;
        }
        else
        {
            return null;
        }
    }

    private void SpawnFountainDrink(Transform spot)
    {
        GameObject.Destroy(sodaGameObjects[spot]);
        sodaGameObjects.Remove(spot);

        occupiedSpots[spot].DestroySelf();

        Item fountainDrink = Item.SpawnItem(fountainDrinkSO, this);
        fountainDrink.transform.parent = spot;
        fountainDrink.transform.localPosition = Vector3.zero;
        fountainDrink.transform.localRotation = Quaternion.identity;
    }

    private void PlaceEmptyCup(Transform spot, Item emptyCup)
    {
        emptyCup.SetItemParent(this, spot);

        occupiedSpots[spot] = emptyCup;
        spotTimers[spot] = 0f; // Start tracking time for this spot

        // Instantiate and position the soda
        Transform sodaPrefab = Instantiate(delicieuxSodaPrefab);
        sodaGameObjects[spot] = sodaPrefab.gameObject;
        sodaPrefab.parent = spot;
        sodaPrefab.transform.localPosition = new Vector3(0f, 0.55f, 0f);  // Soda is slightly above the base of the cup
        sodaPrefab.transform.localRotation = Quaternion.identity;

        // Move the soda downward over time
        StartCoroutine(MoveSoda(sodaPrefab));
    }

    private IEnumerator MoveSoda(Transform soda)
    {
        Vector3 startPos = soda.localPosition;
        Vector3 targetPos = new Vector3(0f, 0.28f, 0f);  // Target position within the cup
        float duration = 0.2f;  // Time to move the soda in seconds
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            soda.localPosition = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        // Ensure the soda reaches the final position
        soda.localPosition = targetPos;
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

    private void SetAvailableSpotsByLevel()
    {
        if (spotsList != null)
        {
            for (int i = 0; i < currentLevel; i++)
            {
                if (spotsList[i] != null)
                {
                    availableSpotsList.Add(spotsList[i]);
                }
                else
                {
                    break;
                }
            }
        }
    }

    public Transform GetItemFollowTransform()
    {
        foreach (var slot in availableSpotsList)
        {
            if (!IsSlotOccupied(slot))
            {
                return slot;
            }
        }
        return null;
    }

    public void SetItem(Item item, Transform slot)
    {      
       occupiedSpots[slot] = item;      
    }

    public Item GetItem(Transform slot)
    {
        if (occupiedSpots.ContainsKey(slot))
        {
            return occupiedSpots[slot];
        }
        return null;
    }

    public void ClearItem(Item item)
    {
        Transform slotToRemove = null;
        foreach (var pair in occupiedSpots)
        {
            if (pair.Value == item)
            {
                slotToRemove = pair.Key;
                break;
            }
        }
   
        if (slotToRemove != null)
        {
            occupiedSpots.Remove(slotToRemove);
        }
    }

    public bool HasItem()
    {
        return occupiedSpots.Count >= availableSpotsList.Count;
    }

    public bool IsSlotOccupied(Transform slot)
    {
        return occupiedSpots.ContainsKey(slot);
    }

    public bool HasMultipleSlots()
    {
        return true;
    }

    public bool IsPlayer()
    {
        return false;
    }

    public Transform GetSlotForItem(Item item)
    {
        foreach (var pair in occupiedSpots)
        {
            if (pair.Value == item)
            {
                return pair.Key;
            }
        }
        return null; // Slot not found
    }
}
