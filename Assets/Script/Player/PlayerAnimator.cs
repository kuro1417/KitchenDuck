using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PlayerAnimator : NetworkBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_DASHING = "IsDashing";
    //get Player from class Player
    [SerializeField] private Player player;
    private Animator animator;
    private void Awake()
    {
       animator =GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner) { 
            return; 
        }
        animator.SetBool(IS_WALKING, player.IsWalking());
        animator.SetBool(IS_DASHING, player.IsDashing());
    }
}
