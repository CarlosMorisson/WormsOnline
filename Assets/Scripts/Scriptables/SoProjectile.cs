using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SoProjectile : ScriptableObject
{
    [Header("Projectile Details")]
    [Tooltip("Effect when the projectile hits something")]
    public GameObject ProjectileEffect;
    [Tooltip("Projectile")]
    public GameObject Projectile;
    [Tooltip("Projectile Speed")]
    public float ProjectileSpeed;
    [Tooltip("The time before destroy the projectile after shoot")]
    public float ProjectileLifeTime;
}
