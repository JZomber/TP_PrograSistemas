using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] Transform shootingOrig; //Origen de las balas
    [SerializeField] GameObject bulletPrefab; //Prefab de las balas (player)
    [SerializeField] GameObject weapon;
    WeaponScript weaponScript;

    StopTime stopTime;

    public bool isPowerActive; //Poder de disparo
    private float timePowerUp = 6f; //Duraci�n del power up

    public bool canShoot = true;
    

    private void Start()
    {
        weaponScript = weapon.GetComponent<WeaponScript>();
        stopTime = GetComponent<StopTime>();

        if (stopTime == null) 
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isPowerActive) //Si el poder est� activo
        {
            var dt = Time.deltaTime;
            timePowerUp -= dt;
            
            if (timePowerUp <= 0f)
            {
                isPowerActive = false;
                timePowerUp = 6f;
                //Debug.LogError("Power Shoot desactivado");
            }
        }

        if (stopTime != null)
        {
            if (!stopTime.GetPowerActive)
            {
                if (Mouse.current.leftButton.wasPressedThisFrame && canShoot) //Cada vez que se presione el mouse
                {
                    if (weaponScript != null)
                    {
                        weaponScript.Shoot(shootingOrig);
                    }
                    //StartCoroutine(PlayerShooting(bulletPrefab, shootingOrig, 0.15f)); //Prefab, Origen, Delay
                }
            }

        }
        else 
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && canShoot) //Cada vez que se presione el mouse
            {
                if (weaponScript != null)
                {
                    weaponScript.Shoot(shootingOrig);
                }
                //StartCoroutine(PlayerShooting(bulletPrefab, shootingOrig, 0.15f)); //Prefab, Origen, Delay
            }
        }
        
        
        
        
        if (!canShoot)
        {
         weapon.GameObject().SetActive(false);
        }
    }

    private IEnumerator PlayerShooting(GameObject prefab, Transform orig, float delay)
    {
        var rotation = orig.rotation;
        rotation *=  Quaternion.Euler(0, 0, -90);
        Instantiate(prefab, orig.position, rotation);
            
        if (isPowerActive) //Si el poder est� activo, instancia otra bala
        {
            yield return new WaitForSeconds(delay);
            Instantiate(prefab, orig.position, rotation);
        }
    }
}
