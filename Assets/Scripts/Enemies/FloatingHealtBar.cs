using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealtBar : MonoBehaviour
{

    [SerializeField] private Slider slider;
    private Camera camera;
    private Transform target;
    [SerializeField] private Vector3 offset;

    void Start()
    {
        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        target = transform.root.GetComponent<Transform>();
    }
    public void UpdateHealthBar(float percentEnemyHealt)
    {
        slider.value = percentEnemyHealt;

        //Disabilita slider se nemico ha 0 di salute
        if (slider.value <= 0) {
           transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera.transform.rotation;
        transform.position = target.position + offset;
    }
}
