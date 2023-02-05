using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : WeaponBase
{
    [SerializeField] KnifeCollider knifeCollider;
    private bool isLeftAttack;
    public override void WeaponBulletinit()
    {
        knifeCollider.Init(this);
        wantShootEF = bulletFireEffect != null;
    }
    public override void OnEnterPlayerState(PlayerState playerState)
    {
        switch(playerState)
        {
            case PlayerState.Shoot:
                if(isLeftAttack)
                {
                    OnLeftAttack();
                }
                else
                {
                    OnRightAttack();
                }
                break;
        }
    }

    public override void OnUpdatePlayerState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Move:
                // ¼ì²âÉä»÷
                if (canAttack && Input.GetMouseButton(0))
                {
                    isLeftAttack = true;
                    playerController.ChangePlayerState(PlayerState.Shoot);
                }
                if (canAttack && Input.GetMouseButton(1))
                {
                    isLeftAttack = false;
                    playerController.ChangePlayerState(PlayerState.Shoot);
                }
                break;
        }
    }
    protected override void OnLeftAttack()
    {
        PlayAudio(1);
        attackValue = 35;
        animator.SetTrigger("LightAttack");
    }
    private void OnRightAttack()
    {
        PlayAudio(1);
        attackValue = 50;
        animator.SetTrigger("HeavyAttack");
    }
    public void HitTarget(GameObject hitObj, Vector3 hitPoint)
    {
        PlayAudio(2);
        if (hitObj.CompareTag("Zombie"))
        {
            // ¹¥»÷Ð§¹û
            GameObject go = Instantiate(bulletPrefabs[1], hitPoint, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);

            // ½©Ê¬Âß¼­
            ZombieController zombieController = hitObj.GetComponent<ZombieController>();
            if (zombieController == null)
                zombieController = hitObj.GetComponentInParent<ZombieController>();
            zombieController.Hurt(attackValue);
        }
        else if (hitObj.gameObject != this.gameObject)
        {
            // ¹¥»÷Ð§¹û
            GameObject go = Instantiate(bulletPrefabs[0], hitPoint, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
        }
    }
    #region ¶¯»­ÊÂ¼þ
    public void StartAttack()
    {
        knifeCollider.EnableDamageCollider();
    }
    public void StopAttack()
    {
        knifeCollider.DisableDamageCollider();
    }
    #endregion
}
