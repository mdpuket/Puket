using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeAnimation : MonoBehaviour {

	public float initialY;
	private float startY;
	private Image image;

	public float time, delay;
	public bool loop = true;
	public bool playOnStart = true;

	public bool play;
	private float i,j;

	void Start () {
		startY = transform.localPosition.y;
		image = GetComponent<Image>();
		if(playOnStart) play = true;
		i = j = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(initialY, startY, i), transform.localPosition.z);
		image.color = new Color(image.color.r, image.color.g, image.color.b, i);
		if (play) {
			i += Time.deltaTime/time;
			if (i > 1f) i = 1f;
			if (i == 1f) {
				j += Time.deltaTime;
				if (j >= 1f) {
					if (loop) {
						i = j = 0f;
					} else {
						Destroy(this);
					}
				}
			}
		}
	}
}
