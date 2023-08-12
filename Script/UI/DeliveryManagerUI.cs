using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    // Start is called before the first frame update
    void Start()
    {
        DeliveryManager.Instance.OnSpawnMainDish += Instance_OnSpawnMainDish;
        DeliveryManager.Instance.OnRecipeComplete += Instance_OnRecipeComplete;

        recipeTemplate.gameObject.SetActive(false);
    }

    private void Instance_OnRecipeComplete(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void Instance_OnSpawnMainDish(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (KitchenGameManager.Instance.IsGamePlaying())
        {
            foreach (Transform item in container)
            {
                if (item == recipeTemplate) continue;
                else
                {
                    Destroy(item.gameObject);
                }
            }
            foreach (MainDishSO mainDishSO in DeliveryManager.Instance.GetMainDishSOList())
            {
                Transform mainDishTransform = Instantiate(recipeTemplate, container);
                mainDishTransform.gameObject.SetActive(true);

                DeliveryManagerSingleUI.Instance.SetKitChenObjectSOSpriteInMainDishSO(mainDishSO);
                //SetKitChenObjectSOSpriteInMainDishSO
            }
        }
    }
}
