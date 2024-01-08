using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {


	[SerializeField] private float rotationSpeed = 10.0f;

	private SceneLoader sceneLoader;

	private AudioSource audioSource;

	private float avoidSoundTimer;
    // Use this for initialization
    void Start () {
        rotationSpeed=Random.Range(rotationSpeed, rotationSpeed * 1.5f);
        sceneLoader = GameObject.FindWithTag("GameEvents").GetComponent<SceneLoader>();
		audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.World);
		avoidSoundTimer += Time.deltaTime;
    }

	void OnTriggerEnter() {
		StartCoroutine(OnTrigger());
	}

	private IEnumerator OnTrigger(){
        sceneLoader.setSpawnPoint(transform.position);
		GetComponent<MeshRenderer>().enabled = false;	
		if (avoidSoundTimer >= 1)
			audioSource.Play();
		yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }
}
