using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    [SerializeField] private CuttingCounter cuttingCounter;

    Animator ani;
    private const string CUT = "Cut";

    // Start is called before the first frame update
    void Start()
    {
        cuttingCounter.OnCut += CuttingCounter_OnCut;

        ani = GetComponent<Animator>();
    }

    private void CuttingCounter_OnCut(object sender, System.EventArgs e)
    {
        ani.SetTrigger(CUT);   
    }
}
