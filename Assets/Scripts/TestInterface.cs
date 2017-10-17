using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInterface : MonoBehaviour {

	public Animal raposa;

	private void OnGUI () {
		if(GUI.Button(new Rect(Screen.width - 150f, 50f, 100f, 50f), "Dançar")) {
			raposa.TriggerAnimation("dance");
		}

		if(GUI.Button(new Rect(Screen.width - 150f, 120f, 100f, 50f), "Pular")) {
			raposa.TriggerAnimation("jump");
		}

		if(GUI.Button(new Rect(Screen.width - 150f, 190f, 100f, 50f), "Sorrir")) {
			raposa.TriggerAnimation("smile");
		}

		if(GUI.Button(new Rect(Screen.width - 150f, 260f, 100f, 50f), (raposa.gameObject.GetComponent<Animator>().GetBool("sleeping"))? "Acordar": "Dormir")) {
			raposa.gameObject.GetComponent<Animator>().SetBool("sleeping", !raposa.gameObject.GetComponent<Animator>().GetBool("sleeping"));
		}
	}
}
