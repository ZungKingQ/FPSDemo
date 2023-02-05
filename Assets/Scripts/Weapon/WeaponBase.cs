using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器基类
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    public PlayerController playerController;

    [SerializeField] protected Animator animator;
    [SerializeField] AudioSource audioSource;

    #region Bullet
    protected int currentBulletNum;
    public int currentMaxBulletNum;
    protected int spareBulletNum;
    public int spareMaxBulletNum;
    #endregion

    #region ShootParas
    public int attackValue;
    public bool wantCrosshair;
    public bool wantBullet;
    public bool wantRecoil;
    public float recoilStrength;
    public bool canThroughWall;
    protected bool canAttack;
    protected bool canReload;
    protected bool wantShootEF;
    #endregion

    #region Effect
    public AudioClip[] audioClips;
    public GameObject[] bulletPrefabs;
    public GameObject bulletFireEffect;
    #endregion
    private Action onExitOver;

    /// <summary>
    /// 切换角色状态时执行
    /// </summary>
    /// <param name="playerState"></param>
    public abstract void OnEnterPlayerState(PlayerState playerState);
    /// <summary>
    /// 角色不同状态时执行
    /// </summary>
    /// <param name="playerState"></param>
    public abstract void OnUpdatePlayerState(PlayerState playerState);
    /// <summary>
    /// 初始化子弹数目
    /// </summary>
    public virtual void WeaponBulletinit()
    {
            currentBulletNum = currentMaxBulletNum;
            spareBulletNum = spareMaxBulletNum;
    }
    /// <summary>
    /// 切换武器时调用
    /// </summary>
    public virtual void Enter()
    {
        // 唤醒武器
        this.gameObject.SetActive(true);
        canAttack = false;
        playerController.ChangePlayerState(PlayerState.Move);
        playerController.EnterWeaponInit(wantCrosshair, wantBullet);
        playerController.UpdateBulletUI(currentBulletNum, currentMaxBulletNum, spareBulletNum);
        PlayAudio(0);
        if (bulletFireEffect != null) 
            bulletFireEffect.SetActive(false);
    }
    public virtual void Exit(Action onExitOver)
    {
        animator.SetTrigger("Exit");
        this.onExitOver = onExitOver;
    }
    public void PlayAudio(int index)
    {
        audioSource.Stop();
        audioSource.PlayOneShot(audioClips[index]);
    }
    /// <summary>
    /// 鼠标左键攻击
    /// </summary>
    protected virtual void OnLeftAttack()
    {
        // 需不需要子弹
        if(wantBullet)
        {
            currentBulletNum--;
            playerController.UpdateBulletUI(currentBulletNum, currentMaxBulletNum, spareBulletNum);
        }
        animator.SetTrigger("Shoot");
        canAttack = false;
        
        if (bulletFireEffect != null)
            bulletFireEffect.SetActive(true);
        if (wantRecoil) 
            playerController.WeaponRecoil(recoilStrength);
        PlayAudio(1);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(canThroughWall)
        {
            RaycastHit[] hitInfos = Physics.RaycastAll(ray, 1500);
            for(int i = 0; i < hitInfos.Length; i++)
            {
                HitGameObject(hitInfos[i]);
            }
        }
        else
        {
            if(Physics.Raycast(ray, out RaycastHit hitInfo, 1500))
            {
                HitGameObject(hitInfo);
            }
        }
    }
    /// <summary>
    /// 射线检测
    /// </summary>
    /// <param name="hitInfo"></param>
    private void HitGameObject(RaycastHit hitInfo)
    {
        if(hitInfo.collider.CompareTag("Zombie"))
        {
            // 攻击效果
            GameObject go = Instantiate(bulletPrefabs[1], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);

            // 僵尸逻辑
            ZombieController zombieController = hitInfo.collider.GetComponent<ZombieController>();
            if (zombieController == null)
                zombieController = hitInfo.collider.GetComponentInParent<ZombieController>();
            zombieController.Hurt(attackValue);
        }
        else if(hitInfo.collider.gameObject != this.gameObject)
        {
            // 攻击效果
            GameObject go = Instantiate(bulletPrefabs[0], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
        }
    }
    /// <summary>
    /// 换子弹
    /// </summary>
    public void Reloading()
    {
        // 一、弹匣满的或者弹匣未满，备用为空
        if (currentBulletNum == currentMaxBulletNum || spareBulletNum == 0)
            return;
        // 二、弹匣未满，存在备用
        else
        {
            playerController.ChangePlayerState(PlayerState.Reload);
            canReload = true;
        }
    }

    #region 动画事件
    private void EnterOver()
    {
        canAttack = true;
    }
    /// <summary>
    /// 播放退出动画时调用，onExitOver委托中是下一个武器的Enter方法
    /// </summary>
    private void ExitOver()
    {
        gameObject.SetActive(false);
        // 退出成功后，
        onExitOver?.Invoke();
    }
    protected void ShootOver()
    {
        canAttack = true;
        if (bulletFireEffect != null)
            bulletFireEffect.SetActive(false);
        if (playerController.playerState == PlayerState.Shoot)
        {
            playerController.ChangePlayerState(PlayerState.Move);
        }
    }
    private void ReloadOver()
    {
        if(canReload)
        {
            // 备用子弹不足
            if(currentMaxBulletNum - currentBulletNum > spareBulletNum)
            {
                currentBulletNum += spareBulletNum;
                spareBulletNum = 0;
            }
            // 足够
            else
            {
                spareBulletNum = spareBulletNum - (currentMaxBulletNum - currentBulletNum);
                currentBulletNum = currentMaxBulletNum;
            }
            playerController.UpdateBulletUI(currentBulletNum, currentMaxBulletNum, spareBulletNum);
            animator.SetBool("Reload", false);
            playerController.ChangePlayerState(PlayerState.Move);
        }
    }
    
    #endregion
}
