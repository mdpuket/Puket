using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FakeLoadingBar : MonoBehaviour {

	public Vector2 increment;
	public Vector2 delay;
	public float lerpSpeed;
	private float progress;

	public Image bar;
	private Vector2 sizeDelta;

	public bool startAnimation = false;

	private void Start() {
		sizeDelta = bar.rectTransform.sizeDelta;
	}

	private void Update() {
		bar.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(bar.rectTransform.sizeDelta.x, sizeDelta.x*progress, lerpSpeed*Time.deltaTime), sizeDelta.y);
		if (startAnimation) {
			startAnimation = false;
			StartCoroutine(BarAnimation(""));
		}
	}

	public void Run(string scene) {
		IEnumerator coroutine = BarAnimation(scene);
		StartCoroutine(coroutine);
	}

	private IEnumerator BarAnimation(string scene) {
		progress = 0f;
		bar.rectTransform.sizeDelta = new Vector2(0f, sizeDelta.y);
		while (progress < 1f) {
			progress += Random.Range(increment.x, increment.y);
			if (progress > 1f) progress = 1f;
			yield return new WaitForSeconds(Random.Range(delay.x, delay.y));
		}
		yield return new WaitForSeconds(1f);
		SceneManager.LoadScene(scene);
	}
}
