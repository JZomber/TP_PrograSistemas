using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Update = UnityEngine.PlayerLoop.Update;

public class EnemyMage : MonoBehaviour
{
    [Header("Enemy Attributes")] 
    public int health;
    private int currentHealth;
    public bool isAlive;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject enemyShield;

    [SerializeField] private EnemyManager enemyManager;

    private CapsuleCollider2D capsuleCollider2D;

    private Vector2 spawnPoint;

    private bool isReviving;

    // Start is called before the first frame update
    void Start()
    {
        isAlive = true;
        currentHealth = health;
        
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        capsuleCollider2D.enabled = false;
        
        enemyShield.SetActive(true);

        spawnPoint = transform.position;

        if (enemyManager != null && isAlive)
        {
            enemyManager.OnMageCalled += MoveToTarget;
            Debug.Log($"{gameObject.name} SE HA SUBSCRITO AL EVENTO OnMageCalled");
        }
        else
        {
            Debug.LogError($"OBJ: {gameObject.name} | REFERENCIA {enemyManager} NO ENCONTRADA");
        }
    }

    private void MoveToTarget(GameObject target)
    {
        Debug.Log($"MOVIENDO HACIA {target}");

        isReviving = true;
        UpdateColliders();
        
        gameObject.transform.position = target.transform.position + new Vector3(0, 1, 0);
        StartCoroutine(ReviveTarget(2, target));
    }

    private IEnumerator ReviveTarget(float delay, GameObject target)
    {
        yield return new WaitForSeconds(delay);
        
        StartCoroutine(target.GetComponent<EnemyScript>().EnemyRevive(3f));

        StartCoroutine(Relocate(1f));
    }

    private void UpdateColliders() //Controla cuando el mago/necro es vulnerable o no
    {
        if (isReviving)
        {
            enemyShield.SetActive(false);
            capsuleCollider2D.enabled = true;
        }
        else
        {
            enemyShield.SetActive(true);
            capsuleCollider2D.enabled = false;
        }
    }

    private IEnumerator Relocate(float delay)
    {
        if (isAlive)
        {
            yield return new WaitForSeconds(delay);
            isReviving = false;
            UpdateColliders();
        
            gameObject.transform.position = spawnPoint;
        }
    }
    
    public void EnemyDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            capsuleCollider2D.enabled = false;
            animator.SetTrigger("isDead");
            isAlive = false;
            
            enemyManager.OnMageCalled -= MoveToTarget;
        }
    }
}
