using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    [SerializeField]private ContainerCounter containerCounter;

    private const string OPEN_CLOSE = "OpenClose";

    private Animator ani;

    private void Start()
    {
        ani = GetComponent<Animator>();

        containerCounter.OnPlayerGrabbed += ContainerCounter_OnPlayerGrabbed;
    }

    private void ContainerCounter_OnPlayerGrabbed(object sender, System.EventArgs e)
    {
        ani.SetTrigger(OPEN_CLOSE);
    }
}
