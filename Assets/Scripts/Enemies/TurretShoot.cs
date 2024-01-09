using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TurretShoot : MonoBehaviour
{
    [Header("General")]
    public Transform shootPoint; 
    public Transform gunPoint;  
    public float bulletDistance;
    private PlayerHealth playerHealth;

    [Header("Gun")]
    public Vector3 spread = new Vector3(0.06f, 0.06f, 0.06f);
    public TrailRenderer bulletTrail;

    private HapticProbeFPS controller;
    private PostProcessVolume attackPostprocessing;

    [Header("Settings")]
    [SerializeField] float damage = 40f;
    [SerializeField] float durationHitEffect= .2f;
    private TurretAIShooting enemyAI;
    private Animator animator;
    private Transform player;
    public float timeBetweenShoot = .5f;
    private float initialTimeBetweenShoot;

    private AudioController enemySound;

    void Start()
    {
        controller = GameObject.FindWithTag("Player").GetComponent<HapticProbeFPS>();
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<TurretAIShooting>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        attackPostprocessing = GetComponentInChildren<PostProcessVolume>();
        attackPostprocessing.weight = 0;
        initialTimeBetweenShoot = timeBetweenShoot;
        enemySound = GetComponent<AudioController>();
    }

    void Update()
    {
        shootTimer();
        Attack();
    }

    private void shootTimer() {
        if (timeBetweenShoot > 0)
        {
            timeBetweenShoot -= Time.deltaTime;
        }
    }


    //Metodo chiamato dall'animazione di sparo del nemico
    public void Shoot()
    {
        RaycastHit hit;

        Vector3 direction = GetDirection();

        if (Physics.Raycast(shootPoint.position, direction, out hit, bulletDistance))
        {
            UnityEngine.Debug.DrawLine(shootPoint.position, shootPoint.position + direction * 10f, Color.red, 1f);
            TrailRenderer trail = Instantiate(bulletTrail, gunPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));
            randomSoundShoot();
        }
        else {
            TrailRenderer trail = Instantiate(bulletTrail, gunPoint.position, Quaternion.identity);
            hit.point = shootPoint.position + shootPoint.forward + direction * bulletDistance;
            StartCoroutine(SpawnTrail(trail, hit));
        }
        timeBetweenShoot = initialTimeBetweenShoot;
    }

    private void randomSoundShoot()
    {
        int random = UnityEngine.Random.Range(1, 5);
        enemySound.Play("enemyShoot" + random);
    }
    private Vector3 GetDirection()
    {
        Vector3 playerPosition = player.position;
        playerPosition.y = playerPosition.y - 1f;
        Vector3 direction = playerPosition - transform.position;
        direction += new Vector3(
        Random.Range(-spread.x, spread.x),
        Random.Range(-spread.y, spread.y),
        Random.Range(-spread.z, spread.z));
        direction.Normalize();
        return direction;
    }


    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0f;
        Vector3 startPosition= trail.transform.position;

        while (time < 1f) {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;
            yield return null;
        }
        
        trail.transform.position = hit.point;

        checkBulletCollsion(hit);

        Destroy(trail.gameObject, trail.time);
    }

    private void checkBulletCollsion(RaycastHit hit) {

        //verifico raycast partendo dall'arma in direzione del player
        Vector3 shootDirection = hit.point - shootPoint.position;
        if (Physics.Raycast(shootPoint.position, shootDirection, out hit, bulletDistance))
        {
            UnityEngine.Debug.DrawLine(shootPoint.position, shootPoint.position + shootDirection * bulletDistance, Color.red, 1f);
            //se ho colpito il player
            if (hit.collider.tag == "Player")
            {
                StartCoroutine(attackPostprocessingEffect());
                StartCoroutine(controller.attackHapticFeedback());
                playerHealth.TakeDamage(damage);
                //UnityEngine.Debug.Log("Danneggiato");
            }
        }
    }
    public void Attack()
    {
        if (enemyAI.isViewing() && timeBetweenShoot <= 0)
        {
            animator.SetBool("isAttacking", true);
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }
    }

    private IEnumerator attackPostprocessingEffect()
    {
        float t = 0f;
        float attesaDisattivazioneEffetto = .1f;

        //Attivazione post processing
        while (t < 1)
        {
            t += Time.deltaTime / durationHitEffect;

            if (t > 1) t = 1;

            attackPostprocessing.weight = Mathf.Lerp(0, 1, Mathf.SmoothStep(0, 1, t));

            yield return null;
        }

        yield return new WaitForSeconds(attesaDisattivazioneEffetto);

        //Disattivazione post processing
        t = 0f;

        while (t < 1)
        {
            t += Time.deltaTime / durationHitEffect;

            if (t > 1) t = 1;

            attackPostprocessing.weight = Mathf.Lerp(1, 0, Mathf.SmoothStep(0, 1, t));

            yield return null;
        }
    }

}