﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class EnemyAttack : MonoBehaviour
{
 
    private HapticProbeFPS controller;
    private PostProcessVolume attackPostprocessing;
    private PlayerHealth playerHealth;

    [Header("Settings")]
    [SerializeField] float damage = 40f;
    [SerializeField] float durationHitEffect= .2f;
    private EnemyAI enemyAI;
    private Animator animator;

    void Start()
    {
        controller = GameObject.FindWithTag("Player").GetComponent<HapticProbeFPS>();
        animator = GetComponent<Animator>();
        enemyAI = GetComponent<EnemyAI>();
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        attackPostprocessing = GetComponentInChildren<PostProcessVolume>();
        attackPostprocessing.weight = 0;
    }

    void Update()
    {
        Attack();
    }



    public void Attack()
    {

        //se abbastanza vicino effettua attacco o si avvicina ulteriormente
        if (enemyAI.isClose())
        {
            animator.SetBool("isAttacking", true);
        }

        else
        {
            animator.SetBool("isAttacking", false);
        }

    }

    //Metodo chiamato come evento durante animazione attacco
    public void hitEvent()
    {
        if (enemyAI.isClose())
        {
            StartCoroutine(attackPostprocessingEffect());
            StartCoroutine(controller.attackHapticFeedback());
            playerHealth.TakeDamage(damage);
            Debug.Log("Danneggiato");
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