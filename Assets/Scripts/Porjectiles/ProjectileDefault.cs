using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ProjectileDefault : MonoBehaviour
{
    public SoProjectile _stats;
    [HideInInspector]
    public float _lifeTime;
    [HideInInspector]
    public Vector2 _shootDirection;
    [HideInInspector]
    public GameObject _shootEffect;
    public float _speed;
    public GameObject parent;
    private PhotonView _photon;
    private float _damage;
    public void Awake()
    {
        GetReferences();
        Destroy(parent, _lifeTime);
        //
        SetDirection();
        _photon = GetComponentInParent<PhotonView>();
    }
    public virtual void SetDirection()
    {
        GetComponent<Rigidbody2D>().AddForce(transform.up * _speed);
    }

    public void GetReferences()
    {
        _speed = _stats.ProjectileSpeed;
        _damage = _stats.ProjectileDamage;
        _lifeTime = _stats.ProjectileLifeTime;
    }
    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (_photon.IsMine == false)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                GameObject Effect = PhotonNetwork.Instantiate("Particle", collision.transform.position, collision.transform.rotation);

                
                Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 knockbackDirection = collision.transform.position - transform.position;
                    knockbackDirection.Normalize();
                    PlayerController.instance.ReceiveDamage(knockbackDirection.normalized, _damage);
                }
                StartCoroutine(DestroyObject(Effect));
                DestroyObject(parent);

            }
            if (collision.gameObject.CompareTag("NetworkPlayer"))
            {
                UIController.instance.UpdateHudEnemy(false, _damage, collision.GetComponent<NetworkPlayer>().HudEnemy);
                DestroyObject(parent);

            }
        }
        else
        {
            if (collision.gameObject.CompareTag("NetworkPlayer"))
            {
                UIController.instance.UpdateHudEnemy(false, _damage, collision.GetComponent<NetworkPlayer>().HudEnemy);
                DestroyObject(parent);

            }
        }
        if (collision.gameObject.CompareTag("Untagged"))
        {
            GameObject Effect = PhotonNetwork.Instantiate("Particle", collision.transform.position, collision.transform.rotation);

            DestroyObject(Effect);
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 knockbackDirection = collision.transform.position - transform.position;
                knockbackDirection.Normalize();
                PlayerController.instance.ReceiveDamage(knockbackDirection.normalized, _damage);
            }

            //Destroy(parent);
        }
    }
    IEnumerator DestroyObject(GameObject ObjectToDestroy)
    {
        yield return new WaitForSeconds(.25f);
        PhotonNetwork.Destroy(ObjectToDestroy);
    }
}
