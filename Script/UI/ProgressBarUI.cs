using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] CuttingCounter cuttingCounter;
    [SerializeField] Image bar;

    private void Start()
    {
        cuttingCounter.OnProgressBarChanged += CuttingCounter_OnProgressBarChanged;

        bar.fillAmount = 0f;
        Hide();
    }

    private void CuttingCounter_OnProgressBarChanged(object sender, CuttingCounter.OnProgressBarChangedEventArgs e)
    {
        bar.fillAmount = e.progressBarNomalized;

        if (bar.fillAmount == 0 || bar.fillAmount == 1)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
