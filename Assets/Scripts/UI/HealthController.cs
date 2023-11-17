﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{

    private PlayerHealth playerHealth;
    private Slider healthSlider;
    private Image healthBarColor;
    private Animator animator;
    private bool isDecreasing = false;
    private bool isIncreasing = false;

    [SerializeField] private float smoothDecreaseDuration = .5f;
    [SerializeField] public float smoothIncreaseHealthDuration = .5f;

    [Header("Damage color settings")]
    private Color originalHealthColor;
    [SerializeField] private Color damageHealthColor;


    // Use this for initialization
    void Start()
    {
        playerHealth = GameObject.FindWithTag("Player").GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();
        healthSlider = GetComponent<Slider>();
        healthSlider.value = 1 - playerHealth.fractionRemaining();
        healthBarColor = GetComponentInChildren<Image>();
        originalHealthColor = healthBarColor.color;
    }

    public void healthDecrease()
    {
        StartCoroutine(smoothDecreaseHealth());
    }

    public void healthIncrease()
    {
        StartCoroutine(smoothIncreaseHealth());
    }

    private IEnumerator smoothDecreaseHealth()
    {
        while (isDecreasing)
        {
            yield return null;
        }

        if (!isDecreasing)
        {
            isDecreasing = true;

            animator.Play("HealthDecrease");
            healthBarColor.color = damageHealthColor;
            float healtValue = 1 - playerHealth.fractionRemaining();
            float elapsedTime = 0f;

            while (elapsedTime < smoothDecreaseDuration)
            {
                healthSlider.value = Mathf.Lerp(healthSlider.value, healtValue, elapsedTime / smoothDecreaseDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // verifico che il valore sia esattamente quello finale
            healthSlider.value = healtValue;

            healthBarColor.color = originalHealthColor;
            animator.Play("Disabled");
            isDecreasing = false;
        }
    }

    private IEnumerator smoothIncreaseHealth()
    {
        if (!isIncreasing)
        {
            isIncreasing = true;

            animator.Play("HealthIncrease");
            float healtValue = 1 - playerHealth.fractionRemaining();
            float elapsedTime = 0f;

            while (elapsedTime < smoothIncreaseHealthDuration)
            {
                healthSlider.value = Mathf.Lerp(healthSlider.value, healtValue, elapsedTime / smoothIncreaseHealthDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // verifico che il valore sia esattamente quello finale
            healthSlider.value = healtValue;

            animator.Play("Disabled");
            isIncreasing = false;
        }

    }
}