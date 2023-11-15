using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{

    private Animator animator;
    private Transform aimTarget;
    private float previousHitPoints;
    private List<Color> changeColorHit=new List<Color>();
    private Renderer[] meshes;
    private bool isDead =false;

    [Header("Effects")]
    [SerializeField] GameObject deathExplosion;

    [SerializeField] float hitPoints = 100f;
    private float initialHitPoints;


    void Start() { 
        initialHitPoints = hitPoints;
        animator = GetComponent<Animator>();
        aimTarget= gameObject.transform.Find("AimTarget");
        previousHitPoints = hitPoints;

        meshes = GetComponentsInChildren<Renderer>();

        foreach (Renderer mesh in meshes)
            foreach (Material mat in mesh.materials)
                changeColorHit.Add(mat.color);
    }

    public float fractionRemaining() {
        return hitPoints/ initialHitPoints;
    }
    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
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
            return true;
        }
        return false;
    }

    IEnumerator Death() {
        float explosionDuration = 3f;
        if (!isDead) {
            isDead = true;
            aimTarget.gameObject.SetActive(false);
            animator.SetBool("death", true);
            yield return new WaitForSeconds(1);
            DeathExplosion(explosionDuration);
            foreach (Renderer mesh in meshes)
                mesh.enabled = false;
            yield return new WaitForSeconds(explosionDuration);
            Destroy(transform.root.gameObject);
        }
    }

    void DeathExplosion(float explosionDuration)
    {
        GameObject explosion = Instantiate(deathExplosion, transform.position, Quaternion.identity);
        explosion.SetActive(true);
        Destroy(explosion, explosionDuration);
    }
}
