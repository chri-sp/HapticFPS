using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {


	[SerializeField] private float rotationSpeed = 10.0f;

	private SceneLoader sceneLoader;

    // Use this for initialization
    void Start () {
        rotationSpeed=Random.Range(rotationSpeed, rotationSpeed * 1.5f);
        sceneLoader = GameObject.FindWithTag("GameEvents").GetComponent<SceneLoader>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.World);
    }

	void OnTriggerEnter() {
		sceneLoader.spawnPoint(transform.position);
		Destroy(gameObject);
	}
}
