using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class DeliveryManager : NetworkBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private _MenuListSO menuListSO;

    public event EventHandler OnSpawnMainDish;
    public event EventHandler OnDeliveryFailed;
    public event EventHandler OnDeliverySuccess;
    public event EventHandler OnRecipeComplete;

    private List<MainDishSO> waitingMainDishSOList;
    private float spawnTimer = 4f;
    private float spawnTimerMax = 4f;
    private int spawnMainDishMax = 4;
    private int successRecipeAmount;


    private void Awake()
    {
        Instance = this;

        waitingMainDishSOList = new List<MainDishSO>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
        {
            return;
        }


        spawnTimer -= Time.deltaTime;
        if (spawnTimer < 0f)
        {
            spawnTimer = spawnTimerMax;
            if (waitingMainDishSOList.Count < spawnMainDishMax)
            {
                int mainDishSOIndex = UnityEngine.Random.Range(0, menuListSO.mainDishSOList.Count);
               

                SpawnNewMainDishClientRpc(mainDishSOIndex);
            }
            
        }
    }

    [ClientRpc]
    private void SpawnNewMainDishClientRpc(int mainDishSOIndex)
    {
        MainDishSO mainDishSpawn = menuListSO.mainDishSOList[mainDishSOIndex];
        waitingMainDishSOList.Add(mainDishSpawn);

        OnSpawnMainDish?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingMainDishSOList.Count; i++)
        {
            MainDishSO mainDishSO = waitingMainDishSOList[i]; 
            if (mainDishSO.kitchenObjectSOList.Count == plateKitchenObject.GetKitchenObjectSOList().Count)
            {
                bool mainDishKitchenObjectSOMatchesPlateKOSO = true;
                foreach (KitchenObjectSO mainDishKitchenObjectSO in mainDishSO.kitchenObjectSOList)
                {
                    bool ingredientFound = true; 
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList())
                    {
                        if (plateKitchenObjectSO == mainDishKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (ingredientFound == false)
                    {
                        mainDishKitchenObjectSOMatchesPlateKOSO = false;
                    }
                }
                if (mainDishKitchenObjectSOMatchesPlateKOSO)
                {
                    OnDeliveryCorrectRecipeServerRpc(i);
                    return;
                }
            }
        }
        OnDeliveryIncorrectRecipeServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnDeliveryIncorrectRecipeServerRpc()
    {
        OnDeliveryIncorrectRecipeClientRpc();
    }
    [ClientRpc]
    private void OnDeliveryIncorrectRecipeClientRpc()
    {
        OnDeliveryFailed?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnDeliveryCorrectRecipeServerRpc(int waitingMainDishSOIndex)
    {
        OnDeliveryCorrectRecipeClientRpc(waitingMainDishSOIndex);
    }
    [ClientRpc]
    private void OnDeliveryCorrectRecipeClientRpc(int waitingMainDishSOIndex)
    {
        waitingMainDishSOList.RemoveAt(waitingMainDishSOIndex);

        successRecipeAmount++;
        OnDeliverySuccess?.Invoke(this, EventArgs.Empty);
        OnRecipeComplete?.Invoke(this, EventArgs.Empty);
    }

    public List<MainDishSO> GetMainDishSOList()
    {
        return waitingMainDishSOList;
    } 

    public int GetSuccessRecipeAmount()
    {
        return successRecipeAmount;
    }
}
