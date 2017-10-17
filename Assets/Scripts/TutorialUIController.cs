using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class TutorialUIController : MonoBehaviour {

	public Color[] stepBackgroundColors;
	public GameObject[] stepPanels;
	public GameObject[] stepMarkers;

	public Vector3 bigStepScale = Vector3.one;
	public Vector3 smallStepScale = Vector3.one*0.4988048f;
	public float panelDistance = 1000f;
	public float speed = 10f;

	public int nSteps = 4;
	public int currentStep = 0;
	private ComponentTracker tracker;
	
	public static string tutorialReadedFileName {
        get {
            return string.Format("{0}/{1}.txt", Application.persistentDataPath, "tutorialReaded");
        }
    }

	private void Start() {
		tracker = ComponentTracker.Instance;

		if(!File.Exists(tutorialReadedFileName)) File.WriteAllLines(tutorialReadedFileName, new string[0]);

		currentStep = 0;
		SetAnimation();
		tracker.GetElement<Image>("background_image").color = stepBackgroundColors[currentStep];
		stepMarkers[currentStep].transform.localScale = bigStepScale;
		for (int i = 1; i < nSteps; i++) {
			stepPanels[i].transform.localPosition = new Vector3(panelDistance*i, 0f, 0f);
			stepMarkers[i].transform.localScale = smallStepScale;
		}
		tracker.GetObject("loading_text").SetActive(false);
		// tracker.GetObject("previous_button").SetActive(false);
		if (AudioListener.volume == 1f) tracker.GetObject("soundoff_image").SetActive(false);
		else tracker.GetObject("soundon_image").SetActive(false);
	}

	private void Update() {
		Transform stepsPanel = tracker.GetElement<Transform>("steps_panel");
		Image background = tracker.GetElement<Image>("background_image");

		stepsPanel.localPosition = Vector3.Lerp(stepsPanel.transform.localPosition, new Vector3(-1*currentStep*panelDistance, 0f, 0f), Time.deltaTime*speed);
		background.color = Color.Lerp(background.color, stepBackgroundColors[currentStep], Time.deltaTime*speed);
		tracker.GetElement<Image>("step3_mask").color = background.color;
		for (int i = 0; i < nSteps; i++) {
			Vector3 targetScale = (currentStep == i)? bigStepScale: smallStepScale;
			stepMarkers[i].transform.localScale = Vector3.Lerp(stepMarkers[i].transform.localScale, targetScale, Time.deltaTime*speed);
		}
	}

	public void Previous() {
		if(currentStep == 0) {
			SceneManager.LoadScene("start");
			return;
		}

		currentStep--;
		// if (currentStep == 0) tracker.GetObject("previous_button").SetActive(false);
		if (currentStep == nSteps - 2) tracker.GetElement<Text>("next_text").text = "Avançar";
		SetAnimation();
	}

	public void Next() {
		if (currentStep == nSteps - 1) {
			tracker.GetObject("next_button").SetActive(false);
			tracker.GetObject("previous_button").SetActive(false);
			tracker.GetObject("sound_button").SetActive(false);
			tracker.GetObject("loading_text").SetActive(true);

			if(File.Exists(ARUIController.dicaFileName)) {
				File.Delete(ARUIController.dicaFileName);
			}

			// SceneManager.LoadScene("characterselect");
			SceneManager.LoadScene("ar");
			return;
		}

		currentStep++;
		if (currentStep == 1) tracker.GetObject("previous_button").SetActive(true);
		if (currentStep == nSteps - 1) tracker.GetElement<Text>("next_text").text = "Começar";
		SetAnimation();
	}

	private void SetAnimation() {
		if (currentStep == 0) {
			iTween.ScaleFrom(tracker.GetObject("step1_circle"), iTween.Hash(
				"scale", Vector3.zero,
				"time", 1f,
				"easetype", "easeOutBounce"
			));

			iTween.MoveFrom(tracker.GetObject("step1_camisa"), iTween.Hash(
				"y", -1174f,
				"time", 0.5f,
				"easetype", "easeOutQuad",
				"delay", 0.7f,
				"islocal", true
			));

			iTween.ScaleFrom(tracker.GetObject("step1_small"), iTween.Hash(
				"scale", Vector3.zero,
				"time", 0.5f,
				"easetype", "easeOutBack",
				"delay", 2f
			));

			AnimateImageColor(tracker.GetElement<Image>("step1_camisa"), new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), 2f);
		} else if (currentStep == 1) {
			iTween.ScaleFrom(tracker.GetObject("step2_circle"), iTween.Hash(
				"scale", Vector3.zero,
				"time", 1f,
				"easetype", "easeOutBounce"
			));
			iTween.MoveFrom(tracker.GetObject("step2_camisa"), iTween.Hash(
				"y", -231f,
				"time", 1f,
				"easetype", "easeOutBack",
				// "delay", 0.5f,
				"islocal", true
			));
			AnimateImageColor(tracker.GetElement<Image>("step2_camisa"), new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), 1f);

			iTween.ScaleFrom(tracker.GetObject("step2_small"), iTween.Hash(
				"scale", Vector3.zero,
				"time", 0.5f,
				"easetype", "easeOutBack",
				"delay", 3f
			));

			AnimateImageColor(tracker.GetElement<Image>("step2_phone"), new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), 0.6f);
			tracker.GetElement<SmartTutorial>("step2_phone").Play();
		} else if (currentStep == 2) {
			iTween.ScaleFrom(tracker.GetObject("step3_circle"), iTween.Hash(
				"scale", Vector3.zero,
				"time", 1f,
				"easetype", "easeOutBounce"
			));

			iTween.MoveFrom(tracker.GetObject("step3_cellphone"), iTween.Hash(
				"y", -748f,
				"islocal", true,
				"time", 0.5f,
				"easetype", "easeOutQuad",
				"delay", 1f
			));

			iTween.ScaleFrom(tracker.GetObject("step3_small"), iTween.Hash(
				"scale", Vector3.zero,
				"time", 0.5f,
				"easetype", "easeOutBack",
				"delay", 3f
			));

			iTween.MoveFrom(tracker.GetObject("raposa_shadow"), iTween.Hash(
				"y", 188f,
				"time", 0.5f,
				"easetype", "easeOutQuad",
				"delay", 1f,
				"islocal", true
			));
			AnimateImageColor(tracker.GetElement<Image>("raposa_shadow"), new Color(0f, 0f, 0f, 0f), new Color(0f, 0f, 0f, 0.36f), 1f, 1f);
			AnimateImageColor(tracker.GetElement<Image>("raposa_preview"), new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), 1f, 1f);
		} else if (currentStep == 3) {
			iTween.ScaleFrom(tracker.GetObject("step4_circle"), iTween.Hash(
				"scale", Vector3.zero,
				"time", 1f,
				"easetype", "easeOutBounce"
			));

			iTween.ScaleFrom(tracker.GetObject("step4_small"), iTween.Hash(
				"scale", Vector3.zero,
				"time", 0.5f,
				"easetype", "easeOutBack",
				"delay", 3f
			));

			AnimateImageColor(tracker.GetElement<Image>("step4_camera"), new Color(1f, 1f, 1f, 0f), new Color(1f, 1f, 1f, 1f), 1f);
		}
	}

	public void Sound_OnClick () {
		if (AudioListener.volume == 1f) {
			AudioListener.volume = 0f;
			tracker.GetObject("soundoff_image").SetActive(true);
			tracker.GetObject("soundon_image").SetActive(false);
		} else {
			AudioListener.volume = 1f;
			tracker.GetObject("soundon_image").SetActive(true);
			tracker.GetObject("soundoff_image").SetActive(false);
		}
	}

	public void AnimateImageColor(Image image, Color a, Color b, float time, float delay = 0f) {
		IEnumerator coroutine = IAnimateImageColor(image, a, b, time, delay);
		StartCoroutine(coroutine);
	}

	private IEnumerator IAnimateImageColor(Image image, Color a, Color b, float time, float delay = 0f) {
		image.color = a;
		yield return new WaitForSeconds(delay);
		for (float i = 0f; i < 1f; i+=Time.deltaTime/time) {
			image.color = Color.Lerp(a, b, i);
			yield return new WaitForEndOfFrame();
		}
		image.color = b;
	}
}
