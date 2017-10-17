using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class MenuUIController : MonoBehaviour {

	public static bool TermsAccepted = false;

	private ComponentTracker tracker;

	private bool begins = false;
	private static bool openAnimation = false;

	private bool sonecaChanged = false;
	private float sonecaValue = 0f;
	
	private string acceptTermsFileName;
	public static DateTime alarmClock = DateTime.Now;

	public GameObject soundtrack;

	public static string alarmClockFileName {
        get {
            return string.Format("{0}/{1}.txt", Application.persistentDataPath, "alarmClock");
        }
    }

	void Start () {
		tracker = ComponentTracker.Instance;
		acceptTermsFileName = string.Format("{0}/{1}.txt", Application.persistentDataPath, "accepted");

		tracker.GetObject("loading_text").SetActive(false);
		tracker.GetObject("config_panel").SetActive(false);
		tracker.GetObject("about_panel").SetActive(false);
		tracker.GetObject("prancheta_text").SetActive(false);
		tracker.GetObject("config_button").SetActive(false);
		tracker.GetObject("sound_button").SetActive(false);
		tracker.GetObject("begin_button").SetActive(false);
		tracker.GetObject("exit_button").SetActive(false);
		tracker.GetObject("terms_button").SetActive(false);
		tracker.GetObject("loadingBar").SetActive(false);
		
		// if (!Soundtrack.instantiated) {
		// 	GameObject go = GameObject.Instantiate(soundtrack, Vector3.zero, transform.rotation) as GameObject;
		// }

		tracker.GetElement<Toggle>("accept_toggle").isOn = TermsAccepted;

		if (openAnimation || File.Exists(acceptTermsFileName)) {
			openAnimation = true;
			tracker.GetObject("terms_panel").SetActive(false);
			tracker.GetElement<AudioSource>("soundtrack").Play();
			StartAnimation(0.5f);
		}

		if (AudioListener.volume == 1f) tracker.GetObject("soundoff_image").SetActive(false);
		else tracker.GetObject("soundon_image").SetActive(false);

		if (File.Exists(alarmClockFileName)) {
			// Debug.Log(File.ReadAllText(alarmClockFileName));
			alarmClock = DateTime.Parse(File.ReadAllText(alarmClockFileName));
			// Debug.Log(alarmClock.Subtract(DateTime.Now).TotalMilliseconds/1000f/60f/60f);
		} else {
			alarmClock = DateTime.Now;
		}
	}

	private void Update () {
		// Debug.Log(Input.GetMouseButton(0));
		if (sonecaChanged) {
			if (!Input.GetMouseButton(0) && Input.touchCount == 0) {
				sonecaChanged = false;
				alarmClock = DateTime.Now.AddMilliseconds(sonecaValue*60f*60f*1000f);
				File.WriteAllText(alarmClockFileName, alarmClock.ToString());
			}
		}
	}
	
	public void Begin_OnClick() {

		tracker.GetObject("config_button").SetActive(false);
		tracker.GetObject("sound_button").SetActive(false);
		tracker.GetObject("prancheta_text").SetActive(false);
		tracker.GetObject("terms_button").SetActive(false);
		tracker.GetObject("begin_button").SetActive(false);
		tracker.GetObject("exit_button").SetActive(false);
		tracker.GetObject("loading_text").SetActive(true);
		tracker.GetObject("loadingBar").SetActive(true);

		iTween.MoveTo(tracker.GetObject("logo_image"), iTween.Hash(
			"y", 69f,
			"islocal", true,
			"easetype", "easeOutExpo",
			"time", 0.3f
		));

		if (File.Exists(TutorialUIController.tutorialReadedFileName)) {
			// SceneManager.LoadScene("ar");
			// tracker.GetElement<FakeLoadingBar>("loadingBar").Run("characterselect");
			tracker.GetElement<FakeLoadingBar>("loadingBar").Run("ar");
		} else {
			// SceneManager.LoadScene("tutorial");
			tracker.GetElement<FakeLoadingBar>("loadingBar").Run("tutorial");
		}
		
	}

	public void Exit_OnClick() {
		Application.Quit();
	}

	public void Config_OnClick() {
		tracker.GetObject("config_panel").SetActive(true);

		if (alarmClock.CompareTo(DateTime.Now) <= 0) {
			tracker.GetElement<Slider>("soneca_slider").value = 0;
			tracker.GetElement<Text>("soneca_label_hour").text = "0h";
		} else {
			tracker.GetElement<Slider>("soneca_slider").value = (float)alarmClock.Subtract(DateTime.Now).TotalMilliseconds/1000f/60f/60f/12f;
			tracker.GetElement<Text>("soneca_label_hour").text = Mathf.Floor((float)alarmClock.Subtract(DateTime.Now).TotalMilliseconds/1000f/60f/60f) + "h";
		}
	}

	public void CloseConfig_OnClick() {
		tracker.GetObject("config_panel").SetActive(false);	
	}

	public void About_OnClick() {
		tracker.GetObject("about_panel").SetActive(true);
	}

	public void CloseAbout_OnClick() {
		tracker.GetObject("about_panel").SetActive(false);
	}

	public void Terms_OnClick() {
		tracker.GetObject("terms_panel").SetActive(true);
		tracker.GetElement<AudioSource>("soundtrack").Pause();
		tracker.GetElement<Toggle>("accept_toggle").isOn = TermsAccepted;
	}

	public void AcceptTerms_OnChange() {
		TermsAccepted = tracker.GetElement<Toggle>("accept_toggle").isOn;
		tracker.GetElement<Button>("terms_button_confirm").interactable = TermsAccepted;
	}

	public void TermsConfirm_OnClick() {
		tracker.GetObject("terms_panel").SetActive(false);
		tracker.GetObject("accept_toggle").SetActive(false);
		tracker.GetElement<AudioSource>("soundtrack").Play();
		tracker.GetElement<Text>("terms_button_confirm_text").text = "CONTINUAR";
		// TermsAccepted = tracker.GetElement<Toggle>("accept_toggle").isOn;

		if (!openAnimation) {
			if(!File.Exists(acceptTermsFileName)) File.WriteAllLines(acceptTermsFileName, new string[0]);
			openAnimation = true;
			StartAnimation(0f);
		}
	}

	private void StartAnimation (float delay) {
		tracker.GetObject("terms_button").SetActive(true);
		tracker.GetObject("config_button").SetActive(true);
		tracker.GetObject("sound_button").SetActive(true);
		tracker.GetObject("begin_button").SetActive(true);
		tracker.GetObject("prancheta_text").SetActive(true);
		tracker.GetObject("exit_button").SetActive(true);

		iTween.MoveFrom(tracker.GetObject("logo_image"), iTween.Hash(
			"y", 1218f,
			"time", 2f,
			"easetype", "easeOutElastic",
			"islocal", true,
			"delay", delay
		));

		iTween.MoveFrom(tracker.GetObject("terms_button"), iTween.Hash(
			"y", -988f,
			"time", 0.5f,
			"easetype", "easeOutExpo",
			"islocal", true,
			"delay", delay
		));

		iTween.MoveFrom(tracker.GetObject("config_button"), iTween.Hash(
			"y", 957f,
			"time", 0.5f,
			"easetype", "easeOutExpo",
			"islocal", true,
			"delay", 0.1f + delay
		));

		iTween.MoveFrom(tracker.GetObject("sound_button"), iTween.Hash(
			"y", 957f,
			"time", 0.5f,
			"easetype", "easeOutExpo",
			"islocal", true,
			"delay", 0.2f + delay
		));

		iTween.MoveFrom(tracker.GetObject("begin_button"), iTween.Hash(
			"x", 1203f,
			"time", 0.5f,
			"easetype", "easeOutExpo",
			"islocal", true,
			"delay", delay
		));

		iTween.MoveFrom(tracker.GetObject("prancheta_text"), iTween.Hash(
			"y", 1124f,
			"time", 0.5f,
			"easetype", "easeOutExpo",
			"islocal", true,
			"delay", delay
		));

		iTween.MoveFrom(tracker.GetObject("exit_button"), iTween.Hash(
			"x", -999f,
			"time", 0.5f,
			"easetype", "easeOutExpo",
			"islocal", true,
			"delay", delay
		));
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

	public void Tutorial_OnClick () {
		SceneManager.LoadScene("tutorial");
	}

	public void Soneca_OnValueChange () {
		sonecaValue = Mathf.Lerp(0f, 12f, tracker.GetElement<Slider>("soneca_slider").value);
		sonecaChanged = true;
		tracker.GetElement<Text>("soneca_label_hour").text = Mathf.Floor(sonecaValue) + "h";
	}

	public void ImageAlphaTransiction(Image image, float to, float time) {
		ImageAlphaTransiction(image, image.color.a, to, time);
	}

	public void ImageAlphaTransiction(Image image, float from, float to, float time) {
		IEnumerator coroutine = IAlphaTransiction(image, from, to, time);
		StartCoroutine(coroutine);
	}

	private IEnumerator IAlphaTransiction (Image image, float from, float to, float time) {
		for (float i = 0f; i < time; i += Time.deltaTime) {
			image.color = new Color(image.color.r, image.color.g, image.color.b, Mathf.Lerp(from, to, i));
			yield return new WaitForEndOfFrame();
		}
		image.color = new Color(image.color.r, image.color.g, image.color.b, to);
	}
}
