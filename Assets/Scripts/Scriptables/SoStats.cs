using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SoStats : ScriptableObject
{
    [Header("LAYERS")]
    [Tooltip("Set this to the layer your player is on")]
    public LayerMask PlayerLayer;

    [Header("MOVEMENT")]
    [Tooltip("The top horizontal movement speed")]
    public float MaxSpeed = 14;

    [Tooltip("The player's capacity to gain horizontal speed")]
    public float Acceleration = 120;

    [Tooltip("The pace at which the player comes to a stop")]
    public float GroundDeceleration = 60;

    [Tooltip("Deceleration in air only after stopping input mid-air")]
    public float AirDeceleration = 30;

    [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
    public float GroundingForce = -1.5f;

    [Tooltip("The detection distance for grounding and roof detection"), Range(-5f, 5f)]
    public float GrounderDistance = 0.05f;

    [Header("JUMP")]
    [Tooltip("The immediate velocity applied when jumping")]
    public float JumpPower = 36;

    [Tooltip("The maximum possible number of jumps before touching the ground")]
    public float MaxJumps = 2;

    [Tooltip("The maximum vertical movement speed")]
    public float MaxFallSpeed = 40;

    [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
    public float FallAcceleration = 110;

    [Tooltip("The gravity multiplier added when jump is released early")]
    public float JumpEndEarlyGravityModifier = 3;

    [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
    public float CoyoteTime = .15f;

    [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
    public float JumpBuffer = .2f;

    [Header("DASH")]
    [Tooltip("The horizontal speed multiplier when dash"), Range(3, 5)]
    public float DashPower = 3.5f;

    [Tooltip("The time after the dash is temporarily unavailable after it has been used."), Range(1, 5)]
    public float DashCooldown = 2f;

    [Tooltip("The time that the dash works when the button is pressed"), Range(0, 1)]
    public float DashingTime = .5f;

    [Header("BACKTIME")]
    [Tooltip("The time after the back time is temporarily unavailable after it has been used. "), Range(1, 5)]
    public float FullBackTimeCooldown = 3f;

    [Tooltip("The time counting after the back time is temporarily unavailable after it has been used. "), Range(1, 5)]
    public float BackTimeCooldown = 3f;
}