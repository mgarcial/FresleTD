using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public enum Events
{
    EngineerEvent,
    StrikeEvent,
    ClimateEvent,
    TerroristEvent,
    SurvivorsEvent
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        enemyManager = EnemyManager.instance;
        eventManager = EventManager.instance;
    }

    [SerializeField] private int maxHealth;
    [SerializeField] private int initialMoney;
    private int health;
    public int circuitos;
    public TMP_Text circuitosDisplay;
    public GameObject grid;
    public CustomCursor CC;
    public Tile[] tiles;

    private Tower bAColocar;

    private int goldRate = 0;

    private float _gameSpeed;

    private EnemyManager enemyManager;
    private EventManager eventManager;

    public int Circuitos
    {
        get { return goldRate; }
        set { goldRate += value; }
    }

    private void Start()
    {
        _gameSpeed = 1f;
        health = maxHealth;
        Debug.Log($"base has {health} health points left");
        circuitos = initialMoney;

        if(eventManager == null)
        {
            Debug.Log("hay un error");
        }
        eventManager.OnTimeChange += SetGameTime;
    }

    void Update(){
        circuitosDisplay.text = "Recursos: " + circuitos.ToString();

        if(Input.GetMouseButtonDown(0) && bAColocar != null){
            Tile nearestTile = null;
            float shortestDistance = 100;
            foreach(Tile tile in tiles){
                float dist = Vector2.Distance(tile.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

                if(dist < shortestDistance){
                    shortestDistance = dist;
                    nearestTile = tile;
                }
            }

            if (!nearestTile.isOccupied)
            {
                Instantiate(bAColocar, nearestTile.transform.position, Quaternion.identity);
                nearestTile.isOccupied = true;
                grid.SetActive(false);
                CC.gameObject.SetActive(false);
                Cursor.visible = true;
                bAColocar = null;
            }
        }
    }

    public void BuyBuilding(Tower b){
        if (circuitos >= b.cost){
            Debug.Log("boton oprimido");
            GameObject Edificio = b.gameObject;
            CC.gameObject.SetActive(true);
            CC.setCursor(Edificio.GetComponent<SpriteRenderer>());
            Cursor.visible = false;
            circuitos -= b.cost;
            grid.SetActive(true);
            bAColocar = b;
        }
    }

    public void SetGameTime(TimeStates timeState)
    {
        switch (timeState)
        {
            case TimeStates.pause:
                Time.timeScale = 0;
                break;
            case TimeStates.unpause:
                Time.timeScale = 1;
                break;
        }
    }

    public void RestartLevel()
    {
        FindObjectOfType<LevelManager>().LoadCurrentScene();
        CleanLevel();
        Destroy(gameObject);
    }


    private void CleanLevel()
    {
        health = maxHealth;
        circuitos = initialMoney;
        enemyManager.ClearEnemiesList();
        _gameSpeed = 1f;
        SetGameSpeed(_gameSpeed);
        Debug.Log("LevelCleaned");
    }
    public void NextLevel()
    {
        CleanLevel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Destroy(gameObject);
    }
    public void ExitToMainMenu()
    {
        CleanLevel();
        FindObjectOfType<LevelManager>().LoadMainMenuScene();
        Destroy(gameObject);
    }
    public void SetGameSpeed(float speed)
    {
        _gameSpeed = speed;
        Time.timeScale = speed;
    }

    public void AddToMoney(int amount)
    {
        circuitos += amount;
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log($"the base has taken {dmg} damage, and has {health} hit points left");
        if(health <= 0)
        {
            health = 0;
            LooseLevel();
        }
    }

    private void LooseLevel()
    {
        RestartLevel();
    }

    public int GetCurrentMoney() => circuitos;

    private void WaveFinished()
    {
        
    }
}
