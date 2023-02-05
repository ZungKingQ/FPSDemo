using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������
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
    /// �л���ɫ״̬ʱִ��
    /// </summary>
    /// <param name="playerState"></param>
    public abstract void OnEnterPlayerState(PlayerState playerState);
    /// <summary>
    /// ��ɫ��ͬ״̬ʱִ��
    /// </summary>
    /// <param name="playerState"></param>
    public abstract void OnUpdatePlayerState(PlayerState playerState);
    /// <summary>
    /// ��ʼ���ӵ���Ŀ
    /// </summary>
    public virtual void WeaponBulletinit()
    {
            currentBulletNum = currentMaxBulletNum;
            spareBulletNum = spareMaxBulletNum;
    }
    /// <summary>
    /// �л�����ʱ����
    /// </summary>
    public virtual void Enter()
    {
        // ��������
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
    /// ����������
    /// </summary>
    protected virtual void OnLeftAttack()
    {
        // �費��Ҫ�ӵ�
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
    /// ���߼��
    /// </summary>
    /// <param name="hitInfo"></param>
    private void HitGameObject(RaycastHit hitInfo)
    {
        if(hitInfo.collider.CompareTag("Zombie"))
        {
            // ����Ч��
            GameObject go = Instantiate(bulletPrefabs[1], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);

            // ��ʬ�߼�
            ZombieController zombieController = hitInfo.collider.GetComponent<ZombieController>();
            if (zombieController == null)
                zombieController = hitInfo.collider.GetComponentInParent<ZombieController>();
            zombieController.Hurt(attackValue);
        }
        else if(hitInfo.collider.gameObject != this.gameObject)
        {
            // ����Ч��
            GameObject go = Instantiate(bulletPrefabs[0], hitInfo.point, Quaternion.identity);
            go.transform.LookAt(Camera.main.transform);
        }
    }
    /// <summary>
    /// ���ӵ�
    /// </summary>
    public void Reloading()
    {
        // һ����ϻ���Ļ��ߵ�ϻδ��������Ϊ��
        if (currentBulletNum == currentMaxBulletNum || spareBulletNum == 0)
            return;
        // ������ϻδ�������ڱ���
        else
        {
            playerController.ChangePlayerState(PlayerState.Reload);
            canReload = true;
        }
    }

    #region �����¼�
    private void EnterOver()
    {
        canAttack = true;
    }
    /// <summary>
    /// �����˳�����ʱ���ã�onExitOverί��������һ��������Enter����
    /// </summary>
    private void ExitOver()
    {
        gameObject.SetActive(false);
        // �˳��ɹ���
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
            // �����ӵ�����
            if(currentMaxBulletNum - currentBulletNum > spareBulletNum)
            {
                currentBulletNum += spareBulletNum;
                spareBulletNum = 0;
            }
            // �㹻
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
