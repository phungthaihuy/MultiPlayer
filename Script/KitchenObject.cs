using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class KitchenObject : NetworkBehaviour
{
    private FollowTransform followTransform;

    [SerializeField] KitchenObjectSO kitchenObjectSO;
    private IKitchenObjectParent kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    protected virtual void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenobjectParentNetworkObjectReference)
    {
        SetKitchenObjectParentClientRpc(kitchenobjectParentNetworkObjectReference);
    }
    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenobjectParentNetworkObjectReference)
    {
        if (this.kitchenObjectParent != null)
        {
            this.kitchenObjectParent.ClearKitchenObject(); //xoa hon kitchenObject con ton tai o kitchenObjectParent cu~. 
        }

        kitchenobjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        this.kitchenObjectParent = kitchenObjectParent;
         
        kitchenObjectParent.SetKitchenObject(this); // chi spawn 1 lan

        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollower());
        //transform.parent = kitchenObjectParent.GetKitchenObjectFollower();
        //transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public void DestroyKitchenObject()
    {
        Destroy(gameObject);
    }
    public void ClearKitchenObjectOnParent()
    {
        kitchenObjectParent.ClearKitchenObject();
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject)
        {
            plateKitchenObject = this as PlateKitchenObject;

            return true;
        }
        else
        {
            plateKitchenObject = null;

            return false;
        }
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        KitchenGameMultiPlayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
    }

    public static void DestroyKitchenObjectMultiPlayer(KitchenObject kitchenObject)
    {
        KitchenGameMultiPlayer.Instance.DestroyKitchenObject(kitchenObject);
    }
}
