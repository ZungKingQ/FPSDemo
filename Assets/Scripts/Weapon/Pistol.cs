using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : WeaponBase
{
    public override void OnEnterPlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Move:
                
                break;
            case PlayerState.Shoot:
                OnLeftAttack();
                break;
            case PlayerState.Reload:
                PlayAudio(2);
                animator.SetBool("Reload", true);
                break;
        }
    }

    public override void OnUpdatePlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Move:
                // »»×Óµ¯
                if(Input.GetKeyDown(KeyCode.R))
                {
                    Reloading();
                }
                // ¼ì²âÉä»÷
                if(canAttack && currentBulletNum > 0 && Input.GetMouseButton(0))
                {
                    playerController.ChangePlayerState(PlayerState.Shoot);
                }
                break;
        }
    }
    

}
