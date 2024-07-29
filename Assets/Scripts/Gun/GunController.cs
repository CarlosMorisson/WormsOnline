using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class GunController : MonoBehaviour, IGunController
{
    private GunInput _gunInput;
    #region Interface
    public bool GunInput => _gunInput.RightMouseDown;
    #endregion
    private GameObject _player;
    private Transform _playerHand;
    private Transform _playerGun;
    private float _fireRate;
    private Animator _anim;
    [SerializeField]
    private SoGun _stats;
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerHand = GameObject.FindGameObjectWithTag("Hand").transform;
        _playerGun = _playerHand.gameObject.transform.GetChild(0).transform;
        _anim = _playerGun.GetComponent<Animator>();
        _rotationSpeed = _stats.GunSpeedRotation;
        _fireRate = _stats.GunCoolDown;
    }

    // Update is called once per frame
    void Update()
    {
        GatherInput();
        AroundThePlayer();
        GetRightInput();
        GetLeftInput();
    }
    #region GetInputs
    private void GatherInput()
    {
        _gunInput = new GunInput
        {
            RightMouseDown = Input.GetKeyDown(KeyCode.Mouse1),
            LeftMouseDown = Input.GetKeyDown(KeyCode.Mouse0)
        };
        if (_gunInput.RightMouseDown)
            _rShoot = true;
        if (_gunInput.LeftMouseDown)
            _lShoot = true;
    }
    #endregion
    #region RightMouseShoot
    private bool _rShoot;
    private float _rFireTimer;
    private void GetRightInput()
    {
        if (_rShoot && _fireRate <= _rFireTimer)
            ExecuteRightShoot();
        if (_fireRate > _rFireTimer)
        {
            _rFireTimer += Time.deltaTime;
        }
        _rShoot = false;
    }
    private void ExecuteRightShoot()
    {
        _anim.SetTrigger("Shoot");
        PhotonNetwork.Instantiate("LeftProjectile", _playerGun.transform.position, _playerGun.transform.rotation);
        _rShoot = false;
        _rFireTimer = 0;
    }
    #endregion

    #region LeftMouseShoot
    private bool _lShoot;
    private float _lFireTimer;
    private void GetLeftInput()
    {
        if (_lShoot && _fireRate <= _lFireTimer)
            ExecuteLeftShoot();
        if (_fireRate > _lFireTimer)
        {
            _lFireTimer += Time.deltaTime;
        }
        _lShoot = false;
    }
    private void ExecuteLeftShoot()
    {
        _anim.SetBool("Shoot", true);
        PhotonNetwork.Instantiate("LeftProjectile", _playerGun.transform.position, _playerGun.transform.rotation);
        _lShoot = false;
        _lFireTimer = 0;
        _anim.CrossFadeInFixedTime("GunIdle", 0.1f);
    }
    #endregion
    #region MoveGunAroundThePlayer
    private float _rotationSpeed;
    private bool _isFacingRight=true;
    private void AroundThePlayer()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector3 direction = mousePosition - _player.transform.position;
        direction.Normalize();
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        _playerHand.transform.rotation = Quaternion.Lerp(_playerHand.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);

        //if (direction.x > 0 && !_isFacingRight || direction.x < 0 && _isFacingRight)
          //  FlipGun();
    }
    private void FlipGun()
    {
        Vector3 currentScale = _playerGun.gameObject.transform.localScale;
        currentScale.y *= -1;
        _playerGun.gameObject.transform.localScale = currentScale;

        _isFacingRight = !_isFacingRight;
    }
    #endregion
}
public interface IGunController
{
    public bool GunInput { get; }
}
public struct GunInput
{
    public bool RightMouseDown;
    public bool LeftMouseDown;
}