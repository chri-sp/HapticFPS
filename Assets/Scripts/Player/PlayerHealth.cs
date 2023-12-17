using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] float health = 100f;
    private float initialHealth;
    private HealthController healthController;
    [SerializeField] public float resetHealthDelay = 3f;
    private float resetHealthTimer;

    private AudioManager audioManager;
    void Start()
    {
        initialHealth = health;
        healthController = GameObject.FindWithTag("Canvas").GetComponentInChildren<HealthController>();       
        resetHealthTimer = resetHealthDelay;
        audioManager = GameObject.FindWithTag("AudioSystem").GetComponent<AudioManager>();
    }

    void Update()
    {
        IncreaseHealthTimer();
        resetHealth();
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthController.healthDecrease();
        resetHealthTimer = resetHealthDelay;
        if (health <= 0)
        {
            healthController.death();
            //Debug.Log("Game Over");
        }
    }

    public float fractionRemaining()
    {
        if (health <= 0)
        {
            soundHealthFinished();
            return 0;
        }
        return health / initialHealth;
    }

    void soundHealthFinished()
    {
        if (!audioManager.IsPlaying("HealthFinished"))
            audioManager.Play("HealthFinished");
    }

    public float currentHealth()
    {
        return health;
    }

    void IncreaseHealthTimer()
    {

        if (resetHealthTimer > 0)
        {
            resetHealthTimer -= Time.deltaTime;
        }
    }

    void resetHealth()
    {
        if (resetHealthTimer <= 0 && health < initialHealth)
        {
            health = initialHealth;
            healthController.healthIncrease();
            audioManager.Play("HealthReset");
            resetHealthTimer = resetHealthDelay;
        }
    }
}
