using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeCollider : MonoBehaviour
{
    Collider damageCollider;
    private Knife knife;
    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.enabled = false;
    }
    public void Init(Knife knife)
    {
        this.knife = knife;
    }
    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }
    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        knife.HitTarget(other.gameObject, other.transform.position);
    }
}
