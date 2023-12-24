using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CharacterDash : MonoBehaviour {

    private PlayerStamina playerStamina;
    private AudioManager audioManager;

    [SerializeField] private HapticProbeFPS controller;
    [SerializeField] private PostProcessVolume dashPostprocessing;
    private LensDistortion ls;
    private TrailRenderer trailRenderer;
    CharacterController CharacterController;
    public bool dashing = false;
    //Variabile usata per assicurasi che non vengo perso l'input di dash 
    public bool falconDashed = false;

    [Header("Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] public float dashTime = 1.5f;
    [SerializeField] private float TBWDashes = 3.5f;
    [SerializeField] private float dashEffectDuration = .6f;


    float WaitTime;
    private void Start()
    {
        CharacterController = GetComponentInParent<CharacterController>();
        playerStamina = GameObject.FindWithTag("Player").GetComponent<PlayerStamina>();
        WaitTime = TBWDashes;
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.enabled = false;
        dashPostprocessing.weight = 0;
        dashPostprocessing.profile.TryGetSettings(out ls);
        audioManager = GameObject.FindWithTag("AudioSystem").GetComponent<AudioManager>();
    }

    private void Update()
    {
        WaitTime -= Time.deltaTime;

        if (falconDashed || hasDashed())
        {
            falconDashed = false;
            playerStamina.hasDashed();
            StartCoroutine(Dash());
            StartCoroutine(DashPostProccessingEffect());
            StartCoroutine(DashTrailEffect());        
            StartCoroutine(delayDashingEnable());
        }
    }

    IEnumerator delayDashingEnable()
    {
        yield return new WaitForSeconds(dashTime + .2f);
        dashing = false;
    }

    public bool falconHasDashed()
    {
        
        if (controller.buttonWasPressed(3) && WaitTime <= 0 && CharacterController.velocity.sqrMagnitude > 0 && playerStamina.currentStamina()>0) {
            falconDashed = true;
            return true;
        }

        return false;
    }

    public bool hasDashed()
    {

        return inputDash() && WaitTime <= 0 && CharacterController.velocity.sqrMagnitude > 0 && playerStamina.currentStamina() > 0;
    }

    public bool inputDash() {
        if ((Input.GetKey(KeyCode.LeftShift) && (Input.GetKeyDown(KeyCode.Space))))
            {
                return true;
            }
        return false;
    }

    
    IEnumerator DashTrailEffect() {
        trailRenderer.enabled = true;
        yield return new WaitForSeconds(dashEffectDuration);   
        trailRenderer.enabled = false;
    }

    private IEnumerator DashPostProccessingEffect() {
        float t = 0f;
        float effectDuration = dashTime + .1f;
        float attesaDisattivazioneEffetto = .1f;

        setValueDashEffect();

        //Attivazione post processing
        while (t < 1)
        {
            t += Time.deltaTime / effectDuration;

            if (t > 1) t = 1;

            dashPostprocessing.weight = Mathf.Lerp(0, 1, Mathf.SmoothStep(0, 1, t));

            yield return null;
        }

        yield return new WaitForSeconds(attesaDisattivazioneEffetto);

        //Disattivazione post processing
        t = 0f;
        effectDuration = dashTime + .1f;

        while (t < 1)
        {
            t += Time.deltaTime / effectDuration;

            if (t > 1) t = 1;

            dashPostprocessing.weight = Mathf.Lerp(1, 0, Mathf.SmoothStep(0, 1, t));

            yield return null;
        }
    }

    private void setValueDashEffect() {
        float intensity = 20;
        float intensityX = 0.5f;
        if (Input.GetAxis("Vertical") > 0)
        {
            ls.intensityX.value = intensityX;
            ls.intensity.value = -intensity;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            ls.intensityX.value = intensityX;
            ls.intensity.value = intensity;
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            ls.intensity.value = intensity;
            ls.intensityX.value = 1;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            ls.intensity.value = intensity;
            ls.intensityX.value = 0;
        }
    }

    IEnumerator Dash()
    {  
        dashing = true;
        audioManager.Play("Dash");
        float startTime = Time.time;

        while (Time.time < startTime + dashTime)
        {

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 moveDir = transform.right * x + transform.forward * z;
            CharacterController.Move(moveDir * dashSpeed * Time.deltaTime);

            WaitTime = TBWDashes;

            yield return null;
        }
    }
}
