using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryManagerSingleUI : MonoBehaviour
{
    public static DeliveryManagerSingleUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private Transform iconContainer;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {

        iconTemplate.gameObject.SetActive(false);
    }

    public void SetKitChenObjectSOSpriteInMainDishSO(MainDishSO mainDishSO)
    {
        recipeNameText.text = mainDishSO.mainDishName;

        //foreach (Transform item in iconContainer)
        //{
        //    if (item == iconTemplate) continue;
        //    else
        //    { 
        //        Destroy(item.gameObject);
        //    }
        //}
        foreach (KitchenObjectSO item in mainDishSO.kitchenObjectSOList)
        {
            Transform kitchenObjectTransform = Instantiate(iconTemplate, iconContainer);
            kitchenObjectTransform.gameObject.SetActive(true);

            kitchenObjectTransform.GetComponent<Image>().sprite = item.sprite;
        }
    }
}
