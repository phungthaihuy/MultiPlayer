using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class _MenuListSO : ScriptableObject
{
    [SerializeField] public List<MainDishSO> mainDishSOList;
}
