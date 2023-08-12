using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField] private Player player;
    private Animator anim;
    private string IS_WALKING = "IsWalking";

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        anim.SetBool(IS_WALKING, player.IsWalking());
    }
}
