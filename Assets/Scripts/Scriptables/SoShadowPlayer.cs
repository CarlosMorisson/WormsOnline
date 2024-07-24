using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SoShadowPlayer : ScriptableObject
{
    [Header("FALLOW PLAYER")]
    [Tooltip("The delay in seconds. Adjust this value to control the follower's delay."), Range(0, 5)]
    public float FollowDelay = 1f;

    [Tooltip("The speed at which the follower catches up when the player stops."), Range(5, 20)]
    public float CatchUpSpeed = 10f;

    [Tooltip("Shadow Player Speed To Fallow Player"), Range(5, 20)]
    public float Speed;
}