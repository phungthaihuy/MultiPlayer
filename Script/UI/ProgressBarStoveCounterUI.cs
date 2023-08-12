using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarStoveCounterUI : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private Image bar;
    // Start is called before the first frame update
    void Start()
    {
        stoveCounter.OnProgressBarChanged += StoveCounter_OnProgressBarChanged;

        bar.fillAmount = 0f;
        Hide();
    }

    private void StoveCounter_OnProgressBarChanged(object sender, StoveCounter.OnProgressBarChangedEventArgs e)
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
