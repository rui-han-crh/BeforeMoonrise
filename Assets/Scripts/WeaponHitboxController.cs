using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponHitboxController : MonoBehaviour
{
    [SerializeField]
    private Collider2D weaponHitbox;

    [SerializeField]
    private Animator animator;

    private void Awake()
    {
        weaponHitbox.isTrigger = true;
        weaponHitbox.enabled = false;
    }

    public void EnableHitbox()
    {
        weaponHitbox.enabled = true;
    }

    public void DisableHitbox()
    {
        weaponHitbox.enabled = false;
    }
}
