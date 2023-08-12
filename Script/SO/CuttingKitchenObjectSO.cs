using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CuttingKitchenObjectSO : ScriptableObject
{
    [SerializeField] public KitchenObjectSO kitchenObjectInput;
    [SerializeField] public KitchenObjectSO kitchenObjectOutput;
    [SerializeField] public float maxCuttingProgress;
}
