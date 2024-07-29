using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour
{
    [Header("Online Transform Fallow")]
    [Tooltip("The Objects that the clones will fallow")]
    [SerializeField]
    Transform Player, ShadowPlayer, ShootObject, Weapon;
    //
    [Header("Online Animation")]
    [Tooltip("The animation that will appear online")]
    [SerializeField]
    Animator PlayerAnim, ShootObjectAnim;
    //
    [HideInInspector]
    public PhotonView _photonView;
    //
    [Header("Clone Objects")]
    [Tooltip("The Objects that appear online")]
    [SerializeField]
    Transform player, shadowPlayer, shootObject, weapon;
    [SerializeField]
    Animator cloneAnimator;
    //
    [Header("HudPlayer")]
    [Tooltip("The UI reference for the enemies")]
    [SerializeField]
    public GameObject HudEnemy;
    void Start()
    {
        _photonView = GetComponent<PhotonView>();
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerAnim = GameObject.FindGameObjectWithTag("PlayerSprite").GetComponent<Animator>();
        ShadowPlayer = GameObject.FindGameObjectWithTag("ShadowPlayer").transform;
        Weapon = GameObject.FindGameObjectWithTag("Hand").transform; ;
        if (_photonView.IsMine)
        {
            foreach (var item in GetComponentsInChildren<SpriteRenderer>())
            {
                item.enabled = false;

            }
        }
        else
        {
            HudEnemy=UIController.instance.CreateHudPlayer(gameObject.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>().sprite, "Jorgito");
        }
    }

    void Update()
    {
        if (_photonView.IsMine)
        {

            MapPosition(player, Player);
            MapPosition(shadowPlayer, ShadowPlayer);
            //MapPosition(ShootObject, shootObject);
            MapPosition(weapon, Weapon);

            UpdateAnimation(PlayerAnim, cloneAnimator);
            
        }

    }
    
    void UpdateAnimation(Animator targetAnimator, Animator cloneAnimator)
    {
        AnimatorStateInfo stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(0);
        cloneAnimator.Play(stateInfo.fullPathHash, -1, stateInfo.normalizedTime);
        cloneAnimator.gameObject.transform.localScale=targetAnimator.gameObject.transform.localScale;
    }
    void MapPosition(Transform target, Transform rigTransfrom)
    {
        target.position = rigTransfrom.position;
        target.rotation = rigTransfrom.rotation;
        
    }
}
