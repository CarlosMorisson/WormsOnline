using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathObject : MonoBehaviour
{
   
    private void Start()
    {
        
    }
    public void OnTriggerExit2D(Collider2D collision)
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
                Debug.Log("Colidiu");
                UIController.instance.UpdateHudEnemy(true, 0, network.HudEnemy);
               
            }
        }
        

    }
}
