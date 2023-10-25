using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] HapticProbeFPS controller;
    [SerializeField] Camera FirstPersonCamera;
    [SerializeField] private Crosshair crosshair;
    [SerializeField] private FirstPersonControllerFalcon player;

    [Header("Setup")]
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;

    [Header("Effects")]
    [SerializeField] ParticleSystem muzzleFlash;
    [SerializeField] GameObject hitEffect;
    [SerializeField] LineRenderer bulletTrail;
    [SerializeField] Transform bulletTrailShootPoint;
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
    [SerializeField] public float spreadFactor;
    private float initialSpreadFactor;




    void Start() {
        recoil = GameObject.Find("CameraRecoil").GetComponent<Recoil>();
        weaponAnimator = GetComponent<Animator>();
        initialSpreadFactor = spreadFactor;
    }

    void Update()
    {

        setSpreadFactor();
        crosshair.setCrosshairSize(spreadFactor);
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

    public void setSpreadFactor()
    {
        //il player è fermo
        if (player.m_CharacterController.velocity.sqrMagnitude.Equals(0) && (Input.GetAxis("Horizontal") == 0 || 
            Input.GetAxis("Vertical") == 0))
        {
            spreadFactor = initialSpreadFactor;
        }
        //il player sta correndo
        else if (Input.GetKey(KeyCode.LeftShift) && player.m_CharacterController.velocity.sqrMagnitude > 0 && 
            (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            spreadFactor = initialSpreadFactor * 4f;
        }

        //il player sta camminando
        else
        {
            spreadFactor = initialSpreadFactor * 2.5f;
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

        Vector3 shootDirection = SpreadBullets();

        Vector3 shootPoint = FirstPersonCamera.transform.position;

        //hitpoint se non colpisco nulla
        Vector3 hitPoint = FirstPersonCamera.transform.forward*range;

        if (Physics.Raycast(shootPoint, shootDirection, out hit, range))
        {
            Debug.Log("Colpito: " + hit.transform.name);
            HitImpactEffect(hit);

            ProcessDamage(hit);

            hitPoint = hit.point;
        }

        SpawnBulletTrail(hitPoint);
    }

    private void ProcessDamage(RaycastHit hit)
    {
        EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();

        if (target)
        {
            target.TakeDamage(damage);
        }
    }

    private Vector3 SpreadBullets()
    {
        Vector3 shootDirection = FirstPersonCamera.transform.forward;
        float randomSpreadX = Random.Range(-spreadFactor, spreadFactor);
        float randomSpreadY = Random.Range(-spreadFactor, spreadFactor);
        shootDirection = shootDirection + FirstPersonCamera.transform.TransformDirection(new Vector3(randomSpreadX, randomSpreadY));
        return shootDirection;
    }

    private void SpawnBulletTrail(Vector3 hitPoint) {

        GameObject bulletTrailEffect = Instantiate(bulletTrail.gameObject, bulletTrailShootPoint.position, Quaternion.identity);

        LineRenderer lineR = bulletTrailEffect.GetComponent<LineRenderer>();

        lineR.SetPosition(0, bulletTrailShootPoint.position);
        lineR.SetPosition(1, hitPoint);

        Destroy(bulletTrailEffect, .2f);
    }

    private void HitImpactEffect(RaycastHit hit)
    {
        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
        impact.SetActive(true);
        Destroy(impact, .1f);
    }
}
