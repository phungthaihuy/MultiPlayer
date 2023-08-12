using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlateKitchenObject : KitchenObject
{
    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSO;

    public event EventHandler<OnIngredientAddEventArgs> OnIngredientAdd;
    public class OnIngredientAddEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObjectSO;
    }

    private List<KitchenObjectSO> kitchenObjectSOList;

    protected override void Awake()
    {
        base.Awake();
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddingredient (KitchenObjectSO kitchenObjectSO)
    {
        if (!validKitchenObjectSO.Contains(kitchenObjectSO))
        {
            return false;
        }

        if (kitchenObjectSOList.Contains(kitchenObjectSO))
        {
            return false;
        }
        else
        {
            AddIngredientServerRpc(
                KitchenGameMultiPlayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO)
                );

            //kitchenObjectSOList.Add(kitchenObjectSO);
            //OnIngredientAdd?.Invoke(this, new OnIngredientAddEventArgs
            //{
            //    kitchenObjectSO = kitchenObjectSO
            //});

            return true;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }
    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = KitchenGameMultiPlayer.Instance.GetKitchenObjectSOFormIndex(kitchenObjectSOIndex);

        kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdd?.Invoke(this, new OnIngredientAddEventArgs
        {
            kitchenObjectSO = kitchenObjectSO
        });
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList()
    {
        return kitchenObjectSOList;
    }
}
