using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_GameObject 
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<KitchenObjectSO_GameObject> kitchenObjectSO_GameObjectsList;

    // Start is called before the first frame update
    void Start()
    {
        plateKitchenObject.OnIngredientAdd += PlateKitchenObject_OnIngredientAdd;
        foreach ( KitchenObjectSO_GameObject item in kitchenObjectSO_GameObjectsList)
        {
            item.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObject_OnIngredientAdd(object sender, PlateKitchenObject.OnIngredientAddEventArgs e)
    {
        foreach (KitchenObjectSO_GameObject item in kitchenObjectSO_GameObjectsList)
        {
            if (item.kitchenObjectSO == e.kitchenObjectSO)
            {
                item.gameObject.SetActive(true);
            }
        }
    }
}
