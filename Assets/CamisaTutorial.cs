using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamisaTutorial : MonoBehaviour {

	public float delay;
	private float time;

	private void Start() {
		time = Time.time + delay;
	}

	private void Update() {
		if (Time.time >= time) {
			StartCoroutine(Animation());
			time += delay;
		}
	}

	private IEnumerator Animation() {
		float z = transform.localEulerAngles.z;
		iTween.RotateTo(gameObject, iTween.Hash(
			"z", -13.972f,
			"islocal", true,
			"time", 0.15f,
			"easeType", "linear"
		));
		yield return new WaitForSeconds(0.16f);
		// if(GetComponent<iTween>()) Destroy(GetComponent<iTween>());
		iTween.RotateTo(gameObject, iTween.Hash(
			"z", z,
			"islocal", true,
			"time", 0.15f,
			"easeType", "linear"
		));
	}
}
