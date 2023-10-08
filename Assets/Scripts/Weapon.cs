using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] HapticProbeFPS controller;
    [SerializeField] Camera FirstPersonCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;
    private float recoilHapticIntensity = 4f;

    void Update()
    {
        //Uso come input il falcon
        if (controller.isActive() && controller.getButtonState(0))
        {
            Shoot();
        }
        //Uso come input il mouse
        else if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        PlayMuzzleFlash();
        if (controller.isActive()) 
            StartCoroutine(controller.recoilHapticFeedback(recoilHapticIntensity));       
        ProcessRaycast();
    }
    private void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }


    private void ProcessRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(FirstPersonCamera.transform.position, FirstPersonCamera.transform.forward, out hit, range))
        {
            Debug.Log("Colpito: " + hit.transform.name);
            HitImpact(hit);
            EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();
            //evito NullReference se colpisco un oggetto senza lo script EnemyHealth 
            if (target == null) return;
            target.TakeDamage(damage);
        }

    }

    private void HitImpact(RaycastHit hit)
    {
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        impact.SetActive(true);
        Destroy(impact, .1f);
    }
}
