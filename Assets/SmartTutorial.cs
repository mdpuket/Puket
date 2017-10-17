using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartTutorial : MonoBehaviour {

	public Vector3[] positions;
	public float startDelay = 0f;
	public float movementTime = 1f;
	public float flickDelay = 1f;
	public bool test = false;

	private IEnumerator coroutine;
	private Vector3 startPosition, startEulerAngles;

	private void Start() {
		coroutine = Animation();
		startPosition = transform.localPosition;
		startEulerAngles = transform.localEulerAngles;
	}

	private void Update() {
		if(test) {
			test = false;
			Play();
		}
	}

	public void Play() {
		StopCoroutine(coroutine);

		iTween[] itweens = GetComponents<iTween>();
		foreach (iTween itween in itweens) Destroy(itween);
		transform.localPosition = startPosition;
		transform.localEulerAngles = startEulerAngles;

		coroutine = Animation();
		StartCoroutine(coroutine);
	}

	private IEnumerator Animation() {
		yield return new WaitForSeconds(startDelay);
		for (int i = 0; i < positions.Length; i++) {
			iTween.MoveTo(gameObject, iTween.Hash(
				"position", positions[i],
				"islocal", true,
				"easetype", "easeInOutExpo",
				"time", movementTime
			));
			yield return new WaitForSeconds(movementTime);
		}

		while (true) {
			yield return new WaitForSeconds(flickDelay);
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
}
