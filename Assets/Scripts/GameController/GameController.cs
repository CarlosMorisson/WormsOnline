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

    void Start()
    {
        instance = this;
        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating("UpdateTimer", 0f, 1f);
        }
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
            stream.SendNext(TimeInSeconds);
        else
            TimeInSeconds = (float)stream.ReceiveNext();    
        DisplayTime(TimeInSeconds);
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
