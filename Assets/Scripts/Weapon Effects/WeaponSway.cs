using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] private FirstPersonControllerFalcon FPSControllerFalcon;
    private MouseLookFalcon falconLook;
    [SerializeField] HapticProbeFPS controller;

    [Header("Sway Settings")]
    [SerializeField] private float smooth=8;
    [SerializeField] private float swayMultiplier=3;
    [SerializeField] private float swayMultiplierFalcon = 5;


    // Start is called before the first frame update
    void Start()
    {
        falconLook = FPSControllerFalcon.m_MouseLook;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX;
        float mouseY;

        if ((controller.isActive())) {
            mouseX = falconLook.getAxisXFalcon() * swayMultiplierFalcon;
            mouseY = falconLook.getAxisYFalcon()* swayMultiplierFalcon;
        }
        else
        {
            mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
            mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;
        }

        Debug.Log("mouse x: "+mouseX+"\n mouse y: "+mouseY);

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
