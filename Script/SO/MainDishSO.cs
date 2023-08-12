using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MainDishSO : ScriptableObject
{
    [SerializeField] public List<KitchenObjectSO> kitchenObjectSOList;

    [SerializeField] public string mainDishName;
}
