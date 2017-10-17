using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {
	public static string testResult="";
	public static string confidenceScore ="";
	void OnGUI () 
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			if(GUI.Button(new Rect(Screen.width/2,Screen.height/2-200,200,150),"Start Speech"))
			{
                SpeakNow.startSpeech(LanguageUtil.ENGLISH_US);
			}
            if (GUI.Button(new Rect(Screen.width / 2, Screen.height/2, 200, 150), "Reset"))
            {
                SpeakNow.reset();
            }

            GUI.Label(new Rect(Screen.width/2, Screen.height / 2 + 230, 200, 200),"Speech Result : "+SpeakNow.speechResult());
			GUI.Label(new Rect(Screen.width/2, Screen.height / 2 + 260, 200, 200), SpeakNow.getConfidenceScore().Length>0?"Confidence Score : " + SpeakNow.getConfidenceScore():"");
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2 + 350, 200, 200), SpeakNow.isResultMatches("almost") ? "Matched : " + SpeakNow.isResultMatches("almost") : "");
        }
        else
			{



		GUI.Label(new Rect(Screen.width/2,Screen.height/2,300,200),"Please Build and Run in Android and see documentation for usage of functionalities.");
			}
	}

}
