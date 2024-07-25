using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class SoGun : ScriptableObject
{
    [Header("GUN")]
    [Tooltip("The time after the Shoot is temporarily unavailable after it has been used. "), Range(0, 6)]
    public float GunCoolDown;

    [Tooltip("The Types Of Gun")]
    public enum GunType
    {
        PortalGun,
        OtherGun
    };
    public GunType gunType;

    [Tooltip("The projectile of gun")]
    public GameObject[] GunProjectiles;

    [Tooltip("The speed of rotation around the player")]
    public float GunSpeedRotation;
}