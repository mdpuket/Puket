using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDino : MonoBehaviour {

	public Animal animal;
	public ParticleSystem fire;
	public float fireDelay, fireDuration;

	private IEnumerator FireAnimation() {
		yield return new WaitForSeconds(fireDelay);
		fire.Play();
		yield return new WaitForSeconds(fireDuration);
		fire.Stop();
	}

	private void OnGUI () {
		for (int i = 0; i < animal.actions.Length; i++) {
			if(GUI.Button(new Rect(Screen.width - 150f, 50f + 60f*i, 100f, 50f), animal.actions[i].actionName)) {
				animal.TriggerAnimation(animal.actions[i].actionId);
			}
		}
	}
}
