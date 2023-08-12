using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public interface IKitchenObjectParent
{
    public Transform GetKitchenObjectFollower();

    public KitchenObject GetKitchenObject();
    public void SetKitchenObject(KitchenObject kitchenObject);
    public void ClearKitchenObject();
    public bool HasKitchenObject();
    public NetworkObject GetNetworkObject();
}
