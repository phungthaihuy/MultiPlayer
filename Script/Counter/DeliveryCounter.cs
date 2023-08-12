using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    KitchenObject.DestroyKitchenObjectMultiPlayer(player.GetKitchenObject());
                    //player.GetKitchenObject().DestroyKitchenObject();

                    DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);
                }
            }
        }
    }
}
