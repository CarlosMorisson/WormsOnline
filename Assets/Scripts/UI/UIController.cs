using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
public class UIController : MonoBehaviourPunCallbacks
{
    public static UIController instance;
    [SerializeField]
    GameObject PlayerHud;
    [SerializeField]
    GameObject[] EnemyPlayerHud;
    [SerializeField]
    public TextMeshProUGUI Cronometer;
    [SerializeField]
    GameObject GameOver,CanvaHud;
    [HideInInspector]
    public int _enemyPlayerHudIndex;

    void Start()
    {
        instance = this;
        GetPlayerReferenceHud();
    }
    public void ShowGameOver()
    {
        if (CheckWinner())
            Debug.Log("DeuCerto");
        else
            Debug.Log("NaoDeuCerto");
        CanvaHud.SetActive(false);
        GameOver.SetActive(true);
    }
    private bool CheckWinner()
    {
        int maxEnemyDeaths = 0;
        bool hasWinner = false;
        bool isTie = false;

        foreach (var enemyHud in EnemyPlayerHud)
        {
            TextMeshProUGUI deathNumEnemy = enemyHud.transform.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
            int enemyDeaths = int.Parse(deathNumEnemy.text);
            Debug.Log("entrou no foreach");
            if (enemyDeaths > maxEnemyDeaths)
            {
                maxEnemyDeaths = enemyDeaths;
                hasWinner = false; // Reseta a condição de empate
                isTie = false;
            }
            else if (enemyDeaths == maxEnemyDeaths)
            {
                isTie = true; // Marque como empate se encontrar um valor igual
            }
        }

        if (_numPlayerDeath > maxEnemyDeaths)
        {
            hasWinner = true;
        }
        else if (_numPlayerDeath == maxEnemyDeaths && !isTie)
        {
            hasWinner = false;
        }

        return hasWinner;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    #region EnemyHud
    public GameObject CreateHudPlayer(Sprite enemySprite, string enemyName)
    {
        EnemyPlayerHud[_enemyPlayerHudIndex].SetActive(true);
        GetReferencesFromGameObjectEnemyHud(EnemyPlayerHud[_enemyPlayerHudIndex]);
        _enemySplashArt.sprite = enemySprite;
        EnemyPlayerHud[_enemyPlayerHudIndex].transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>().sprite = _enemySplashArt.sprite;
        _enemyName.text = enemyName;
        EnemyPlayerHud[_enemyPlayerHudIndex].transform.GetChild(0).GetChild(3).GetComponentInChildren<TextMeshProUGUI>().text = _enemyName.text;
        return EnemyPlayerHud[_enemyPlayerHudIndex];
    }
    private TextMeshProUGUI _enemyName;
    private Image _enemySplashArt;
    private TextMeshProUGUI _damagePercentageEnemy;
    private TextMeshProUGUI _deathNumEnemy;
    private float _newDamage;
    private int _numEnemyDeath;
    private void GetReferencesFromGameObjectEnemyHud(GameObject EnemyHud)
    {
        _enemyName = EnemyHud.transform.GetChild(0).GetChild(3).GetComponentInChildren<TextMeshProUGUI>();
        _enemySplashArt = EnemyHud.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>();
        _damagePercentageEnemy = EnemyHud.transform.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        _deathNumEnemy = EnemyHud.transform.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
    }
    public void UpdateHudEnemy(bool isDead, float actualizeDamage, GameObject Hud)
    {
        _newDamage += actualizeDamage;
        if (isDead)
        {
            _numEnemyDeath++;
            _deathNumEnemy.text = _numEnemyDeath.ToString();
            _damagePercentageEnemy.text = actualizeDamage.ToString() + "%";
        }
        else
        {
            Hud.transform.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = _newDamage.ToString() + "%";
        }
    }
    #endregion

    #region PlayerHud
    private TextMeshProUGUI _playerName;
    private Image _playerSplashArt;
    private TextMeshProUGUI _damagePercentagePlayer;
    private TextMeshProUGUI _deathNumPlayer;
    private int _numPlayerDeath;
    void GetPlayerReferenceHud()
    {
        _playerName = PlayerHud.transform.GetChild(0).GetChild(3).GetComponentInChildren<TextMeshProUGUI>();
        _playerName.text = PhotonNetwork.NickName;
        _playerSplashArt = PlayerHud.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>();
        _damagePercentagePlayer = PlayerHud.transform.GetChild(0).GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        _deathNumPlayer = PlayerHud.transform.GetChild(0).GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
    }
    public void UpdateHudPlayer(bool isDead, float actualizeDamage)
    {
        if (isDead)
        {
            _numPlayerDeath++;
            _deathNumPlayer.text= _numPlayerDeath.ToString();
            _damagePercentagePlayer.text = actualizeDamage.ToString() + "%";
        }
        else
        {
            _damagePercentagePlayer.text = actualizeDamage.ToString() + "%";
        }
    }
    #endregion
}
