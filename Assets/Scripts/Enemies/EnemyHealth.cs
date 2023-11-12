﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{

    private Animator animator;
    private Transform aimTarget;
    private float previousHitPoints;
    private SkinnedMeshRenderer[] meshes;
    private bool isDead =false;

    [Header("Effects")]
    [SerializeField] GameObject deathExplosion;

    [SerializeField] float hitPoints = 100f;


    void Start() { 
        animator = GetComponent<Animator>();
        aimTarget= gameObject.transform.Find("AimTarget");
        previousHitPoints = hitPoints;
        meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
    }


    public void TakeDamage(float damage)
    {
        hitPoints -= damage;
        animator.SetTrigger("shot");
        if (hitPoints <= 0)
        {
            StartCoroutine(Death());      
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
            foreach (SkinnedMeshRenderer mesh in meshes)
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
