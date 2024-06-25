using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [SerializeField] private Transform[] shootingOrig; //Origen de las balas
    [SerializeField] private GameObject bulletPrefab; //Prefab de bullet (enemy)
    private int totalShootOrigin; //Total de orígenes (depende del arma)

    private float coolDown;
    [SerializeField] private float shootCoolDown;

    [SerializeField] private GameObject weapon; //Prefab del arma
    WeaponScript weaponScript;
    private bool isSmg;
    public bool canShoot = true;
    public bool isWeaponActive = true;


    // Start is called before the first frame update
    void Start()
    {
        if (weapon.GameObject().name == "BasicSMG")
        {
            isSmg = true;
        }
        
        coolDown = shootCoolDown; //Cooldown entre disparos

        totalShootOrigin = weapon.transform.childCount; //Obtengo el número de orígenes de balas
        shootingOrig = new Transform[totalShootOrigin]; //Inicializo la lista según la cantidad

        for (int i = 0; i < totalShootOrigin; i++)
        {
            shootingOrig[i] = weapon.transform.GetChild(i).transform; //Obtengo la posición de cada origen
        }
        
        weaponScript = weapon.GetComponent<WeaponScript>();

    }
    
    // Update is called once per frame
    void Update()
    {
        coolDown -= Time.deltaTime;

        if (coolDown <= 0f && canShoot)
        {
            
            for (int i = 0; i < shootingOrig.Length; i++)
            {
                if (weaponScript != null) 
                {
                    weaponScript.Shoot(shootingOrig[i]);
                }
               
                
            }
            
            coolDown = shootCoolDown;
<<<<<<< HEAD
        }      
=======
        }
        else if (coolDown <= 0f && canShoot && isSmg)
        {
            StartCoroutine(ShootSMG(0.2f)); //Delay entre disparos
            
            coolDown = shootCoolDown;
        }
    }

    private IEnumerator ShootSMG(float delay)
    {
        for (int i = 0; i < 3; i++) //Cantidad de veces que disparará el arma (3 disparos - efecto ráfaga)
        {
            yield return new WaitForSeconds(delay);
        
            var rotation = shootingOrig[0].rotation;
            rotation *= Quaternion.Euler(0, 0, -90);
            Instantiate(bulletPrefab, shootingOrig[0].position, rotation);
        }
>>>>>>> parent of 0bc23d7 (Enemy Pool completed.)
    }
   

    public IEnumerator UpdateWeaponStatus(float delay)
    {

        yield return new WaitForSeconds(delay);
        
        if (!canShoot && !isWeaponActive)
        {
            weapon.GameObject().SetActive(false);
        }
        else if (!canShoot && isWeaponActive)
        {
            weapon.GameObject().SetActive(true);
        }
    }
}
