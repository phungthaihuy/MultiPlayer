using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class CuttingCounter : BaseCounter
{
    [SerializeField] private CuttingKitchenObjectSO[] cuttingKitchenObjectSOArray;

    public event EventHandler OnCut;
    public event EventHandler<OnProgressBarChangedEventArgs> OnProgressBarChanged;
    public class OnProgressBarChangedEventArgs : EventArgs
    {
        public float progressBarNomalized;
    }

    private float cuttingProgress = 0f;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject() && HasKitchenObjectCanCut(player.GetKitchenObject().GetKitchenObjectSO()))
            {
                InteractLogicPlaceObjectOnCuttingCounterServerRpc();

                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
            else
            {
                //player k giu KO
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                //cuttingProgress = 0f;

                GetKitchenObject().SetKitchenObjectParent(player);
            }
            else
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddingredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroyKitchenObject();
                    }
                }
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCuttingCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCuttingCounterClientRpc();
    }
    [ClientRpc]
    private void InteractLogicPlaceObjectOnCuttingCounterClientRpc()
    {
        cuttingProgress = 0f;
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && HasKitchenObjectCanCut(GetKitchenObject().GetKitchenObjectSO()))
        {
            CuttingObjectVisualServerRpc();
            CuttingObjectServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void CuttingObjectServerRpc()
    {
        CuttingObjectClientRpc();
    }
    [ClientRpc]
    private void CuttingObjectClientRpc()
    {
        if (!IsOwner)
        {
            return;
        }
        

        KitchenObject kitchenObject = GetKitchenObject();
        CuttingKitchenObjectSO cuttingKitchenObjectSO = GetCuttingKitchenObjectSO(kitchenObject.GetKitchenObjectSO());

        if (cuttingProgress >= cuttingKitchenObjectSO.maxCuttingProgress)
        {
            KitchenObjectSO outputKitchenObjectSO = GetOutPutKitchenObjectSO(kitchenObject.GetKitchenObjectSO());

            KitchenObject.DestroyKitchenObjectMultiPlayer(kitchenObject);
            //kitchenObject.ClearKitchenObjectOnParent();
            //kitchenObject.DestroyKitchenObject();

            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
            //Transform kitchenObjectTransform = Instantiate(outputKitchenObjectSO.prefab);
            //kitchenObjectTransform.localPosition = Vector3.zero;

            //kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void CuttingObjectVisualServerRpc()
    {
        

        CuttingObjectVIsualClientRpc();
    }
    [ClientRpc]
    private void CuttingObjectVIsualClientRpc()
    {
        cuttingProgress++;

        CuttingKitchenObjectSO cuttingKitchenObjectSO = GetCuttingKitchenObjectSO(GetKitchenObject().GetKitchenObjectSO());
        OnCut?.Invoke(this, EventArgs.Empty);
        OnProgressBarChanged?.Invoke(this, new OnProgressBarChangedEventArgs
        {
            progressBarNomalized = cuttingProgress / cuttingKitchenObjectSO.maxCuttingProgress
        });
    }

    private CuttingKitchenObjectSO GetCuttingKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        foreach (CuttingKitchenObjectSO cuttingKitchenObjectSO in cuttingKitchenObjectSOArray)
        {
            if (cuttingKitchenObjectSO.kitchenObjectInput == kitchenObjectSO)
            {
                return cuttingKitchenObjectSO;
            }
        }
        return null;
    }

    private bool HasKitchenObjectCanCut(KitchenObjectSO kitchenObjectSO)
    {
        CuttingKitchenObjectSO cuttingKitchenObjectSO = GetCuttingKitchenObjectSO(kitchenObjectSO);

        return cuttingKitchenObjectSO != null;
    }

    private KitchenObjectSO GetOutPutKitchenObjectSO(KitchenObjectSO kitchenObjectSO) {
        CuttingKitchenObjectSO cuttingKitchenObjectSO = GetCuttingKitchenObjectSO(kitchenObjectSO);

        if (cuttingKitchenObjectSO != null)
        {
            return cuttingKitchenObjectSO.kitchenObjectOutput;
        }
        return null;
    }
}
