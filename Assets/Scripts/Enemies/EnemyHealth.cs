using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{

    private Animator animator;
    private TurretShoot turretShoot;
    private Transform aimTarget;
    private FloatingHealtBar healtBar;
    private float previousHitPoints;
    private List<Color> changeColorHit=new List<Color>();
    private Renderer[] meshes;
    private bool isDead =false;
    private AudioController enemySound;
    private bool isHit = false;

    [Header("Effects")]
    [SerializeField] GameObject deathExplosion;

    [SerializeField] float hitPoints = 100f;
    private float initialHitPoints;

    void Awake() {
        healtBar = gameObject.GetComponentInChildren<FloatingHealtBar>();
    }
    void Start() { 
        initialHitPoints = hitPoints;
        animator = GetComponent<Animator>();
        turretShoot = GetComponent<TurretShoot>();

        aimTarget = gameObject.transform.Find("AimTarget");
        previousHitPoints = hitPoints;
        healtBar.UpdateHealthBar(fractionRemaining());
        meshes = GetComponentsInChildren<Renderer>();

        foreach (Renderer mesh in meshes)
            foreach (Material mat in mesh.materials)
                changeColorHit.Add(mat.color);

        enemySound = GetComponent<AudioController>();
    }

    public float fractionRemaining() {
        if (hitPoints <= 0)
        {
            return 0;
        }
        return hitPoints / initialHitPoints;
    }
    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        healtBar.UpdateHealthBar(fractionRemaining());
        StartCoroutine(hitEffects());
        if (hitPoints <= 0)
        {
            StartCoroutine(Death());      
        }
    }

    IEnumerator hitEffects()
    {
        animator.SetTrigger("shot");

        //effetto cambio colore
        if (!isDead) {
            Color HitColor = new Color(1f, 0f, 0.1f, 1f);
            foreach (Renderer mesh in meshes)
                foreach (Material mat in mesh.materials)
                    mat.color = HitColor;
            yield return new WaitForSeconds(.1f);

            int i = 0;
            foreach (Renderer mesh in meshes)
                foreach (Material mat in mesh.materials) {
                    mat.color = changeColorHit[i];
                    i++;
                }
                    
        }     
    }
    public bool getHit() {
        if (hitPoints< previousHitPoints) {
            previousHitPoints = hitPoints;
            StartCoroutine(randomSoundHit());
            
            return true;
        }
        return false;
    }

    private IEnumerator randomSoundHit() {
        if (isHit) yield break;
        isHit = true;
        int random = UnityEngine.Random.Range(1, 4);
        yield return new WaitForSeconds(.2f);
        enemySound.Play("enemyHit" + random);
        yield return new WaitForSeconds(.2f);
        isHit = false;
    }

    IEnumerator Death() {
        float explosionDuration = 3f;
        if (!isDead) {
            isDead = true;
            enemySound.Play("enemyDeath");

            //Disabilito torretta se presente
            if (turretShoot!=null)
                turretShoot.enabled = false;

            aimTarget.gameObject.SetActive(false);
            animator.SetBool("death", true);
            yield return new WaitForSeconds(1);
            DeathExplosion(explosionDuration);
            healtBar.gameObject.SetActive(false);
            foreach (Renderer mesh in meshes)
                mesh.enabled = false;
            yield return new WaitForSeconds(explosionDuration);
            Destroy(gameObject);
        }
    }

    void DeathExplosion(float explosionDuration)
    {
        GameObject explosion = Instantiate(deathExplosion, transform.position, Quaternion.identity);
        explosion.SetActive(true);
        Destroy(explosion, explosionDuration);
    }
}
