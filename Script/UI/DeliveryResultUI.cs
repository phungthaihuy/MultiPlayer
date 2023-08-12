using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryResultUI : MonoBehaviour
{
    [SerializeField] private Image backGroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messengerText;

    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failedSprite;

    private float showTimerMax;
    // Start is called before the first frame update
    void Start()
    {
        DeliveryManager.Instance.OnDeliverySuccess += Instance_OnDeliverySuccess;
        DeliveryManager.Instance.OnDeliveryFailed += Instance_OnDeliveryFailed;

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (backGroundImage.color == failedColor && iconImage.sprite == failedSprite && messengerText.text == "DELIVERY\nFAILED")
        {
            showTimerMax -= Time.deltaTime;
            if (showTimerMax <= 0)
            {
                gameObject.SetActive(false);
            }
        }
        if (backGroundImage.color == successColor && iconImage.sprite == successSprite && messengerText.text == "DELIVERY\nSUCCESS")
        {
            showTimerMax -= Time.deltaTime;
            if (showTimerMax <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    private void Instance_OnDeliveryFailed(object sender, System.EventArgs e)
    {
        showTimerMax = 1f;
        gameObject.SetActive(true);
        backGroundImage.color = failedColor;
        iconImage.sprite = failedSprite;
        messengerText.text = "DELIVERY\nFAILED";
    }

    private void Instance_OnDeliverySuccess(object sender, System.EventArgs e)
    {
        showTimerMax = 1f;
        gameObject.SetActive(true);
        backGroundImage.color = successColor;
        iconImage.sprite = successSprite;
        messengerText.text = "DELIVERY\nSUCCESS";
    }

    // Update is called once per frame
    
}
