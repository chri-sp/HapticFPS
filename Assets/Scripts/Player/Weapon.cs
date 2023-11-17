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
    public bool hasShooted = false;

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
    [SerializeField] public float spreadShoot;
    [SerializeField] public float maxSpreadAfterShoot = 0.05f;
    private float resetSpreadDelay = .2f;
    private float resetSpreadTimer;
    private float initialSpreadFactor;
    private float surplusSpreadAfterShoot;


    void Start() {
        recoil = GameObject.Find("CameraRecoil").GetComponent<Recoil>();
        weaponAnimator = GetComponent<Animator>();
        initialSpreadFactor = spreadFactor;
        resetSpreadTimer = resetSpreadDelay;
    }

    void Update()
    {
        spreadAfterShoootTimer();
       
        setSpreadFactor();
        crosshair.setCrosshairSize(spreadFactor);
        //Uso come input il falcon
        if (controller.isActive() && controller.buttonWasPressed(0))
        {
            Shoot();
            IncreaseSpreadAfterShoot();
            resetSpreadTimer = resetSpreadDelay;
        }
        //Uso come input il mouse
        else if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
            IncreaseSpreadAfterShoot();
            resetSpreadTimer = resetSpreadDelay;
        }
    }

    public void setSpreadFactor()
    {
        //il player è fermo
        if (player.m_CharacterController.velocity.sqrMagnitude.Equals(0) && (Input.GetAxis("Horizontal") == 0 ||
            Input.GetAxis("Vertical") == 0))
        {
            spreadFactor = initialSpreadFactor + surplusSpreadAfterShoot;
        }
        //il player sta correndo
        else if (Input.GetKey(KeyCode.LeftShift) && player.m_CharacterController.velocity.sqrMagnitude > 0 &&
            (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            spreadFactor = initialSpreadFactor * 4f + surplusSpreadAfterShoot;
        }

        //il player sta camminando
        else
        {
            spreadFactor = initialSpreadFactor * 2.5f + surplusSpreadAfterShoot;
        }

    }

    IEnumerator HasShooted() {
        hasShooted = true;
        yield return new WaitForSeconds(1f);
        hasShooted = false;
    }

    private void Shoot()
    {
        StartCoroutine(HasShooted());
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

        Vector3 hitPoint;

        Vector3 shootPoint = bulletTrailShootPoint.position;

        Vector3 shootDirection = SpreadBullets();

        //trovo il punto di impatto
        if (Physics.Raycast(FirstPersonCamera.transform.position, shootDirection, out hit, range))
        {
            hitPoint = hit.point;
        }
        else {
            //hitpoint se non colpisco nulla
            hitPoint = transform.position + FirstPersonCamera.transform.forward * range / 3;
        }

        //verifico raycast partendo dall'arma in direzione del punto di impatto
        shootDirection = hitPoint - shootPoint;
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
        EnemyHealth target = hit.transform.root.GetComponent<EnemyHealth>();

        if (target)
        {
            target.TakeDamage(damage);
        }
    }


    void spreadAfterShoootTimer(){
        if (resetSpreadTimer <= 0)
        {
            surplusSpreadAfterShoot = 0;
        }

        if (resetSpreadTimer > 0)
        {
            resetSpreadTimer -= Time.deltaTime;
        }
    }

    private void IncreaseSpreadAfterShoot() {
        if (surplusSpreadAfterShoot <= maxSpreadAfterShoot)
        {
            surplusSpreadAfterShoot = spreadFactor + Random.Range(spreadShoot, spreadShoot * 2);
        }
    }

    private Vector3 SpreadBullets()
    {
        Vector3 shootDirection = FirstPersonCamera.transform.forward;
        float randomSpreadX = Random.Range(-spreadFactor, spreadFactor);
        float randomSpreadY = Random.Range(-spreadFactor, spreadFactor*2);
        shootDirection = shootDirection + FirstPersonCamera.transform.TransformDirection(new Vector3(randomSpreadX, randomSpreadY));
        return shootDirection;
    }

    private void SpawnBulletTrail(Vector3 hitPoint) {

        GameObject bulletTrailEffect = Instantiate(bulletTrail.gameObject);

        LineRenderer lineR = bulletTrailEffect.GetComponent<LineRenderer>();

        lineR.positionCount = 2;

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
