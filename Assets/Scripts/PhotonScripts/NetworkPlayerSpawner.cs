using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    [Header("Player Clone")]
    private GameObject spawnedPlayerPrefab;
    private void Start()
    {
    }

    public override void OnJoinedRoom()
    {

        base.OnJoinedRoom();
        Debug.Log("entrou na sala");
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("NetworkPlayer", transform.position, transform.rotation);
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}