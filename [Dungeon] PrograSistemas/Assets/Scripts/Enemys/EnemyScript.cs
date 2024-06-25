using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    
    [Header("Enemy Attributes")] 
    public float health;
    private float currentHealth; 
    public float speed;
    public bool isAlive = true;
    public bool isRangedEnemy;
    private Animator animator;

    private CapsuleCollider2D capsuleCollider2D;
    
    [Header("Player")] public GameObject player;
    private float distance;

    private LevelManager levelManager;
    private EnemyMage enemyMage;
    private RangedEnemy rangedEnemy;

    public event Action<GameObject> OnEnemyKilled;
    
    private void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        capsuleCollider2D.isTrigger = false;
        animator = GetComponent<Animator>();

        if (isRangedEnemy)
        {
            rangedEnemy = GetComponent<RangedEnemy>();
        }

        currentHealth = health;
    }

    void Update()
    {
        if (player)
        {
            // Perseguir al Jugador
            distance = Vector2.Distance(transform.position, player.transform.position);
        
            if (distance < 20)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
            }
        }
    }

    // Daño del Enemigo
    public void EnemyDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            levelManager.enemyCounter++;
            capsuleCollider2D.enabled = false;
            animator.SetTrigger("isDead");
            isAlive = false;

            if (OnEnemyKilled != null)
            {
                OnEnemyKilled(this.GameObject());
            }
            
            if (isRangedEnemy)
            {
                rangedEnemy.canShoot = false;
                rangedEnemy.isWeaponActive = false;
                StartCoroutine(rangedEnemy.UpdateWeaponStatus(0f));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Shield"))
        {
            levelManager.enemyCounter++;
            capsuleCollider2D.enabled = false;
            animator.SetTrigger("isDead");
            isAlive = false;

            if (OnEnemyKilled != null)
            {
                OnEnemyKilled(this.GameObject());
                Debug.Log("EVENT");
            }

            if (isRangedEnemy)
            {
                rangedEnemy.canShoot = false;
                rangedEnemy.isWeaponActive = false;
                StartCoroutine(rangedEnemy.UpdateWeaponStatus(0f));
            }
        }
    }

    public IEnumerator EnemyRevive(float delay)
    {
        if (!isAlive)
        {
            levelManager.enemyCounter--;
            isAlive = true;
            currentHealth = health;
            animator.SetTrigger("isRevived");
            
            if (isRangedEnemy)
            {
                rangedEnemy.isWeaponActive = true;
                StartCoroutine(rangedEnemy.UpdateWeaponStatus(0.5f));
            }
            
            yield return new WaitForSeconds(delay); //Delay antes de que se pueda volver a atacar

            capsuleCollider2D.enabled = true;
            rangedEnemy.canShoot = true;
        }
    }
}