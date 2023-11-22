using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    private Transform startPoint;
    private Transform endPoint;
    public float travelTime;
    private Rigidbody rb;
    private Vector3 currentPos;

    CharacterController cc;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPoint = GetComponent<Transform>();
        endPoint = GetComponent<Transform>();
    }
    void FixedUpdate()
    {
        currentPos = Vector3.Lerp(startPoint.position, endPoint.position,
            Mathf.Cos(Time.time / travelTime * Mathf.PI * 2) * -.5f + .5f);
        rb.MovePosition(currentPos);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            cc = other.GetComponent<CharacterController>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
            cc.Move(rb.velocity * Time.deltaTime);

    }

}