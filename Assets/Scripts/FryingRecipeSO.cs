using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class FryingRecipeSO : ScriptableObject
{
    public ItemSO input;
    public ItemSO output;
    public float fryingTimerMax;
}
