using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSkybox : MonoBehaviour {

    [SerializeField] private float rotationSpeed = 1.0f;
    float curRot = 0;

    void Start ()
    {
        RenderSettings.skybox.SetFloat("_Rotation", 0);
    }

    // Update is called once per frame
    void Update()
    {
        curRot += rotationSpeed * Time.deltaTime;
        curRot %= 360;
        RenderSettings.skybox.SetFloat("_Rotation", curRot);
    }
}
