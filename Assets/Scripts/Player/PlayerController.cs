using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class PlayerController : MonoBehaviour, IPlayerController
{
    public static PlayerController instance;
    [SerializeField] private SoStats _stats;
    private GameObject player;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    #region Interface

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    #endregion
    private float _time;

    public GameObject Player => player;

    private Transform _playerSprite;
    private TrailRenderer _dashTrail;
    private GameObject _shadownPlayer;
    private Animator _playerAnimator;

    private void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        _playerSprite = GameObject.FindGameObjectWithTag("PlayerSprite").transform;

        _shadownPlayer = GameObject.FindGameObjectWithTag("ShadowPlayer");
        _rb = player.GetComponent<Rigidbody2D>();
        _col = player.GetComponent<CapsuleCollider2D>();
        _dashTrail = player.GetComponentInChildren<TrailRenderer>();
        _playerAnimator = player.GetComponentInChildren<Animator>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
    }

    private void Update()
    {
        _time += Time.deltaTime;
        GatherInput();
    }

    private void GatherInput()
    {
        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump"),
            JumpHeld = Input.GetButton("Jump"),
            DashDown = Input.GetButtonDown("Dash"),
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            BackTimeDown = Input.GetKeyDown(KeyCode.E)
        };

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
        if (_frameInput.BackTimeDown)
        {
            _backTimeToConsume = true;
        }
        if (_frameInput.DashDown)
        {
            _dashToConsume = true;
        }
    }

    private void FixedUpdate()
    {
        CheckCollisions();

        HandleJump();
        HandleDirection();
        HandleGravity();
        HadleDash();

        ApplyMovement();

        HandleBackTime();
    }

    #region Collisions

    private float _frameLeftGrounded = float.MinValue;
    private bool _grounded;

    private void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;

        // Ground and Ceiling
        bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, ~_stats.PlayerLayer);

        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);
        // Debug.Log(groundHit);
        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            Debug.Log(groundHit);
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    void OnDrawGizmos()
    {
        if (_col == null || _stats == null)
            return;

        // Desenhar a cápsula para groundHit
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_col.bounds.center + Vector3.down * _stats.GrounderDistance, _col.size.y / 2);
        Gizmos.DrawWireSphere(_col.bounds.center + Vector3.down * (_stats.GrounderDistance + _col.size.y), _col.size.y / 2);
        Gizmos.DrawLine(_col.bounds.center + Vector3.down * _stats.GrounderDistance + Vector3.left * (_col.size.x / 2),
                        _col.bounds.center + Vector3.down * (_stats.GrounderDistance + _col.size.y) + Vector3.left * (_col.size.x / 2));
        Gizmos.DrawLine(_col.bounds.center + Vector3.down * _stats.GrounderDistance + Vector3.right * (_col.size.x / 2),
                        _col.bounds.center + Vector3.down * (_stats.GrounderDistance + _col.size.y) + Vector3.right * (_col.size.x / 2));

        // Desenhar a cápsula para ceilingHit
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_col.bounds.center + Vector3.up * _stats.GrounderDistance, _col.size.y / 2);
        Gizmos.DrawWireSphere(_col.bounds.center + Vector3.up * (_stats.GrounderDistance + _col.size.y), _col.size.y / 2);
        Gizmos.DrawLine(_col.bounds.center + Vector3.up * _stats.GrounderDistance + Vector3.left * (_col.size.x / 2),
                        _col.bounds.center + Vector3.up * (_stats.GrounderDistance + _col.size.y) + Vector3.left * (_col.size.x / 2));
        Gizmos.DrawLine(_col.bounds.center + Vector3.up * _stats.GrounderDistance + Vector3.right * (_col.size.x / 2),
                        _col.bounds.center + Vector3.up * (_stats.GrounderDistance + _col.size.y) + Vector3.right * (_col.size.x / 2));
    }

    #endregion

    #region Jumping

    private bool _jumpToConsume;
    private float _jumpsToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private bool _coyoteUsable;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    private bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0)
            _endedJumpEarly = true;

        if (_grounded || _backTimeActive)
            _jumpsToConsume = _stats.MaxJumps;

        if (!_jumpToConsume && !HasBufferedJump)
            return;
        if (CanUseCoyote || _jumpsToConsume > 0)
            ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _playerAnimator.CrossFade("Jump Animation", 0);
        _jumpsToConsume -= 1;
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
    }

    #endregion

    #region Horizontal

    private void HandleDirection()
    {
        LookingAtDirection();
        if (_frameInput.Move.x == 0)
        {
            float deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            if (_frameVelocity.x == 0 && _grounded)
                _playerAnimator.CrossFade("Idle Animation", 0);
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed, _stats.Acceleration * Time.fixedDeltaTime);
            if (_grounded)
                _playerAnimator.CrossFade("Run Animation", 0);
        }
    }

    private void LookingAtDirection()
    {
        if (_frameInput.Move.x == -1)
            _playerSprite.transform.localScale = new Vector2(-1, 1);
        if (_frameInput.Move.x == 1)
            _playerSprite.transform.localScale = new Vector2(1, 1);
    }

    #endregion

    #region Gravity

    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
            _frameVelocity.y = _stats.GroundingForce;

        else
        {
            float inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0)
                inAirGravity *= _stats.JumpEndEarlyGravityModifier;

            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Dashing

    private bool _dashToConsume;
    private float _timeLastDash = float.MinValue;
    private float _dashingTime;

    private void HadleDash()
    {
        if (_dashToConsume && _time > _timeLastDash + _stats.DashCooldown)
            StartCoroutine(ExecuteDash());
        _dashToConsume = false;
    }

    private IEnumerator ExecuteDash()
    {
        _dashTrail.emitting = true;
        _frameVelocity.x *= _stats.DashPower;
        _timeLastDash = _time;
        yield return new WaitForSeconds(_dashingTime = _stats.DashingTime);
        _dashTrail.emitting = false;
        _dashToConsume = false;
    }

    #endregion

    #region BackTime

    private float _backTimeCoolDown;
    private float _fullBackTimeCoolDown;
    private bool _canCountCoolDown = true;
    private bool _backTimeToConsume;
    private bool _backTimeActive;

    private void HandleBackTime()
    {
        _fullBackTimeCoolDown = _stats.FullBackTimeCooldown;
        if (_backTimeToConsume && _backTimeCoolDown >= _fullBackTimeCoolDown)
            ExecuteBackTime();
        if (_canCountCoolDown)
        {
            if (_backTimeCoolDown < _fullBackTimeCoolDown)
            {
                _backTimeCoolDown += Time.deltaTime;
                _backTimeActive = false;
                //coolDownBar.fillAmount = coolDownShadow / coolDownTotalShadow;
            }
            else
            {
                _canCountCoolDown = false;
                _shadownPlayer.gameObject.SetActive(true);
            }
        }
        _backTimeToConsume = false;
    }

    private void ExecuteBackTime()
    {
        _backTimeCoolDown = 0;
        player.transform.localPosition = _shadownPlayer.transform.localPosition;
        ShadowPlayerController.instance._playerPositions.Clear();
        _shadownPlayer.SetActive(false);
        _backTimeActive = true;
        _canCountCoolDown = true;
        _backTimeToConsume = false;
    }

    #endregion

    #region CombatPlayer

    [SerializeField]
    private Material damageMaterial;

    public void ReceiveDamage(Vector3 knockUp)
    {
        _knockbackForce = knockUp * 2; // Ajuste a força conforme necessário
        _knockbackDuration = 0.5f; // Duração do knockback em segundos
        _knockbackTimer = _knockbackDuration;

        _playerAnimator.CrossFade("Fall Animation", 0);
        StartCoroutine(ChangeSpriteMaterial());
        Debug.Log("Recebendo dano e aplicando knockback: " + _knockbackForce);
    }

    private IEnumerator ChangeSpriteMaterial()
    {
        Material newMaterial = _playerSprite.GetComponent<SpriteRenderer>().material;
        Material _playerMaterial = newMaterial;
        transform.DOJump(new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), 3, 1, 3);
        _playerSprite.GetComponent<SpriteRenderer>().material = damageMaterial;
        yield return new WaitForSeconds(2f);
        //_rb.isKinematic = true;
        _playerSprite.GetComponent<SpriteRenderer>().material = _playerMaterial;
    }

    #endregion
    private Vector2 _knockbackForce;
    private float _knockbackDuration;
    private float _knockbackTimer;
    private void ApplyMovement()
    {
        if (_knockbackTimer > 0)
        {
            // Durante o knockback, aplique a força de knockback
            _rb.velocity = _knockbackForce;
            _knockbackTimer -= Time.fixedDeltaTime;
        }
        else
        {
            // Movimento regular
            _rb.velocity = _frameVelocity;
        }
    }

}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;
    public Vector2 FrameInput { get; }
}

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public bool DashDown;
    public Vector2 Move;
    public bool BackTimeDown;
}
