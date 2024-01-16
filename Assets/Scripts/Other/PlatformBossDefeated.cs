using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBossDefeated : MonoBehaviour {

	private Animator animator;

	// Use this for initialization
	void Start () {
      animator = GetComponent<Animator>();	
    }
	
	public void bossDefeated() {
			platformMove();     
	}

	private void platformMove() {
		animator.enabled = true;
	}
}
