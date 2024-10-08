using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WeaponScript : MonoBehaviour,IGun
{
    [SerializeField] private WeaponData weaponData;
    private SpriteRenderer spriteRenderer;
    private AudioSource myAudio;
    private List<GameObject> bullets = new List<GameObject>();
    private int poolSize = 3;
    private bool isShooting = false;

    public UnityEvent OnShoot = new UnityEvent();
    public event Action<Sprite> OnSpriteChanged;
    
    private void Start()
    {
        bullets = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(weaponData.GetBulletPrefab);
            bullet.SetActive(false);
            bullets.Add(bullet);
        }
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null) return;

        myAudio = GetComponent<AudioSource>();

        if (myAudio == null) return;

        ChangeWeaponSprite();
    }

    public void Shoot(Transform orig)
    {
        if (!isShooting)
        {
            if (weaponData.GetBulletsPerShoot > 1) 
            {
                ShootShotgun(orig);
            }
            else if (weaponData.GetRoundsBullets > 0)
            {
                StartCoroutine(ShootSmg(weaponData.GetCadency, orig));
            }
            else
            {
                GameObject bullet = GetPooledBullet();
                if (bullet != null)
                {
                    bullet.transform.position = orig.position;
                    bullet.transform.rotation = orig.rotation * Quaternion.Euler(0, 0, -90);
                    bullet.SetActive(true);
                }
                
                OnShoot.Invoke();
                 PutShootSound();
            }
        }
    }

    public void ShootShotgun(Transform orig)
    {
        if (!isShooting)
        {
            StartCoroutine(ShootShotgunCoroutine(orig));
        }
    }

    private IEnumerator ShootShotgunCoroutine(Transform orig)
    {
        isShooting = true;

        float angleStep = weaponData.GetSpreadAngle / (weaponData.GetBulletsPerShoot - 1);
        float angle = -weaponData.GetSpreadAngle / 2;

        for (int i = 0; i < weaponData.GetBulletsPerShoot; i++)
        {
            GameObject bullet = GetPooledBullet();
            if (bullet != null)
            {
                bullet.transform.position = orig.position;
                bullet.transform.rotation = orig.rotation * Quaternion.Euler(0, 0, angle - 90);
                bullet.SetActive(true);
            }
            angle += angleStep;
        }
        PutShootSound();

        OnShoot.Invoke();
        yield return new WaitForSeconds(weaponData.GetCadency);

        isShooting = false;
    }

    private GameObject GetPooledBullet()
    {
        foreach (GameObject bullet in bullets)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }
        GameObject newBullet = Instantiate(weaponData.GetBulletPrefab);
        newBullet.SetActive(false);
        bullets.Add(newBullet);
        return newBullet;
    }

    private IEnumerator ShootSmg(float delay, Transform orig)
    {
        isShooting = true; 

        for (int i = 0; i < weaponData.GetRoundsBullets; i++)
        {
            GameObject bullet = GetPooledBullet();
            if (bullet != null)
            {
                bullet.transform.position = orig.position;
                bullet.transform.rotation = orig.rotation * Quaternion.Euler(0, 0, -90);
                bullet.SetActive(true);
            }

            PutShootSound();

            OnShoot.Invoke();
            yield return new WaitForSeconds(delay);
        }
        
        isShooting = false;
    }

    public void ChangeWeaponSprite() 
    {
        if(spriteRenderer != null && weaponData != null) 
        {
            spriteRenderer.sprite = weaponData.GetWeaponSprite;
            OnSpriteChanged?.Invoke(weaponData.GetweaponUiSprite);
        }
        PutReload();
    }

    public void PutReload() 
    {
        if (weaponData.GetReloadSound != null && myAudio != null) 
        { 
            myAudio.PlayOneShot(weaponData.GetReloadSound); 
        }   
    }

    public void PutShootSound() 
    {
        if (weaponData.GetShotSound != null && myAudio != null)
        {
            myAudio.PlayOneShot(weaponData.GetShotSound);
        }
    }

    public void ChangeWeaponData(WeaponData weaponData) 
    {
        this.weaponData = weaponData;
        ChangeWeaponSprite();
    }
}
