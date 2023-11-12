using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{

    private Animator animator;
    private Transform aimTarget;
    private float previousHitPoints;
    private Color[] initialColorMeshes;
    private Renderer[] meshes;
    private bool isDead =false;

    [Header("Effects")]
    [SerializeField] GameObject deathExplosion;

    [SerializeField] float hitPoints = 100f;


    void Start() { 
        animator = GetComponent<Animator>();
        aimTarget= gameObject.transform.Find("AimTarget");
        previousHitPoints = hitPoints;

        meshes = GetComponentsInChildren<Renderer>();
        initialColorMeshes = new Color[meshes.Length];
        for (int i = 0; i < initialColorMeshes.Length; i++)
            initialColorMeshes[i] = meshes[i].material.color;
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
            foreach (Renderer mesh in meshes)
                mesh.material.color = Color.red;
            yield return new WaitForSeconds(.1f);
            for (int i = 0; i < meshes.Length; i++)
                meshes[i].material.color = initialColorMeshes[i];
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
            DeathExplosion();
            foreach (Renderer mesh in meshes)
                mesh.enabled = false;
            yield return new WaitForSeconds(explosionDuration);
            Destroy(transform.root.gameObject);
        }
    }

    private void DeathExplosion()
    {
        GameObject explosion = Instantiate(deathExplosion, transform.position, Quaternion.identity);
        explosion.SetActive(true);
    }
}
