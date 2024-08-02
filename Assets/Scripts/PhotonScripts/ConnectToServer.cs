using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TMP_InputField _createInput, _joinInput, _nameInput;
    [SerializeField] 
    private GameObject spawnedPlayerPrefab, _canvasServer;
    private Button _joinButton, _createButton;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (FindObjectsOfType<ConnectToServer>().Length > 1)
        {
            Destroy(gameObject);
        }
        GetMenuReferences();
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    private void Update()
    {
        if (_nameInput.text.Length < 1 && _createInput.text.Length < 1)
            _createButton.interactable = false;
        else
            _createButton.interactable = true;

        if (_nameInput.text.Length < 1 && _joinInput.text.Length < 1)
            _joinButton.interactable = false;
        else
            _joinButton.interactable = true;
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        _canvasServer.SetActive(true);
    }
    public void LeaveRoom()
    {
        SceneManager.LoadScene("MenuGame");
        PhotonNetwork.LeaveRoom();
        StartCoroutine(CheckScene());
    }
    public IEnumerator CheckScene()
    {
        while (SceneManager.GetActiveScene().name != "MenuGame")
        {
            yield return null;
        }
        GetMenuReferences();
    }
    public void GetMenuReferences()
    {
        _canvasServer = GameObject.FindGameObjectWithTag("JoinRoom");
        _joinInput = _canvasServer.transform.GetChild(0).GetComponent<TMP_InputField>();
        _createInput = _canvasServer.transform.GetChild(1).GetComponent<TMP_InputField>();
        _nameInput = GameObject.FindGameObjectWithTag("GameName").GetComponent<TMP_InputField>();
        _createButton = _createInput.transform.GetChild(2).GetComponent<Button>();
        _createButton.onClick.AddListener(() => CreateRoom());
        _joinButton = _joinInput.transform.GetChild(2).GetComponent<Button>();
        _joinButton.onClick.AddListener(()=>JoinRoom());

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
        PhotonNetwork.NickName = _nameInput.text;
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
