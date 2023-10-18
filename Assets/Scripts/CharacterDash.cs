using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDash : MonoBehaviour {

    CharacterController CharacterController;
    public bool dashing =false;

    [Header("Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] public float dashTime = 1.5f;
    [SerializeField] private float TBWDashes = 3.5f;

    [Header("Time between double click ")]
    [SerializeField] private float timeBetweenClick=.3f;

    private float lastClickTime;


    float WaitTime;
    private void Start()
    {
        CharacterController = GetComponentInParent<CharacterController>();
        WaitTime = TBWDashes;
    }

    private void Update()
    {
        WaitTime -= Time.time;

        if (hasDashed())
        {
            StartCoroutine(Dash());
        }
    }

    public bool hasDashed() { 
        return doubleClickShift() && WaitTime <= 0;
    }

    private bool doubleClickShift() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) { 
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= timeBetweenClick) { 
                return true;
            }

            lastClickTime = Time.time;
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

        dashing = false;
    }
}
