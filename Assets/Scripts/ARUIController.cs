#define SPEAKNOW
#define ANDROIDNATIVE
//#define KKSPEECH
//#define IOSNATIVE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using Vuforia;
using UnityEngine.SceneManagement;
using System.Linq;
#if KKSPEECH
using KKSpeech;
#endif

public class ARUIController : MonoBehaviour {

	#region Singleton
    private static ARUIController instance;
    public static ARUIController Instance { 
        get {
            return instance;
        }
    }

    private void InitializeSingleton()
    {
        if (instance != null) Destroy(this);
        else instance = this;
    }
    #endregion

	private ComponentTracker tracker;
	private bool closingCommands = false;
	private bool closingMic = false;
	private Animal trackedAnimal;
	private bool mustInstallVoiceSearch = false;

	private bool sonecaChanged = false;
	private float sonecaValue = 0f;
	public static string dicaFileName {
		get {
			return string.Format("{0}/{1}.txt", Application.persistentDataPath, "hint");
		}
	}

	public string testString;
	private float startTime;


	public bool debugSpeech = false;
	private bool waitForSpeakResult = false;

	string tempFileName {
        get {
            return string.Format("{0}/{1}.png", Application.persistentDataPath, "temp");
        }
    }

    private string filePath(string filename) {
    	return string.Format("{0}/{1}.png", Application.persistentDataPath, filename);
    }
    
    public GameObject[] raposaTargets, unicornioTargets, coalaTargets, pandaTargets, dalmataTargets, corujaTargets, dragaoTargets;

    #if KKSPEECH
	private SpeechRecognizerListener kkListener;
	#endif
    private void Awake() {
    	InitializeSingleton();

		startTime = Time.time;
    }

    private IEnumerator fadeVolumeCoroutine;
    private bool fadeVolumeRunning = false;

	void Start () {
		tracker = ComponentTracker.Instance;

        // if (CharacterSelectUIController.selectedCharacter != CharacterSelectUIController.Character.RAPOSA) foreach (GameObject go in raposaTargets) go.SetActive(false);
        // if (CharacterSelectUIController.selectedCharacter != CharacterSelectUIController.Character.UNICORNIO) foreach (GameObject go in unicornioTargets) go.SetActive(false);
        // if (CharacterSelectUIController.selectedCharacter != CharacterSelectUIController.Character.COALA) foreach (GameObject go in coalaTargets) go.SetActive(false);
        // if (CharacterSelectUIController.selectedCharacter != CharacterSelectUIController.Character.PANDA) foreach (GameObject go in pandaTargets) go.SetActive(false);
        // if (CharacterSelectUIController.selectedCharacter != CharacterSelectUIController.Character.DALMATA) foreach (GameObject go in dalmataTargets) go.SetActive(false);
        // if (CharacterSelectUIController.selectedCharacter != CharacterSelectUIController.Character.CORUJA) foreach (GameObject go in corujaTargets) go.SetActive(false);
        // if (CharacterSelectUIController.selectedCharacter != CharacterSelectUIController.Character.DRAGAO) foreach (GameObject go in dragaoTargets) go.SetActive(false);

        tracker.GetObject("commandsexp_group").SetActive(false);
		tracker.GetObject("micexp_group").SetActive(false);
		tracker.GetObject("share_panel").SetActive(false);
		tracker.GetObject("config_panel").SetActive(false);
		tracker.GetObject("about_panel").SetActive(false);
		tracker.GetObject("terms_panel").SetActive(false);
		tracker.GetElement<Button>("commands_button").interactable = false;
		tracker.GetElement<Button>("mic_button").interactable = false;
		tracker.GetObject("dica1_panel").SetActive(false);
		tracker.GetObject("dica2_panel").SetActive(false);
        tracker.GetObject("vuforia_mask").SetActive(false);

        if (AudioListener.volume == 1f) tracker.GetObject("soundoff_image").SetActive(false);
		else tracker.GetObject("soundon_image").SetActive(false);

		string [] filenames = GetImageFiles();
		for(int i = 0; i < filenames.Length; i++) {
			Debug.Log(filenames[i]);
		}

		if (!File.Exists(dicaFileName)) {
			tracker.GetObject("dica1_panel").SetActive(true);
			iTween.MoveTo(tracker.GetObject("dica1_seta"), iTween.Hash(
				"y", -354.2f,
				"time", 0.5f,
				"easetype", "easeInOutQuad",
				"looptype", "pingPong",
				"islocal", true
			));

			iTween.ScaleTo(tracker.GetObject("dica1_circle"), iTween.Hash(
				"scale", Vector3.one*1.391557f,
				"time", 0.5f,
				"easetype", "easeInOutQuad",
				"looptype", "pingPong",
				"islocal", true
			));

			iTween.MoveTo(tracker.GetObject("dica2_seta"), iTween.Hash(
				"y", -354.2f,
				"time", 0.5f,
				"easetype", "easeInOutQuad",
				"looptype", "pingPong",
				"islocal", true
			));

			iTween.ScaleTo(tracker.GetObject("dica2_circle"), iTween.Hash(
				"scale", Vector3.one*1.391557f,
				"time", 0.5f,
				"easetype", "easeInOutQuad",
				"looptype", "pingPong",
				"islocal", true
			));
		}

		#if KKSPEECH
		if (SpeechRecognizer.ExistsOnDevice()) {
			kkListener = GameObject.FindObjectOfType<SpeechRecognizerListener>();
			kkListener.onAuthorizationStatusFetched.AddListener(OnAuthorizationStatusFetched);
			kkListener.onAvailabilityChanged.AddListener(OnAvailabilityChange);
			kkListener.onErrorDuringRecording.AddListener(OnError);
			kkListener.onErrorOnStartRecording.AddListener(OnError);
			kkListener.onFinalResults.AddListener(OnFinalResult);
			kkListener.onPartialResults.AddListener(OnPartialResult);
			kkListener.onEndOfSpeech.AddListener(OnEndOfSpeech);
			SpeechRecognizer.RequestAccess();
		} else {
			Debug.Log("Sorry, but this device doesn't support speech recognition");
		}
#endif
    }

    public bool test = false;
	void Update() {
        if (test)
        {
            test = false;

            ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
            DataSet dataSet = objectTracker.CreateDataSet();
            string dataSetName = Application.persistentDataPath + "Puket_Panda.xml";

            if (dataSet.Load(dataSetName))
            {
                objectTracker.Stop();  // stop tracker so that we can add new dataset

                if (!objectTracker.ActivateDataSet(dataSet))
                {
                    // Note: ImageTracker cannot have more than 100 total targets activated
                    Debug.Log("<color=yellow>Failed to Activate DataSet: " + dataSetName + "</color>");
                }

                if (!objectTracker.Start())
                {
                    Debug.Log("<color=yellow>Tracker Failed to Start.</color>");
                }
            }
            else
            {
                Debug.LogError("<color=yellow>Failed to load dataset: '" + dataSetName + "'</color>");
            }
        }

        if (sonecaChanged) {
			if (!Input.GetMouseButton(0) && Input.touchCount == 0) {
				sonecaChanged = false;
				MenuUIController.alarmClock = DateTime.Now.AddMilliseconds(sonecaValue*60f*60f*1000f);
				File.WriteAllText(MenuUIController.alarmClockFileName, MenuUIController.alarmClock.ToString());
			}
		}

		if (waitForSpeakResult) {
			// Debug.Log("waiting - " + string.IsNullOrEmpty(SpeakNow.speechResult()));
			#if SPEAKNOW
			if (!string.IsNullOrEmpty(SpeakNow.speechResult())) {
				waitForSpeakResult = false;
				if (SpeakNow.speechResult() == "error from server") {
					tracker.GetElement<Popup>("serverError_popup").Activate(6f);
				} else if (SpeakNow.speechResult() == "No Match") {
					tracker.GetElement<Popup>("noMatch_popup").Activate(6f);
				} else {
					bool matched = false;
					for (int i = 0; i < trackedAnimal.actions.Length; i++) {
						if (SpeakNow.isResultMatches(trackedAnimal.actions[i].actionName.ToLower())) {
							trackedAnimal.TriggerAnimation(trackedAnimal.actions[i].actionId);
							Debug.Log("Action matched: " + trackedAnimal.actions[i].actionName);
							matched = true;
							break;
						}
					}
					if (!matched) {
						tracker.GetElement<Popup>("noMatched_popup").Activate(6f);
					}
				}
				// Debug.Log("closing microphone");
				CloseMicrophone_OnClick();
			}
			#else
			waitForSpeakResult = false;
			CloseMicrophone_OnClick();
			#endif
		}
	}

	private string[] GetImageFiles() {
	    string[] files = Directory.GetFiles(Application.persistentDataPath, "*.png");
	    for(int i = 0; i < files.Length; i++)
	        files[i] = Path.GetFileName(files[i]).Replace(".png", "");
	    return files;
	}

	public void Commands_OnClick() {
		tracker.GetObject("commandsexp_group").SetActive(true);
		
		iTween.ScaleFrom(tracker.GetObject("commandsexp_shadow_image"), iTween.Hash(
			"scale", Vector3.zero,
			"islocal", true,
			"time", 0.3f,
			"easetype", "easeOutExpo"
		));

		iTween.ScaleFrom(tracker.GetObject("commandsexp_bg_image"), iTween.Hash(
			"scale", Vector3.zero,
			"islocal", true,
			"time", 0.3f,
			"delay", 0.1f,
			"easetype", "easeOutExpo"
		));
	}

	public void CloseCommands_OnClick() {
		if (closingCommands) return;

		StartCoroutine(ICloseCommands());
	}

	private IEnumerator ICloseCommands () {
		closingCommands = true;
		Vector3 shadowScale = tracker.GetElement<Transform>("commandsexp_shadow_image").localScale;
		Vector3 bgScale = tracker.GetElement<Transform>("commandsexp_bg_image").localScale;

		iTween.ScaleTo(tracker.GetObject("commandsexp_shadow_image"), iTween.Hash(
			"scale", Vector3.zero,
			"islocal", true,
			"delay", 0.1f,
			"time", 0.3f,
			"easetype", "easeInExpo"
		));

		iTween.ScaleTo(tracker.GetObject("commandsexp_bg_image"), iTween.Hash(
			"scale", Vector3.zero,
			"islocal", true,
			"time", 0.3f,
			"easetype", "easeInExpo"
		));

		yield return new WaitForSeconds(0.4f);

		tracker.GetElement<Transform>("commandsexp_shadow_image").localScale = shadowScale;
		tracker.GetElement<Transform>("commandsexp_bg_image").localScale = bgScale;
		tracker.GetObject("commandsexp_group").SetActive(false);
		closingCommands = false;
	}

	public void Microphone_OnClick() {
		tracker.GetObject("micexp_group").SetActive(true);
		
		iTween.ScaleFrom(tracker.GetObject("micexp_shadow_image"), iTween.Hash(
			"scale", Vector3.zero,
			"islocal", true,
			"time", 0.3f,
			"easetype", "easeOutExpo"
		));

		iTween.ScaleFrom(tracker.GetObject("micexp_bg_image"), iTween.Hash(
			"scale", Vector3.zero,
			"islocal", true,
			"time", 0.3f,
			"delay", 0.1f,
			"easetype", "easeOutExpo"
		));

		if (!mustInstallVoiceSearch) {
			mustInstallVoiceSearch = true;
        	tracker.GetElement<Popup>("mustInstallVoiceSearch_popup").Activate(5f);
		}

		FadeVolume(0f, 0.5f);
		WaitAndActivateSpeaker(0.5f);
        Debug.Log("microphone click");
	}

	private void WaitAndActivateSpeaker(float delay) {
		IEnumerator coroutine = IWaitAndActivateSpeaker(delay);
		StartCoroutine(coroutine);
	}

	private IEnumerator IWaitAndActivateSpeaker(float delay) {
		yield return new WaitForSeconds(delay);

#if SPEAKNOW
        waitForSpeakResult = true;
        SpeakNow.reset();
        SpeakNow.startSpeech(LanguageUtil.PORTUGUESE_BRAZIL);
#endif

#if KKSPEECH
        if (!SpeechRecognizer.IsRecording()) {
        	SpeechRecognizer.StartRecording(true);
        	IEnumerator coroutine = WaitAndStopRecognition (5.5f); // segundos para reconhecer a voz
			StartCoroutine(coroutine);
        }
#endif
	}

	public void CloseMicrophone_OnClick() {
		if (closingMic) return;

		StartCoroutine(ICloseMicrophone());
	}

	public void TakeScreenshot_OnClick () {
		StartCoroutine(ITakeScreenshot());
	}

	public void CloseScreenshot_OnClick() {
		tracker.GetObject("share_panel").SetActive(false);
        tracker.GetObject("header_panel").SetActive(true);
        tracker.GetObject("footer_panel").SetActive(true);
        if (File.Exists(tempFileName)) File.Delete(tempFileName);
	}

	public void SaveScreenshot_OnClick() {
		tracker.GetObject("share_panel").SetActive(false);
		// if (File.Exists(tempFileName)) {
		// 	Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
	 //        tex.LoadImage(File.ReadAllBytes(tempFileName));
	 //        tracker.GetElement<RawImage>("galeryImage_rawImage").texture = tex;
		// 	File.Move(tempFileName, filePath(DateTime.Now.ToString("yyyy-dd-M-HH-mm-ss")));
		// }
		if (File.Exists(tempFileName))
        {
            //string targetLocation = string.Format("{0}/RealityMug_{1}.png", Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            //Debug.Log("Moving to " + targetLocation);
            //File.Copy(tempFileName, targetLocation);

#if UNITY_ANDROID
#if ANDROIDNATIVE
            AndroidCamera.instance.SaveImageToGallery(File.ReadAllBytes(tempFileName), string.Format("Puket_{0}.png", System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")));
#endif
#elif UNITY_IOS
#if IOSNATIVE
   			IOSCamera.OnImageSaved += OnImageSaved;
			IOSCamera.Instance.SaveTextureToCameraRoll(File.ReadAllBytes(tempFileName));
#endif
#endif
            tracker.GetElement<Popup>("imageSaved_popup").Activate(3f);
        }

        tracker.GetObject("header_panel").SetActive(true);
        tracker.GetObject("footer_panel").SetActive(true);
	}

	public void SwapCamera_OnClick () {
		CameraDevice.CameraDirection currentDir = CameraDevice.Instance.GetCameraDirection();
        if (currentDir == CameraDevice.CameraDirection.CAMERA_BACK || currentDir == CameraDevice.CameraDirection.CAMERA_DEFAULT) {
        	// if (!CameraDevice.Instance.Init(CameraDevice.CameraDirection.CAMERA_FRONT)) RestartCamera(CameraDevice.CameraDirection.CAMERA_FRONT, true);
         	//  else tracker.GetElement<Popup>("noFrontCamera_popup").Activate(3f);
        	RestartCamera(CameraDevice.CameraDirection.CAMERA_FRONT, true);
        } else {
            RestartCamera(CameraDevice.CameraDirection.CAMERA_BACK, false);
        }
	}

	IEnumerator ITakeScreenshot() {
// #if NATIVE
        tracker.GetObject("header_panel").SetActive(false);
        tracker.GetObject("footer_panel").SetActive(false);
        tracker.GetObject("vuforia_mask").SetActive(true);

        if(Application.platform == RuntimePlatform.WindowsEditor) Application.CaptureScreenshot(tempFileName);
        else Application.CaptureScreenshot("temp.png");
        
        print("waiting");
        while (!File.Exists(tempFileName)) yield return new WaitForEndOfFrame();
		print ("Security delay: 1f");
		yield return new WaitForSeconds(1f);
        print("done");
        
        // tracker.GetObject("header_panel").SetActive(true);
        // tracker.GetObject("footer_panel").SetActive(true);

        tracker.GetObject("share_panel").SetActive(true);
        tracker.GetObject("vuforia_mask").SetActive(false);

        Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        tex.LoadImage(File.ReadAllBytes(tempFileName));
        tracker.GetElement<RawImage>("print_image").texture = tex;
// #else
		yield return new WaitForEndOfFrame();
// #endif
    }

    public void OnTrackAnimal(Animal animal) {
    	if (Time.time - startTime > 0.5f) {
			tracker.GetElement<Button>("commands_button").interactable = true;
#if KKSPEECH
			// if (SpeechRecognizer.ExistsOnDevice())
				tracker.GetElement<Button>("mic_button").interactable = true;
#else
			tracker.GetElement<Button>("mic_button").interactable = true;
#endif
    	}

		// nomear os botoes
		tracker.GetElement<Text>("action1_commands_label").text = animal.actions[0].actionName;
		tracker.GetElement<Text>("action2_commands_label").text = animal.actions[1].actionName;
		tracker.GetElement<Text>("action3_commands_label").text = animal.actions[2].actionName;
		tracker.GetElement<Text>("action1_mic_label").text = animal.actions[0].actionName;
		tracker.GetElement<Text>("action2_mic_label").text = animal.actions[1].actionName;
		tracker.GetElement<Text>("action3_mic_label").text = animal.actions[2].actionName;
		trackedAnimal = animal;

        // if(Time.time - startTime > 3f && !animal.gameObject.GetComponent<Animator>().GetBool("sleeping")) tracker.GetElement<AudioSource>("temp_track").Play();
        if (!animal.gameObject.GetComponent<Animator>().GetBool("sleeping")) tracker.GetElement<AudioSource>("temp_track").Play();
    }

    public void OnLostTacking() {
		tracker.GetElement<Button>("commands_button").interactable = false;
		tracker.GetElement<Button>("mic_button").interactable = false;
		CloseMicrophone_OnClick();
		CloseCommands_OnClick();
		trackedAnimal = null;
		Camera.main.GetComponent<AudioSource>().Stop();
		tracker.GetElement<AudioSource>("temp_track").Stop();
    }

	private IEnumerator ICloseMicrophone () {
		closingMic = true;
		Vector3 shadowScale = tracker.GetElement<Transform>("micexp_shadow_image").localScale;
		Vector3 bgScale = tracker.GetElement<Transform>("micexp_bg_image").localScale;
		FadeVolume(1f, 0.5f);

		iTween.ScaleTo(tracker.GetObject("micexp_shadow_image"), iTween.Hash(
			"scale", Vector3.zero,
			"islocal", true,
			"delay", 0.1f,
			"time", 0.3f,
			"easetype", "easeInExpo"
		));

		iTween.ScaleTo(tracker.GetObject("micexp_bg_image"), iTween.Hash(
			"scale", Vector3.zero,
			"islocal", true,
			"time", 0.3f,
			"easetype", "easeInExpo"
		));

		yield return new WaitForSeconds(0.4f);

		tracker.GetElement<Transform>("micexp_shadow_image").localScale = shadowScale;
		tracker.GetElement<Transform>("micexp_bg_image").localScale = bgScale;
		tracker.GetObject("micexp_group").SetActive(false);
		closingMic = false;
	}

	public void Action1_OnClick() {
		trackedAnimal.TriggerAnimation(trackedAnimal.actions[0].actionId);
	}

	public void Action2_OnClick() {
		trackedAnimal.TriggerAnimation(trackedAnimal.actions[1].actionId);
	}

	public void Action3_OnClick() {
		trackedAnimal.TriggerAnimation(trackedAnimal.actions[2].actionId);
	}

	public void Config_OnClick() {
		tracker.GetObject("config_panel").SetActive(true);

		if (MenuUIController.alarmClock.CompareTo(DateTime.Now) <= 0) {
			tracker.GetElement<Slider>("soneca_slider").value = 0;
			tracker.GetElement<Text>("soneca_label_hour").text = "0h";
		} else {
			tracker.GetElement<Slider>("soneca_slider").value = (float)MenuUIController.alarmClock.Subtract(DateTime.Now).TotalMilliseconds/1000f/60f/60f/12f;
			tracker.GetElement<Text>("soneca_label_hour").text = Mathf.Floor((float)MenuUIController.alarmClock.Subtract(DateTime.Now).TotalMilliseconds/1000f/60f/60f) + "h";
		}
	}

	public void CloseConfig_OnClick() {
		tracker.GetObject("config_panel").SetActive(false);	
	}

	public void Tutorial_OnClick() {
		SceneManager.LoadScene("tutorial");
	}

	public void Terms_OnClick() {
		tracker.GetObject("terms_panel").SetActive(true);
		// tracker.GetElement<AudioSource>("soundtrack").Pause();
	}

	public void CloseTerms_OnClick() {
		tracker.GetObject("terms_panel").SetActive(false);
		// tracker.GetElement<AudioSource>("soundtrack").Play();
	}

	public void About_OnClick() {
		tracker.GetObject("about_panel").SetActive(true);
	}

	public void CloseAbout_OnClick() {
		tracker.GetObject("about_panel").SetActive(false);
	}

	public void CloseApp_OnClick() {
		Application.Quit();
	}

	public void Soneca_OnValueChange () {
		sonecaValue = Mathf.Lerp(0f, 12f, tracker.GetElement<Slider>("soneca_slider").value);
		sonecaChanged = true;
		tracker.GetElement<Text>("soneca_label_hour").text = Mathf.Floor(sonecaValue) + "h";
	}

	public void Dica1_OnClick() {
		tracker.GetObject("dica1_panel").SetActive(false);
		tracker.GetObject("dica2_panel").SetActive(true);
	}

	public void Dica2_OnClick() {
		tracker.GetObject("dica2_panel").SetActive(false);
		if(!File.Exists(dicaFileName)) File.WriteAllLines(dicaFileName, new string[0]);
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

	public void ChangeCharacter_OnClick() {
		SceneManager.LoadScene("characterselect");
	}

	private void FadeVolume (float volume, float fadeTime) {
		if (tracker.GetObject("soundon_image").activeSelf) {
			if(fadeVolumeRunning) StopCoroutine(fadeVolumeCoroutine);
			fadeVolumeCoroutine = IFadeVolume(volume, fadeTime);
			StartCoroutine(fadeVolumeCoroutine);
		}
	}

	private IEnumerator IFadeVolume (float volume, float fadeTime) {
		fadeVolumeRunning = true;
		float oldVolume = AudioListener.volume;
		for (float i = 0f; i < 1f; i += Time.deltaTime/fadeTime) {
			AudioListener.volume = Mathf.Lerp(oldVolume, volume, i);
			yield return new WaitForEndOfFrame();
		}
        AudioListener.volume = volume;
        fadeVolumeRunning = false;
	}

	private void RestartCamera(CameraDevice.CameraDirection newDir, bool mirror)
    {
        CameraDevice.Instance.Stop();
        CameraDevice.Instance.Deinit();
        
   //      CameraDevice.CameraDirection currentDir = CameraDevice.Instance.GetCameraDirection();
       	// Debug.Log("CameraDevice.Init(" + newDir + "): " + CameraDevice.Instance.Init(newDir));
   //      if(!CameraDevice.Instance.Init(newDir)) {
   //      	tracker.GetElement<Popup>("noFrontCamera_popup").Activate(3f);
			// CameraDevice.Instance.Deinit();
			// CameraDevice.Instance.Init(currentDir);
   //      }

        testString = CameraDevice.Instance.Init(newDir).ToString();

        // Set mirroring 
        /*var config = QCARRenderer.Instance.GetVideoBackgroundConfig();
        config.reflection = mirror ? QCARRenderer.VideoBackgroundReflection.ON : QCARRenderer.VideoBackgroundReflection.OFF;
        QCARRenderer.Instance.SetVideoBackgroundConfig(config);*/

        Debug.Log(CameraDevice.Instance.Start());
    }

#if IOSNATIVE
    private void OnImageSaved(SA.Common.Models.Result result)
    {
        IOSCamera.OnImageSaved -= OnImageSaved;
        if (result.IsSucceeded)
        {
            IOSMessage.Create("Success", "Image Successfully saved to Camera Roll");
        }
        else
        {
            IOSMessage.Create("ERROR", "Image Save Failed");
        }
    }
#endif

#if KKSPEECH
	public void OnFinalResult(string result) {
		// resultText.text = result;
		Debug.Log("OnFinalResult: " + result);
	}

	public void OnPartialResult(string result) {
		// resultText.text = result;
		Debug.Log("OnPartialResult: " + result);
		if (!string.IsNullOrEmpty(result)) {
			waitForSpeakResult = false;
			//tracker.GetElement<Popup>("serverError_popup").Activate(6f);
			//tracker.GetElement<Popup>("noMatch_popup").Activate(6f);
			bool matched = false;
			string[] results = result.Split (' ');
			Debug.Log ("Partial results: " + results.Length);
			for (int i = 0; !matched && i < trackedAnimal.actions.Length; i++) {
				for (int j = 0; !matched && j < results.Length; j++) {
					if (trackedAnimal.actions[i].actionName.ToLower() == results [j].ToLower()) {
						trackedAnimal.TriggerAnimation(trackedAnimal.actions[i].actionId);
						Debug.Log("Action matched: " + trackedAnimal.actions[i].actionName);
						matched = true;
					}
				}
			}
			if (!matched) {
				tracker.GetElement<Popup>("noMatched_popup").Activate(6f);
			}
		}
		Debug.Log("closing microphone");
		CloseMicrophone_OnClick();
	}

	public void OnAvailabilityChange(bool available) {
		if(!available) {
			tracker.GetElement<Button>("mic_button").interactable = false;
		} else if(tracker.GetElement<Button>("commands_button").interactable) {
			tracker.GetElement<Button>("mic_button").interactable = true;
		}	
	}

	public void OnAuthorizationStatusFetched(AuthorizationStatus status) {
		switch (status) {
		case AuthorizationStatus.Authorized:
			// startRecordingButton.enabled = true;
			OnAvailabilityChange(true);
			break;
		default:
			// startRecordingButton.enabled = false;
			OnAvailabilityChange(false);
			Debug.Log("Cannot use Speech Recognition, authorization status is " + status);
			break;
		}
	}

	public void OnEndOfSpeech() {
		// startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
		Debug.Log("End Of Speech");
		waitForSpeakResult = false;
		CloseMicrophone_OnClick();
	}

	public void OnError(string error) {
		Debug.LogError("Something went wrong... Try again! \n [" + error + "]");
		// startRecordingButton.GetComponentInChildren<Text>().text = "Start Recording";
		waitForSpeakResult = false;
		CloseMicrophone_OnClick();
	}

	private IEnumerator WaitAndStopRecognition (float seconds) {
		yield return new WaitForSeconds(seconds);
		if (SpeechRecognizer.IsRecording()) {
			SpeechRecognizer.StopIfRecording();
		}
	}
#endif

    private void OnGUI () {
    	// GUI.Label(new Rect(10f, 200f, 200f, 200f),"Init: " + testString);
    	if (debugSpeech) {
    		#if SPEAKNOW
    		GUI.Label(new Rect(10f, 200f, 200f, 200f),"Speech Result : " + SpeakNow.speechResult());
			GUI.Label(new Rect(10f, 250f, 200f, 200f), "Confidence Score : " + SpeakNow.getConfidenceScore());
			GUI.Label(new Rect(10f, 300f, 200f, 200f), "getListOfResults: " + string.Join(", ", SpeakNow.getListOfResults()));
			GUI.Label(new Rect(10f, 350f, 200f, 200f), "getListOfConfidenceScores: " + string.Join(", ", SpeakNow.getListOfConfidenceScores()));
			GUI.Label(new Rect(10f, 400f, 200f, 200f), "isResultMatches(carro)? " + SpeakNow.isResultMatches("carro"));
			#endif
		}
    }
}
