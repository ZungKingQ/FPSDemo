using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

public enum PlayerState
{
    Move,
    Shoot,
    Reload,
    Dead
}
/// <summary>
/// 玩家控制器
/// </summary>
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Camera[] cameras;
    [SerializeField] private Image crossImage;

    private int hp = 100;

    #region WeaponReffering
    [SerializeField] WeaponBase[] weapons;
    private int currentWeaponIndex = -1;
    private int previousWeaponIndex = -1;
    private bool canChangeWeapon = true;
    #endregion
    Coroutine weaponRecoil_Cross_Coroutine;
    Coroutine weaponRecoil_Camaera_Coroutine;

    public PlayerState playerState { get; private set; }

    private void Awake()
    {
        Instance = this;

        for(int i = 0; i < weapons.Length; i++)
        {
            weapons[i].WeaponBulletinit();
        }    

        ChangeWeapon(2);
    }
    private void Update()
    {
        Tick();
        weapons[currentWeaponIndex].OnUpdatePlayerState(playerState);
        
    }
    public void DisableFPC()
    {
        this.GetComponent<PlayerController>().enabled = false;
    }
    public void WeaponRecoil(float recoil)
    {
        // 先暂停之前的
        if(weaponRecoil_Cross_Coroutine != null)
        {
            StopCoroutine(weaponRecoil_Cross_Coroutine);
        }
        if (weaponRecoil_Camaera_Coroutine != null)
        {
            StopCoroutine(weaponRecoil_Camaera_Coroutine);
        }
        // 开启新的
        // 准心的后坐力
        weaponRecoil_Cross_Coroutine = StartCoroutine(WeaponRecoil_Cross(recoil));
        // 视角的后坐力
        weaponRecoil_Camaera_Coroutine = StartCoroutine(WeaponRecoil_Camaera(recoil));
    }
    IEnumerator WeaponRecoil_Cross(float recoil)
    {
        Vector2 scale = crossImage.transform.localScale;
        while (scale.x < 1.3f * recoil)
        {
            yield return null;
            scale.x += Time.deltaTime * 3;
            scale.y = scale.x;
            crossImage.transform.localScale = scale;
        }
        while (scale.x > 1)
        {
            yield return null;
            scale.x -= Time.deltaTime * 3;
            scale.y = scale.x;
            crossImage.transform.localScale = scale;
        }
        crossImage.transform.localScale = Vector2.one;
        yield break;
    }
    IEnumerator WeaponRecoil_Camaera(float recoil)
    {
        // 确认偏移量
        float xOffset = Random.Range(0.3f, 0.6f) * recoil;
        float yOffset = Random.Range(-0.15f, 0.15f) * recoil;
        firstPersonController.xRotOffset = xOffset;
        firstPersonController.yRotOffset = yOffset;
        for(int i = 0; i < 6; i++)
        {
            yield return null;
        }
        firstPersonController.xRotOffset = 0;
        firstPersonController.yRotOffset = 0;
    }
    private void Tick()
    {
        CheckKeyDown();
    }
    public void GetHurt(int damage)
    {
        hp -= damage;
        UI_MainPanel.Instance.UpdateHP_Text(hp);
    }
    public void CheckKeyDown()
    {
        if (!canChangeWeapon)
            return;
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (previousWeaponIndex >= 0)
            {
                ChangeWeapon(previousWeaponIndex);
            }
        }
    }
    /// <summary>
    /// 修改玩家状态
    /// </summary>
    /// <param name="newState"></param>
    public void ChangePlayerState(PlayerState newState)
    {
        if (hp <= 0)
            this.playerState = PlayerState.Dead;
        else
            this.playerState = newState;
        weapons[currentWeaponIndex].OnEnterPlayerState(this.playerState);
    }
    public void ChangeWeapon(int newWeaponIndex)
    {
        if (currentWeaponIndex == newWeaponIndex)
            return;
        previousWeaponIndex = currentWeaponIndex;
        currentWeaponIndex = newWeaponIndex;

        // 没有武器时
        if(previousWeaponIndex < 0)
        {
            // 直接进入新武器
            weapons[currentWeaponIndex].Enter();
        }
        else
        {
            // 退出当前武器
            weapons[previousWeaponIndex].Exit(OnWeaponExitOver);
            canChangeWeapon = false;
        }
    }
    /// <summary>
    /// 上一个武器退出成功后的Action
    /// </summary>
    private void OnWeaponExitOver()
    {
        weapons[currentWeaponIndex].Enter();
        canChangeWeapon = true;
    }
    public void EnterWeaponInit(bool wantCrosshair, bool wantBullet)
    {
        crossImage.gameObject.SetActive(wantCrosshair);
        UI_MainPanel.Instance.InitForEnterWeapon(wantBullet);
    }
    /// <summary>
    /// 更新子弹UI
    /// </summary>
    public void UpdateBulletUI(int currentBulletNum, int currentMaxBulletNum, int spareBulletNum)
    {
        UI_MainPanel.Instance.UpdateCurrBullet_Text(currentBulletNum, currentMaxBulletNum);
        UI_MainPanel.Instance.UpdateStandByBullet_Text(spareBulletNum);
    }
    public void SetCameraFOV(int value)
    {
        cameras[0].fieldOfView = value;
        cameras[1].fieldOfView = value;

    }
}
