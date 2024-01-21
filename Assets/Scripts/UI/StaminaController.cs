using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaController : MonoBehaviour
{


    private PlayerStamina playerStamina;
    private Slider staminaSlider;
    private Animator animator;
    private bool isIncreasing = false;
    private bool isConsumed = false;

    [SerializeField] private RawImage Icon;
    [SerializeField] private RawImage endStaminaIcon;
    [SerializeField] public float smoothIncreaseStaminaDuration = .5f;

    // Use this for initialization
    void Start()
    {
        playerStamina = GameObject.FindWithTag("Player").GetComponent<PlayerStamina>();
        animator = GetComponent<Animator>();
        staminaSlider = GetComponent<Slider>();
        staminaSlider.value = 1 - playerStamina.fractionRemaining();
    }


    public void staminaDecrease()
    {
        staminaSlider.value = 1 - playerStamina.fractionRemaining();
        if (staminaSlider.value >= 1f)
        {
            Icon.enabled = false;
            endStaminaIcon.enabled = true;
            StartCoroutine(staminaConsumedAnimation());
        }
    }

    IEnumerator staminaConsumedAnimation()
    {
        if (!isConsumed)
        {
            isConsumed = true;
            animator.Play("StaminaConsumed");
            yield return new WaitForSeconds(.2f);
            animator.Play("Disabled");
            isConsumed = false;
        }
    }

    public void staminaIncrease()
    {
        StartCoroutine(smoothIncreaseStamina());
    }


    private IEnumerator smoothIncreaseStamina()
    {
        if (!isIncreasing)
        {
            isIncreasing = true;
            endStaminaIcon.enabled = false;
            Icon.enabled = true;

            animator.Play("StaminaIncrease");
            float staminaValue = 1 - playerStamina.fractionRemaining();
            float elapsedTime = 0f;

            while (elapsedTime < smoothIncreaseStaminaDuration)
            {
                staminaSlider.value = Mathf.Lerp(staminaSlider.value, staminaValue, elapsedTime / smoothIncreaseStaminaDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // verifico che il valore sia esattamente quello finale
            staminaSlider.value = staminaValue;
            animator.Play("Disabled");
            isIncreasing = false;
        }

    }

}