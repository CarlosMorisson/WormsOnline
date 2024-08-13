using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
public class GameController : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameController instance;
    [SerializeField]
    float DeathCount, TimeInSeconds;
    [SerializeField]
    Transform ressurrectionPoint;
    [SerializeField]
    GameObject ressurrectionPlataform;
    private bool _readyToPlay;
    [SerializeField]
    int numberOfReadyPlayer;
    void Start()
    {
        instance = this;

    }
    private void UpdateTimer()
    {
        if (TimeInSeconds > 0)
        {
            TimeInSeconds -= 1f;
            DisplayTime(TimeInSeconds);
        }
        else
        {
            TimeInSeconds = 0;
            CancelInvoke("UpdateTimer");
        }

       
    }
    public void LeaveGame()
    {
        ConnectToServer server;
        server = FindObjectOfType<ConnectToServer>();
        server.LeaveRoom();
    }
    void DisplayTime(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        UIController.instance.Cronometer.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(TimeInSeconds);
            stream.SendNext(numberOfReadyPlayer);
        }
        else
        {
            TimeInSeconds = (float)stream.ReceiveNext();
            numberOfReadyPlayer = (int)stream.ReceiveNext();
        }
            
        if (PhotonNetwork.IsMasterClient==false)
        {
            DisplayTime(TimeInSeconds);
        }
    }
    public void ReadyToPlay()
    {
        _readyToPlay = !_readyToPlay;
        photonView.RPC("UpdateReadyPlayers", RpcTarget.All, _readyToPlay);
    }

    [PunRPC]
    void UpdateReadyPlayers(bool playerReady)
    {
        if (playerReady)
            numberOfReadyPlayer++;
        else
            numberOfReadyPlayer--;

        UIController.instance.readyPlayerText.text = numberOfReadyPlayer.ToString();
        if (PhotonNetwork.PlayerList.Length == numberOfReadyPlayer)
        {
            UIController.instance._startCanva.SetActive(false);
            if (PhotonNetwork.IsMasterClient)
                InvokeRepeating("UpdateTimer", 0f, 1f);
        }
    }
    public void Ressurrection()
    {
        DeathCount++;
        StartCoroutine(ScalePlataform());

    }
    IEnumerator ScalePlataform()
    {
        ressurrectionPlataform.SetActive(true);
        ressurrectionPlataform.transform.DOScale(new Vector3(6, 1, 1), 3f);
        PlayerController.instance.Player.transform.position = ressurrectionPoint.position;
        yield return new WaitForSeconds(3f);
        ressurrectionPlataform.transform.DOScale(new Vector3(0, 0, 0), 3f);
        ressurrectionPlataform.SetActive(false);
    }
}
