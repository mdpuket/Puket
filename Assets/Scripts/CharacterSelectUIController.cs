using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;

public class CharacterSelectUIController : MonoBehaviour {

	public enum Character { RAPOSA=0, UNICORNIO=1, COALA=2, PANDA=3, DALMATA=4, CORUJA=5, DRAGAO=6 };
	public static Character selectedCharacter;

	private ComponentTracker tracker;

	private void Start () {
		tracker = ComponentTracker.Instance;
		tracker.GetObject("loading_text").SetActive(false);
    }

	public void CharacterSelect(int i) {
		Debug.Log((Character)i);
		selectedCharacter = (Character)i;
		SceneManager.LoadScene("ar");
		tracker.GetObject("loading_text").SetActive(true);
	}

	public void Back() {
		SceneManager.LoadScene("start");
		tracker.GetObject("loading_text").SetActive(true);
	}
}
