using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BurnedKitchenObjectSO : ScriptableObject
{
    [SerializeField] public KitchenObjectSO input;
    [SerializeField] public KitchenObjectSO output;
    [SerializeField] public float maxBurnedTimer;
}
