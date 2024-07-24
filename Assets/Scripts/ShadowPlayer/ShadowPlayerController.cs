using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowPlayerController : MonoBehaviour
{
    public static ShadowPlayerController instance;
    private Transform _playerTransform;
    private Rigidbody2D _followerRigidbody;
    [HideInInspector]
    public List<Vector2> _playerPositions = new List<Vector2>();
    private Transform _shadowTransform;
    [SerializeField] private SoShadowPlayer _stats;
    private void Start()
    {
        instance = this;

    }
    private void OnEnable()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _shadowTransform = GameObject.FindGameObjectWithTag("ShadowPlayer").transform;
        _shadowTransform.position = _playerTransform.position;
        _followerRigidbody = GameObject.FindGameObjectWithTag("ShadowPlayer").GetComponent<Rigidbody2D>();

        StartCoroutine(UpdatePlayerPositions());
    }

    private void FixedUpdate()
    {
        FallowPlayer();
    }
    #region FallowPlayer
    private float _speed;
    private void FallowPlayer()
    {

        if (_playerPositions.Count > 0)
        {
            _speed = _stats.Speed;
            Vector2 targetPosition = _playerPositions[0];
            if (_playerPositions.Count > 10)
            {
                targetPosition = _playerPositions[10];
            }

            _followerRigidbody.MovePosition(Vector2.Lerp(_followerRigidbody.position, targetPosition, _speed * Time.fixedDeltaTime));
        }
    }
    private IEnumerator UpdatePlayerPositions()
    {
        while (true)
        {
            _playerPositions.Insert(0, _playerTransform.position);
            if (_playerPositions.Count > 11)
            {
                _playerPositions.RemoveAt(_playerPositions.Count - 1);
            }
            yield return new WaitForSeconds(.05f);
        }
    }
    #endregion

}