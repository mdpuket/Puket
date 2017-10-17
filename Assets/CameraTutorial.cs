using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraTutorial : MonoBehaviour {

	public float shotDelay, unshotDelay;
	private float i,j;
	public Sprite unshot, shot;
	// Use this for initialization
	void Start () {
		i = j = 0f;
	}

	// Update is called once per frame
	void Update () {
		if (i < shotDelay) {
			i += Time.deltaTime;
			if(i >= shotDelay) {
				GetComponent<Image>().sprite = shot;
			}
		} else {
			j += Time.deltaTime;
			if (j >= unshotDelay) {
				i = j = 0f;
				GetComponent<Image>().sprite = unshot;	
			}
		}
	}
}
