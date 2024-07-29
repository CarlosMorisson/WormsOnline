using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class GameController : MonoBehaviour
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

    }

    // Update is called once per frame
    void Update()
    {
        
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
