using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class WeaponBobbing : MonoBehaviour {

    private FirstPersonControllerFalcon FPSControllerFalcon;

    [Header("Bobbing")]
    public float speedCurve;
    float curveSin { get { return Mathf.Sin(speedCurve); } }
    float curveCos { get { return Mathf.Cos(speedCurve); } }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    Vector3 bobPosition;

    public float bobExaggeration;
    private float initialBobExaggeration;
    public float changeMovementSpeed = 5f;

    private Vector3 initialWeaponPosition;

    Vector2 walkInput;

    void Start()
    {
        initialBobExaggeration = bobExaggeration;
        FPSControllerFalcon = FindObjectOfType<FirstPersonControllerFalcon>();
        initialWeaponPosition = transform.localPosition;
    }

    void Update()
    {
        GetInput();

        BobOffset();

        BobRunning();

        transform.localPosition = Vector3.Lerp(transform.localPosition, initialWeaponPosition + bobPosition, Time.deltaTime * changeMovementSpeed);
    }

    void BobRunning() {

        if (FPSControllerFalcon.m_IsWalking)
        {
            bobExaggeration = initialBobExaggeration;
           
        }
        else
        {
            bobExaggeration = initialBobExaggeration * 3;
        }
    }


    void GetInput()
    {
        walkInput.x = Input.GetAxis("Horizontal");
        walkInput.y = Input.GetAxis("Vertical");
        walkInput = walkInput.normalized;
    }

    void BobOffset()
    {
        speedCurve += Time.deltaTime * (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical") * bobExaggeration) + 0.01f;

        bobPosition.x = (curveCos * bobLimit.x  - (walkInput.x * travelLimit.x));
        bobPosition.y = (curveSin * bobLimit.y) - (Input.GetAxis("Vertical") * travelLimit.y);
        bobPosition.z = -(walkInput.y * travelLimit.z);
    }
}
