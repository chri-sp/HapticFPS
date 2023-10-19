using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDash : MonoBehaviour {

    [SerializeField] private HapticProbeFPS controller;
    CharacterController CharacterController;
    public bool dashing = false;

    [Header("Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] public float dashTime = 1.5f;
    [SerializeField] private float TBWDashes = 3.5f;

    float WaitTime;
    private void Start()
    {
        CharacterController = GetComponentInParent<CharacterController>();
        WaitTime = TBWDashes;
    }

    private void Update()
    {
        WaitTime -=  Time.deltaTime;

        //la prima condizione deve essere necessariamente nell'update per evitare che venga perso l'input del falcon
        if ((controller.buttonWasPressed(3) && WaitTime <= 0) || hasDashed())
        {
            StartCoroutine(Dash());
            StartCoroutine(dalyDashingEnable());
        }
    }

    IEnumerator dalyDashingEnable() {
        yield return new WaitForSeconds(dashTime+.2f);
        dashing = false;    
    }

    public bool hasDashed() {      
        return inputDash() && WaitTime <= 0;
    }

    public bool inputDash() {
        if ((Input.GetKey(KeyCode.LeftShift) && (Input.GetKeyDown(KeyCode.Space))))
            {
                return true;
            }
        return false;
    }

    IEnumerator Dash()
    {
        dashing = true;
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
