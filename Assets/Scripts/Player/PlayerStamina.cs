using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{

    private StaminaController staminaController;
    private CharacterController characterController;

    [SerializeField] float stamina = 100f;
    private float maxStamina;
    [SerializeField] public float runStaminaConsumed = .1f;
    [SerializeField] public float dashStaminaConsumed = 10f;
    [SerializeField] public float resetStaminaDelay = 1f;
    private float initialResetStaminaDelay;
    [SerializeField] public float resetStaminaEndedDelay = 3f;
    private float resetStaminaTimer;


    void Start()
    {
        maxStamina = stamina;
        initialResetStaminaDelay = resetStaminaDelay;
        staminaController = GameObject.FindWithTag("Canvas").GetComponentInChildren<StaminaController>();
        characterController = GetComponent<CharacterController>();
        resetStaminaTimer = resetStaminaDelay;
    }

    void Update()
    {
        IncreaseStaminaTimer();
        resetStamina();
        changeDelayIfStaminaIsConsumed();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift) && characterController.velocity.sqrMagnitude > 0)
        {
            resetStaminaTimer = resetStaminaDelay;
            decreaseStamina(runStaminaConsumed);
        }
    }


    public void hasDashed() {
        resetStaminaTimer = resetStaminaDelay;
        decreaseStamina(dashStaminaConsumed);
    }


    private void decreaseStamina(float value)
    {
        if (stamina > 0)
            stamina = stamina - value;
        else
            stamina = 0f;

        staminaController.staminaDecrease();
    }

    private void changeDelayIfStaminaIsConsumed()
    {
        //Cambio tempo di attesa ricarica stamina se totalmente consumata
        if (stamina <= 0)
        {
            resetStaminaDelay = resetStaminaEndedDelay;
        }
        else
        {
            resetStaminaDelay = initialResetStaminaDelay;
        }
    }

    public float fractionRemaining()
    {
        if (stamina <= 0)
        {
            return 0;
        }
        return stamina / maxStamina;
    }


    void IncreaseStaminaTimer()
    {
        if (resetStaminaTimer > 0)
        {
            resetStaminaTimer -= Time.deltaTime;
        }
    }

    void resetStamina()
    {
        if (resetStaminaTimer <= 0 && stamina < maxStamina)
        {
            stamina = maxStamina;
            staminaController.staminaIncrease();
            resetStaminaTimer = resetStaminaDelay;
        }
    }


    public float currentStamina()
    {
        return stamina;
    }
}
