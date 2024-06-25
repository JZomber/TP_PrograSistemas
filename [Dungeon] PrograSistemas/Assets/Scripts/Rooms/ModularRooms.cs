using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ModularRooms : MonoBehaviour
{
    [SerializeField] private Exit[] exits = new Exit[4];

    [SerializeField] private Enemy[] enemies = new Enemy[4];

    [SerializeField] private List<GameObject> doorsList;
    
    [SerializeField] private List<Transform> enemySpawnsList;

    private int maxEnemiesAmount;
    
    public static event Action<ModularRooms> OnPlayerEnteredRoom;
    
    //public static event Action<ModularRooms> OnRoomUnlocked; //Si tiene que pasar algo cuando se desbloquea una sala.
    
    public event Action<List<GameObject>, List<Transform>> OnSpawnEnemiesRequest;

    [SerializeField] private RoomConfig roomConfig;
    
    [SerializeField] private Collider2D roomTrigger;

    [SerializeField] private GameObject easyRoomDesign;
    [SerializeField] private GameObject hardRoomDesign;
    
    [SerializeField] private bool unlocked;
    [SerializeField] private bool playerSpawn;
    
    private enum Difficulty { Easy, Hard }
    private Difficulty selectedDifficulty;
    
    private int maxDifficultyPoints;

    private List<GameObject> enemiesSelected;
    private List<Transform> enemiesSelectedSpawns;

    private void Awake()
    {
        enemiesSelected = new List<GameObject>();
        enemiesSelectedSpawns = new List<Transform>();
        
        if (Application.isPlaying)
        {
            UpdateExits();
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        maxEnemiesAmount = enemySpawnsList.Count;
        
        if (Application.isPlaying)
        {
            UpdateExits();

            if (!playerSpawn)
            {
                SelectDifficulty();
                EnemySelector();
            }
        }
    }

    public void UnlockRoom()
    {
        unlocked = true;
        UpdateExits();
        roomTrigger.enabled = !unlocked;
        //OnRoomUnlocked?.Invoke(this); //Si tiene que pasar algo cuando se desbloquea una sala.
    }

    private void SelectDifficulty()
    {
        int randomValue = Random.Range(0, 2);
        selectedDifficulty = (randomValue == 0) ? Difficulty.Easy : Difficulty.Hard;

        if (selectedDifficulty == Difficulty.Easy)
        {
            maxDifficultyPoints = roomConfig.easyDiffPoints;
            //Debug.Log($"SALA {this} | DIFICULTAD {selectedDifficulty} | PUNTOS {maxDifficultyPoints}");
            
            hardRoomDesign.SetActive(false);
            easyRoomDesign.SetActive(true);
        }
        else
        {
            maxDifficultyPoints = roomConfig.hardDiffPoints;
            //Debug.Log($"SALA {this} | DIFICULTAD {selectedDifficulty} | PUNTOS {maxDifficultyPoints}");

            hardRoomDesign.SetActive(true);
            easyRoomDesign.SetActive(false);
        }
    }

    private void EnemySelector()
    {
        var filteredEnemies = enemies;

        //enemies.Where(e => selectedDifficulty == Difficulty.Hard || !e.isHardOnly);
        
        filteredEnemies.Shuffle();

        int totalDiffPoints = 0;
        int attempts = 0;

        // for(int i = 0; i < filteredEnemies.Length; i++)
        // {
        //     Debug.Log($"Nombre: {filteredEnemies[i].enemyPrefab.name} | Posición: {i}");
        // }

        while (totalDiffPoints != maxDifficultyPoints && attempts < filteredEnemies.Length)
        {
            foreach (var enemy in filteredEnemies)
            {
                if (enemiesSelected.Count < maxEnemiesAmount && totalDiffPoints + enemy.diffPoints <= maxDifficultyPoints)
                {
                    enemiesSelected.Add(enemy.enemyPrefab);
                    totalDiffPoints += enemy.diffPoints;
                }
                else if (totalDiffPoints + enemy.diffPoints > maxDifficultyPoints)
                {
                    continue;
                }

                if (totalDiffPoints == maxDifficultyPoints)
                {
                    break;
                }
            }
            
            attempts++;
        }
        
        enemiesSelectedSpawns = enemySpawnsList.Take(enemiesSelected.Count).ToList();
    }

    private void PlayerEnteredNewRoom()
    {
        OnSpawnEnemiesRequest?.Invoke(enemiesSelected, enemiesSelectedSpawns);
    }

    private void UpdateExits()
    {
        foreach (Exit exit in exits)
        {
            if (exit != null && exit.exitObject && exit.wallObject)
            {
                exit.exitObject.SetActive(exit.available);
                exit.wallObject.SetActive(!exit.available);

                foreach (GameObject door in doorsList)
                {
                    if (door)
                    {
                        door.SetActive(!unlocked);
                    }
                }

                if (exit.teleportTrigger && exit.teleportPosition)
                {
                    TeleportTrigger teleportTrigger = exit.teleportTrigger.GetComponent<TeleportTrigger>();
                    
                    teleportTrigger.Initialize(exit.teleportPosition);
                }
            }
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !unlocked)
        {
            OnPlayerEnteredRoom?.Invoke(this);
            PlayerEnteredNewRoom();
        }
    }
    
    #if UNITY_EDITOR
    private void HandlerUpdateExits() //Utilizado para evitar llamadas al mismo frame que se actualiza el editor
    {
        EditorApplication.update -= HandlerUpdateExits;
        UpdateExits();
    }
    #endif
    
    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            EditorApplication.update += HandlerUpdateExits;
        }
    }
    #endif
    
    [Serializable]
    public class Exit
    {
        public Direction direction;
        public bool available;
        public GameObject exitObject;
        public GameObject wallObject;
        public Collider2D teleportTrigger;
        public Transform teleportPosition;
    }
    
    [Serializable]
    public class Enemy
    {
        public GameObject enemyPrefab;
        public int diffPoints;
        //public bool isHardOnly;
    }
    
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
