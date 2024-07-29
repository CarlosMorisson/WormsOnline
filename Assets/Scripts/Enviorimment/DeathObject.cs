using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathObject : MonoBehaviour
{
   
    private void Start()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        NetworkPlayer network = collision.GetComponent<NetworkPlayer>();
        if(network != null)
        {
            if (network._photonView.IsMine == true)
            {
                PlayerController.instance.CountDeath();
            }
            else
            { 
                UIController.instance.UpdateHudEnemy(true, 0, network.HudEnemy);
               
            }
        }
        

    }
}
