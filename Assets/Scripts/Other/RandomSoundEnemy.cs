using System.Collections;
using UnityEngine;

public class RandomSoundEnemy : MonoBehaviour {

	private AudioController audioController;

	private float timer;

	private float initialTimer;

	private bool playingSound = false;
	// Use this for initialization
	void Start () {
		audioController = GetComponent<AudioController>();
		initialTimer=Random.Range(3,6);
		timer = initialTimer;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;

		if (timer <= 0)
		{
            StartCoroutine(randomSound());
			timer = initialTimer;
		}
	}

    private IEnumerator randomSound()
    {
        if (playingSound) yield break;
        playingSound = true;
        int random = Random.Range(1, 4);
        yield return new WaitForSeconds(.2f);
        audioController.PlayOverlappingSound("enemyHit"+random);
        yield return new WaitForSeconds(.2f);
        playingSound = false;
    }
}
