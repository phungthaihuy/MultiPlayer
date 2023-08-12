using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlatesCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    public event EventHandler OnPlateVisualSpawn;
    public event EventHandler OnRemovePlate;

    private float plateSpawnTimer;
    private float plateSpawnTimerMax = 4f;

    private int plateCount;
    private int plateCountMax = 4;

    private void Update()
    {
        plateSpawnTimer += Time.deltaTime;
        if (plateSpawnTimer > plateSpawnTimerMax)
        {
            plateSpawnTimer = 0f;
            if (plateCount < plateCountMax)
            {
                SpawnPlatesServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SpawnPlatesServerRpc()
    {
        SpawnPlatesCLientRpc();
    }
    [ClientRpc]
    private void SpawnPlatesCLientRpc()
    {
        plateCount++;

        OnPlateVisualSpawn?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player)
    {
        if (plateCount > 0)
        {
            KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
            //Transform plateTransform = Instantiate(plateKitchenObjectSO.prefab, GetKitchenObjectFollower());
            //plateTransform.localPosition = Vector3.zero;

            //plateTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player);

            InteractLogicServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        plateCount--;
        
        OnRemovePlate?.Invoke(this, EventArgs.Empty);
    }
}
