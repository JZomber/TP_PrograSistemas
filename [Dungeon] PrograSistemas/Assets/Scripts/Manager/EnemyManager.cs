using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private EnemyScript[] enemyScripts;

    public event Action<GameObject> OnMageCalled;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyScripts = FindObjectsOfType<EnemyScript>();
        
        foreach (EnemyScript enemy in enemyScripts)
        {
            enemy.OnEnemyKilled += EnemyKilledHandler;
        }
    }
    
    private void EnemyKilledHandler(GameObject obj)
    {
        //Debug.Log($"ENEMIGO {obj} ELIMINADO");
        
        if (OnMageCalled != null)
        {
            OnMageCalled(obj);
        }
    }

    private void OnDestroy()
    {
        foreach (EnemyScript enemy in enemyScripts)
        {
            enemy.OnEnemyKilled -= EnemyKilledHandler;
        }
    }
}
