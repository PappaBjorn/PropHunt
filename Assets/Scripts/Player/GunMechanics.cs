using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class GunMechanics : MonoBehaviour
{
    public float damage = 10f;
    public float selfDamage = 5f;
    public float range = 100f;
    public float maxAmmo = 7;
    public float currentAmmo;

    public Camera camera;
    public GameObject muzzleFlash;
    private GameObject muzzle = null;
    private ParticleSystem muzzleFlashParticle = null;

    private void Start()
    {
        muzzle = this.gameObject;
        muzzleFlashParticle = muzzleFlash.GetComponent<ParticleSystem>();
        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0)
        {
            Shoot();
            muzzleFlashParticle.Play();
        }

        if (Input.GetKeyDown("r") && currentAmmo < maxAmmo)
            ReloadWeapon();
    }

    void Shoot()
    {
        currentAmmo--;
        Debug.Log("currentAmmo = " + currentAmmo);
        
        RaycastHit hit;
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, range))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                IHealth temp = hit.collider.gameObject.GetComponentInParent<IHealth>();
                temp?.TakeDamage(damage);
                Debug.Log("Damage done, remaining health: " + hit.collider.gameObject.GetComponentInParent<IHealth>());
                //hit.collider.gameObject.GetComponent<PlayerMovement>().TakeDamage(damage) = true;
            }
            Debug.DrawLine(muzzle.transform.position, hit.transform.position, Color.red);
            Debug.Log(hit.transform.name);
            Debug.Log(hit.transform.tag);

            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Prop"))
            {
                System.Diagnostics.Debug.Assert(transform != null, nameof(transform) + " != null");
                IHealth temp = this.transform.gameObject.GetComponentInParent<IHealth>();
                //Debug.Log("Hunter IHealth " + this.transform.gameObject.GetComponentInParent<HealthComponent>().Health);
                temp?.TakeDamage(selfDamage);
                //transform.root.gameObject.GetComponent<PlayerMovement>().TakeDamage(selfDamage);
            }
        }
    }

    void ReloadWeapon()
    {
        Debug.Log("Trying to reload");
        if (currentAmmo < maxAmmo)
            currentAmmo = maxAmmo;
    }
}
