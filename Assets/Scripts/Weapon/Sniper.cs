using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : WeaponBase
{
    [SerializeField] GameObject scope;
    [SerializeField] GameObject[] renders;
    [SerializeField] GameObject weaponCamera;
    private bool isAim;
    public override void OnEnterPlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Shoot:
                if(isAim)
                {
                    StopAim();
                }
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
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Reloading();
                }
                // ¼ì²âÉä»÷
                if (canAttack && currentBulletNum > 0 && Input.GetMouseButtonDown(0))
                {
                    playerController.ChangePlayerState(PlayerState.Shoot);
                }
                // ¼ì²â¿ª¾µ
                if (canAttack && Input.GetMouseButtonDown(1))
                {
                    isAim = !isAim;
                    if(isAim)
                    {
                        StartAim();
                    }
                    else
                    {
                        StopAim();
                    }
                }
                break;
        }
    }
    private void StartAim()
    {
        weaponCamera.SetActive(false);
        animator.SetBool("Aim", true);
        wantShootEF = false;
    }
    private void StopAim()
    {
        weaponCamera.SetActive(true);
        animator.SetBool("Aim", false);
        wantShootEF = true;
        scope.SetActive(false);
        // »Ö¸´ÉãÏñ»úµÄ¾àÀë
        playerController.SetCameraFOV(60);
    }
    #region ¶¯»­ÊÂ¼þ
    private void ShootEnd()
    {
        if (bulletFireEffect != null)
            bulletFireEffect.SetActive(false);
        if (playerController.playerState == PlayerState.Shoot)
        {
            playerController.ChangePlayerState(PlayerState.Move);
        }
    }
    private void StartLoad()
    {
        PlayAudio(3);
    }
    private void AimOver()
    {
        StartCoroutine(DoAim());
    }
    IEnumerator DoAim()
    {
        weaponCamera.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        scope.SetActive(true);
        // ÉãÏñ»úÀ­½ü
        playerController.SetCameraFOV(30);
    }
    #endregion
}
