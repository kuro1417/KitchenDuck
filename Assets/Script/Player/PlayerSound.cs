using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private Player player;
    private float footstepTimer;
    private float footstepTimerMax=.1f;
    private float dashSoundTimer;
    private float dashSoundTimerMax = .16f;
    private void Awake()
    {
        player= GetComponent<Player>();
    }
    private void Update()
    {
        footstepTimer -= Time.deltaTime;
        if(footstepTimer < 0 ) {
            footstepTimer = footstepTimerMax;

            if (player.IsWalking())
            {
            float footstepVolume = 5f;
            SoundManager.Instance.PlayFootstepSound(player.transform.position, footstepVolume);
            }
        }
        dashSoundTimer-= Time.deltaTime;
        if(dashSoundTimer < 0 )
        {
            dashSoundTimer = dashSoundTimerMax;

            if (player.IsDashing())
            {
                float dashVolume = 6f;
                SoundManager.Instance.PlayDashSound(player.transform.position, dashVolume);
            }
        }
    }
}
