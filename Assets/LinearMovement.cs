using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearMovement : MonoBehaviour {

	public float xFrom, xTo, speed;
	private float y, z;

	private void Start() {
		y = transform.localPosition.y;
		z = transform.localPosition.z;
	}
	private void Update () {
		transform.localPosition = new Vector3(transform.localPosition.x + speed*Time.deltaTime, y, z);
		if (transform.localPosition.x > xTo) {
			transform.localPosition = new Vector3(transform.localPosition.x - (xTo-xFrom), y, z);
		}
	}
}
