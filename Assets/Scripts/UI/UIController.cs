using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
public class UIController : MonoBehaviourPunCallbacks
{
    public static UIController instance;
    [Header("HUD")]
    [SerializeField]
    GameObject PlayerHud;
    [SerializeField]
    GameObject[] EnemyPlayerHud;
    [SerializeField]
    GameObject CanvaHud;
    [Header("UI")]
    [SerializeField]
    public TextMeshProUGUI Cronometer;
    [Header("GameOver")]
    [SerializeField]
    GameObject GameOver;
    [HideInInspector]
    public int _enemyPlayerHudIndex;
    private TextMeshProUGUI _gameOverText;
    #region WinnerCanva
    [SerializeField]
    private GameObject _winnerCanva;
    private TextMeshProUGUI _winnerName;
    private Image _winnerImage;
    #endregion
    #region TieCanva
    [SerializeField]
    private GameObject _tieCanva;

    #endregion
    void Start()
    {
        instance = this;
        GetPlayerReferenceHud();
        _winnerImage = _winnerCanva.transform.GetChild(0).GetComponent<Image>();
        _winnerName = _winnerCanva.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }
    public void ShowGameOver()
    {

        GameOver.SetActive(true);
        _gameOverText = GameOver.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (CheckWinner())
            ShowWinnerCanva();
        else
            ShowLoseCanva();
        CanvaHud.SetActive(false);
    }
    private void ShowWinnerCanva()
    {
        _winnerCanva.SetActive(true);
        _gameOverText.text = "We have a Winner";
        //_winnerName = ;

    }
    private void ShowLoseCanva()
    {
        _winnerCanva.SetActive(false);
        _tieCanva.SetActive(true);
        _gameOverText.text = "FriendShip";
    }
    private bool CheckWinner()
    {
        int minEnemyDeaths = int.MaxValue;
        int numPlayersWithMinDeaths = 0;
        bool isTie = false;
        bool hasWinner = false;

        // Verifica o menor número de mortes entre os inimigos
        foreach (var enemyHud in EnemyPlayerHud)
        {
            if (enemyHud.active)
            {
                TextMeshProUGUI deathNumEnemy = enemyHud.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
                int enemyDeaths = int.Parse(deathNumEnemy.text);
                Debug.Log("Loucura");
                if (enemyDeaths < minEnemyDeaths)
                {
                    minEnemyDeaths = enemyDeaths;
                    numPlayersWithMinDeaths = 0; // Reseta a contagem se encontrar um menor valor
                    isTie = false; // Reseta o empate
                    _winnerCanva.SetActive(true);
                    _winnerName.text = enemyHud.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text;
                    _winnerImage.sprite = enemyHud.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
                }
                else if (enemyDeaths == minEnemyDeaths)
                {
                    numPlayersWithMinDeaths++;
                    isTie = true; // Marca empate se encontrar outro com o mesmo número de mortes
                }
                else
                {
                    isTie = false; // Se encontrar um número maior, não há empate
                }
                
            }

        }
        if (isTie)
        {
            Debug.Log("O jogo terminou em empate.");
            return false;
        }
        if (_numPlayerDeath < minEnemyDeaths)
        {
            Debug.Log("Tem um vencedor");
            hasWinner = true;
            _winnerName.text = PlayerHud.transform.GetChild(0).GetChild(3).GetComponent<TextMeshProUGUI>().text;
            _winnerImage.sprite = PlayerHud.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite;
        }
        else if (_numPlayerDeath == minEnemyDeaths)
        {
            // Se o jogador tem o mesmo número de mortes que o menor dos inimigos, é empate
            Debug.Log("O jogo terminou em empate.");
            return false;
        }

        return hasWinner;
    }

    // Update is called once per frame
    void Update()
    {
        if (Cronometer.text == "0:00")
            ShowGameOver();

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
            _deathNumPlayer.text = _numPlayerDeath.ToString();
            _damagePercentagePlayer.text = actualizeDamage.ToString() + "%";
        }
        else
        {
            _damagePercentagePlayer.text = actualizeDamage.ToString() + "%";
        }
    }
    #endregion
}