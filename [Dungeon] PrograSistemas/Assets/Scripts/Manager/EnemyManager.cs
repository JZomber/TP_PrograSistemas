using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyManager : MonoBehaviour
{
    private EnemyScript[] enemyScripts;

    private ModularRooms[] modularRooms;

    private EnemyMage[] enemyMages;

    [SerializeField] private List<EnemyPoolClass> enemyClass;
    private List<GameObject> enemyPool;

    private int activeEnemyAmount;

    public event Action<GameObject> OnMageCalled;

    public static event Action OnRoomCompleted;
    
    private void Awake()
    {
        enemyPool = new List<GameObject>();
        
        foreach (var poolItem in enemyClass)
        {
            for (int i = 0; i < poolItem.initialAmount; i++)
            {
                GameObject newEnemy = Instantiate(poolItem.enemyPrefab);
                newEnemy.SetActive(false);
                enemyPool.Add(newEnemy);

                EnemyScript enemyScript = newEnemy.GetComponent<EnemyScript>();
                EnemyMage enemyMage = newEnemy.GetComponent<EnemyMage>();

                if (enemyScript != null)
                {
                    enemyScript.OnEnemyKilled += HandlerEnemyKilled;
                    enemyScript.OnEnemyRevived += HandlerEnemyRevived;
                    Debug.Log($"Subscripto al enemigo {enemyScript.GameObject()}");
                }
                else if (enemyMage != null)
                {
                    enemyMage.OnMageKilled += HandlerEnemyKilled;
                    Debug.Log("Subscripto al mago");
                }
            }
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        modularRooms = FindObjectsOfType<ModularRooms>();

        foreach (ModularRooms rooms in modularRooms)
        {
            rooms.OnSpawnEnemiesRequest += HandleSpawnEnemies;
            //Debug.Log("Subscripto a SpawnRequest");
        }
    }

    private void HandleSpawnEnemies(List<GameObject> enemies, List<Transform> spawns)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            GameObject enemy = GetEnemyFromPool(enemies[i]);
            enemy.transform.position = spawns[i].position;
            enemy.SetActive(true);
            //enemy.GetComponent<EnemyScript>().OnEnemyKilled += HandlerEnemyDeath;
        }

        activeEnemyAmount = enemies.Count;
        Debug.Log($"Objetivo de enemigos {activeEnemyAmount}");
    }

    private GameObject GetEnemyFromPool(GameObject prefab)
    {
        for (int i = 0; i < enemyPool.Count; i++)
        {
            if (!enemyPool[i].activeInHierarchy && enemyPool[i].name.Contains(prefab.name))
            {
                return enemyPool[i];
            }
        }

        GameObject newEnemy = Instantiate(prefab);
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    private void DeactivateEnemies()
    {
        for (int i = 0; i < enemyPool.Count; i++)
        {
            GameObject enemy = enemyPool[i].GameObject();
            
            if (enemy.activeInHierarchy)
            {
                enemy.SetActive(false);
            }
        }
    }
    
    private void HandlerEnemyKilled(GameObject obj)
    {
        Debug.Log($"ENEMIGO {obj} ELIMINADO");

        OnMageCalled?.Invoke(obj);

        activeEnemyAmount--;

        if (activeEnemyAmount == 0)
        {
            OnRoomCompleted?.Invoke();
            DeactivateEnemies();
        }
    }

    private void HandlerEnemyRevived()
    {
        activeEnemyAmount++;
    }

    private void UnsubscribeFromEvents()
    {
        //OnSpawnEnemies -= HandleSpawnEnemies;
        if (modularRooms != null)
        {
            foreach (ModularRooms rooms in modularRooms)
            {
                rooms.OnSpawnEnemiesRequest -= HandleSpawnEnemies;
            }
        }

        if (enemyPool != null)
        {
            foreach (GameObject enemyObject in enemyPool)
            {
                if (enemyObject != null)
                {
                    EnemyScript enemyScript = enemyObject.GetComponent<EnemyScript>();
                    EnemyMage enemyMage = enemyObject.GetComponent<EnemyMage>();

                    if (enemyScript != null)
                    {
                        enemyScript.OnEnemyKilled -= HandlerEnemyKilled;
                        enemyScript.OnEnemyRevived -= HandlerEnemyRevived;
                        Debug.Log($"Desubscripto al enemigo {enemyScript.GameObject()}");
                    }
                    else if (enemyMage != null)
                    {
                        enemyMage.OnMageKilled -= HandlerEnemyKilled;
                        Debug.Log("Desubscripto al mago");
                    }
                    
                }
            }
        }
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void OnApplicationQuit()
    {
        UnsubscribeFromEvents();
    }

    [Serializable]
    public class EnemyPoolClass
    {
        public GameObject enemyPrefab;
        public int initialAmount;
    }
}
