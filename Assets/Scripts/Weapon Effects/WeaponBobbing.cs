using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBobbing : MonoBehaviour {

    [Header("Bobbing")]
    public float speedCurve;
    float curveSin { get { return Mathf.Sin(speedCurve); } }
    float curveCos { get { return Mathf.Cos(speedCurve); } }

    public Vector3 travelLimit = Vector3.one * 0.025f;
    public Vector3 bobLimit = Vector3.one * 0.01f;
    Vector3 bobPosition;

    public float bobExaggeration;
    public float changeMovementSpeed = 5f;


    private Vector3 initialWeaponPosition;

    void Start()
    {
        initialWeaponPosition = transform.localPosition;
    }

    void Update()
    {
        GetInput();

        BobOffset();

        transform.localPosition = Vector3.Lerp(transform.localPosition, initialWeaponPosition + bobPosition, Time.deltaTime * changeMovementSpeed);
    }


    Vector2 walkInput;

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
