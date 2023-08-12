using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconUI : MonoBehaviour
{
    [SerializeField] private PlateKitchenObject platesKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }
    private void Start()
    {
        platesKitchenObject.OnIngredientAdd += PlatesKitchenObject_OnIngredientAdd;
    }

    private void PlatesKitchenObject_OnIngredientAdd(object sender, PlateKitchenObject.OnIngredientAddEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in this.transform)
        {
            if (child.transform == iconTemplate)
            {
                continue;
            }
            child.gameObject.SetActive(false);
        }
        foreach (KitchenObjectSO kitchenObjectSO in platesKitchenObject.GetKitchenObjectSOList())
        {
            Transform iconTransform = Instantiate(iconTemplate, this.transform);
            iconTransform.gameObject.SetActive(true);

            iconTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectSOImage(kitchenObjectSO);
        }
        
    }
}
