using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject sizzlingParticles;
    [SerializeField] private GameObject stoveOnVisual;

    private bool showVisual;
    // Start is called before the first frame update
    void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        showVisual = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fired;

        sizzlingParticles.SetActive(showVisual);
        stoveOnVisual.SetActive(showVisual);
    }
}
