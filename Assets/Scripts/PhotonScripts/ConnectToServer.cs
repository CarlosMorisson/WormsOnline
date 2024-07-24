using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Canvas _canvasServer;
    [SerializeField]
    private TMP_InputField _createInput, _joinInput;
    [SerializeField] 
    private GameObject spawnedPlayerPrefab;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (FindObjectsOfType<ConnectToServer>().Length > 1)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        _canvasServer.gameObject.SetActive(true);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(_createInput.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(_joinInput.text);

    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("SampleScene");

        base.OnJoinedRoom();
        Debug.Log("entrou na sala");
        StartCoroutine(CheckAndInstantiatePlayer());

    }
    private IEnumerator CheckAndInstantiatePlayer()
    {
        while (SceneManager.GetActiveScene().name != "SampleScene")
        {
            yield return null; // Espera até o próximo frame
        }

        // Instancia o NetworkPlayer quando a cena for "SampleScene"
        InstantiateNetworkPlayer();
    }

    private void InstantiateNetworkPlayer()
    {
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("NetworkPlayer", transform.position, transform.rotation);
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
