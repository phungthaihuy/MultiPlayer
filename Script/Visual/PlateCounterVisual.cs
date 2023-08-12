using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCounterVisual : MonoBehaviour
{
    [SerializeField] private PlatesCounter platesCounter;
    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;
    [SerializeField] private Transform counterTopPoint;

    private List<GameObject> plateGameObject;

    private void Awake()
    {
        plateGameObject = new List<GameObject>();
    }

    private void Start()
    {
        platesCounter.OnRemovePlate += PlatesCounter_OnRemovePlate;
        platesCounter.OnPlateVisualSpawn += PlatesCounter_OnPlateVisualSpawn;
    }

    private void PlatesCounter_OnPlateVisualSpawn(object sender, System.EventArgs e)
    {
        float plateTransformPosY = .1f;

        Transform plateTransform = Instantiate(plateKitchenObjectSO.prefab, counterTopPoint);
        plateTransform.localPosition = new Vector3(0, plateTransformPosY * plateGameObject.Count, 0);

        plateGameObject.Add(plateTransform.gameObject);
    }

    private void PlatesCounter_OnRemovePlate(object sender, System.EventArgs e)
    {
        GameObject plateGO = plateGameObject[plateGameObject.Count - 1];

        plateGameObject.Remove(plateGO);
        Destroy(plateGO);
    }
}
