using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] HapticProbeFPS controller;
    [SerializeField] Camera FirstPersonCamera;

    [Header("Setup")]
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;

    [Header("Effects")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;
    private Animator weaponAnimator;


    [Header("Recoil settings")]
    [SerializeField] public float recoilX = -5;
    [SerializeField] public float recoilY = 2;
    [SerializeField] public float recoilZ = 0.35f;
    [SerializeField] public float snappiness = 20;
    [SerializeField] public float returnSpeed = 6;
    [SerializeField] private float recoilHapticIntensity = 5f;
    private Recoil recoil;

    [Header("Bullet Spread")]
    [SerializeField] public float spreadFactor = 0.1f;




    void Start() {
        recoil = GameObject.Find("CameraRecoil").GetComponent<Recoil>();
        weaponAnimator = GetComponent<Animator>();

    }

    void Update()
    {
        //Uso come input il falcon
        if (controller.isActive() && controller.buttonWasPressed(0))
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

        StartCoroutine(recoilAnimation());
        recoil.RecoilFire();
        ProcessRaycast();
    }

    IEnumerator recoilAnimation() {
        weaponAnimator.Play("Recoil");
        yield return new WaitForSeconds(0.1f);
        weaponAnimator.Play("Idle");
    }


    private void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }


    private void ProcessRaycast()
    {
        RaycastHit hit;

        //Spread bullets
        Vector3 shootDirection = FirstPersonCamera.transform.forward;
        float randomSpreadX = Random.Range(-spreadFactor, spreadFactor);
        float randomSpreadY = Random.Range(-spreadFactor, spreadFactor);
        shootDirection = shootDirection + FirstPersonCamera.transform.TransformDirection(new Vector3(randomSpreadX, randomSpreadY));


        if (Physics.Raycast(FirstPersonCamera.transform.position, shootDirection, out hit, range))
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
