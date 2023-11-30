using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    HapticProbeFPS controller;
    Camera FirstPersonCamera;
    private Crosshair crosshair;
    private FirstPersonControllerFalcon player;
    private Animator WeaponsAnimator;
    public bool hasShooted = false;
    private FloatingCircleReloading reloadingCircle;
    [SerializeField] private ReloadingWaitTimeCircle reloadingWaitTimeCircle;

    [Header("Setup")]
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;
    private float initialDamage;

    [Header("Effects")]
    ParticleSystem muzzleFlash;
    GameObject hitEffect;
    [SerializeField] LineRenderer bulletTrail;
    Transform bulletTrailShootPoint;
    private Animator weaponAnimator;

    public float fireRate = .13f;
    private float nextTimeToFire = 0f;

    [Header("Reload settings")]
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 1f;
    public bool isReloading = false;

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

    Coroutine reloadingCoroutine;

    void Start()
    {
        controller = GameObject.FindWithTag("Player").GetComponent<HapticProbeFPS>();
        FirstPersonCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        player = GameObject.FindWithTag("Player").GetComponent<FirstPersonControllerFalcon>();
        WeaponsAnimator = transform.parent.GetComponent<Animator>();
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
        hitEffect = transform.Find("Hit Impact VFX").gameObject;
        bulletTrailShootPoint = muzzleFlash.gameObject.transform;
        crosshair = GameObject.FindWithTag("Canvas").GetComponentInChildren<Crosshair>();
        recoil = GameObject.Find("CameraRecoil").GetComponent<Recoil>();
        weaponAnimator = GetComponent<Animator>();
        WeaponsAnimator.enabled = false;
        initialSpreadFactor = spreadFactor;
        resetSpreadTimer = resetSpreadDelay;
        currentAmmo = maxAmmo;
        reloadingCircle = gameObject.GetComponentInChildren<FloatingCircleReloading>();
        initialDamage = damage;
        //reloadingWaitTimeCircle = GameObject.FindWithTag("Canvas").GetComponent<ReloadingWaitTimeCircle>();
        reloadingWaitTimeCircle.gameObject.SetActive(false);
    }

    void Update()
    {
        spreadAfterShoootTimer();

        setSpreadFactor();
        crosshair.setCrosshairSize(spreadFactor);

        if (isReloading)
        {
            return;
        }

        if (currentAmmo <= 0f || (Input.GetKeyDown("r") && currentAmmo < maxAmmo))
        {
            reloadingCoroutine = StartCoroutine(Reload());
            return;
        }

        if (weaponAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            weaponInput();
        }   
    }

    private void weaponInput()
    {
        if (gameObject.name.Equals("Rifle"))
        {
            //Uso come input il falcon
            if (controller.isActive() && controller.getButtonState(0) && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
                IncreaseSpreadAfterShoot();
                resetSpreadTimer = resetSpreadDelay;
            }
            //Uso come input il mouse
            else if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + fireRate;
                Shoot();
                IncreaseSpreadAfterShoot();
                resetSpreadTimer = resetSpreadDelay;
            }
        }
        else
        {
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
    }

    IEnumerator Reload()
    {

        if (!isReloading)
        {
            isReloading = true;
            WeaponsAnimator.enabled = true;
            WeaponsAnimator.SetBool("Reloading", true);
            reloadingWaitTimeCircle.gameObject.SetActive(true);
            reloadingWaitTimeCircle.startReloadTime(this);

            yield return new WaitForSeconds(reloadTime - .25f);
            currentAmmo = maxAmmo;
            reloadingCircle.UpdateReloadingCircle(currentAmmo, maxAmmo);
            WeaponsAnimator.SetBool("Reloading", false);
            yield return new WaitForSeconds(.25f);
            WeaponsAnimator.enabled = false;
            isReloading = false;
        }
    }

    public void fastReloading()
    {
        if (reloadingCoroutine!=null)
            StopCoroutine(reloadingCoroutine);
        currentAmmo = maxAmmo;
        reloadingCircle.UpdateReloadingCircle(currentAmmo, maxAmmo);
        WeaponsAnimator.SetBool("Reloading", false);
        WeaponsAnimator.enabled = false;
        isReloading = false;
    }

    void OnEnable()
    {
        isReloading = false;
        WeaponsAnimator = transform.parent.GetComponent<Animator>();
        weaponAnimator = GetComponent<Animator>();
        WeaponsAnimator.SetBool("Reloading", false);
        weaponAnimator.Play("WeaponSwitched");
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

    IEnumerator HasShooted()
    {
        hasShooted = true;
        yield return new WaitForSeconds(1f);
        hasShooted = false;
    }

    private void Shoot()
    {
        StartCoroutine(HasShooted());
        currentAmmo--;
        reloadingCircle.UpdateReloadingCircle(currentAmmo, maxAmmo);
        PlayMuzzleFlash();
        if (controller.isActive())
            StartCoroutine(controller.recoilHapticFeedback(recoilHapticIntensity));

        StartCoroutine(recoilAnimation());
        recoil.RecoilFire();

        if (gameObject.name.Equals("Shotgun"))
            ProcessRaycastShotgun();
        else
            ProcessRaycast();
    }

    private void ProcessRaycastShotgun()
    {
        RaycastHit hit;

        Vector3 hitPoint;

        Vector3 shootPoint = bulletTrailShootPoint.position;

        int numBulletShooted = 5;

        Vector3[] shootDirection = new Vector3[numBulletShooted];

        for (int i = 0; i < shootDirection.Length; i++)
        {
            shootDirection[i] = SpreadBullets();
        }

        int j = 0;
        foreach (Vector3 bullet in shootDirection)
        {
            //trovo il punto di impatto
            if (Physics.Raycast(FirstPersonCamera.transform.position, bullet, out hit, range))
            {
                hitPoint = hit.point;
            }
            else
            {
                //hitpoint se non colpisco nulla
                hitPoint = transform.position + FirstPersonCamera.transform.forward * range / 3;
            }

            //verifico raycast partendo dall'arma in direzione del punto di impatto
            shootDirection[j] = hitPoint - shootPoint;
            if (Physics.Raycast(shootPoint, bullet, out hit, range))
            {
                hitPoint = hit.point;
                //Debug.Log("Colpito: " + hit.transform.name);
                HitImpactEffect(hit);

                damageByDistance(shootPoint, hitPoint);

                ProcessDamage(hit);

                damage = initialDamage;
                
            }

            SpawnBulletTrail(hitPoint);
            j++;
        }

    }

    private void damageByDistance(Vector3 shootPoint, Vector3 hitPoint){
        float distance = Vector3.Distance(shootPoint, hitPoint);
        damage = damage - (distance * 6);
        if (damage <= 5)
            damage = 5;
    }

    IEnumerator recoilAnimation()
    {
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
        else
        {
            //hitpoint se non colpisco nulla
            hitPoint = transform.position + FirstPersonCamera.transform.forward * range / 3;
        }

        //verifico raycast partendo dall'arma in direzione del punto di impatto
        shootDirection = hitPoint - shootPoint;
        if (Physics.Raycast(shootPoint, shootDirection, out hit, range))
        {
            //Debug.Log("Colpito: " + hit.transform.name);
            HitImpactEffect(hit);

            ProcessDamage(hit);

            hitPoint = hit.point;
        }

        SpawnBulletTrail(hitPoint);
    }

    private void ProcessDamage(RaycastHit hit)
    {
        EnemyHealth target = hit.transform.GetComponentInParent<EnemyHealth>();

        if (target)
        {
            target.TakeDamage(damage);
        }
    }


    void spreadAfterShoootTimer()
    {
        if (resetSpreadTimer <= 0)
        {
            surplusSpreadAfterShoot = 0;
        }

        if (resetSpreadTimer > 0)
        {
            resetSpreadTimer -= Time.deltaTime;
        }
    }

    private void IncreaseSpreadAfterShoot()
    {
        if (surplusSpreadAfterShoot <= maxSpreadAfterShoot)
        {
            surplusSpreadAfterShoot = spreadFactor + Random.Range(spreadShoot, spreadShoot * 2);
        }
    }

    private Vector3 SpreadBullets()
    {
        Vector3 shootDirection = FirstPersonCamera.transform.forward;
        float randomSpreadX = Random.Range(-spreadFactor, spreadFactor);
        float randomSpreadY = Random.Range(-spreadFactor, spreadFactor * 2);
        shootDirection = shootDirection + FirstPersonCamera.transform.TransformDirection(new Vector3(randomSpreadX, randomSpreadY));
        return shootDirection;
    }

    private void SpawnBulletTrail(Vector3 hitPoint)
    {

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
