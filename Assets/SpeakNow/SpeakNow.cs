using System.Collections;
using UnityEngine;

public class SpeakNow : MonoBehaviour{
    #if !UNITY_EDITOR
    #endif
    private static AndroidJavaClass uClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    private static AndroidJavaClass pClass = new AndroidJavaClass("com.vishnu.speaknow.SpeakNow");
    private static AndroidJavaObject uObject = uClass.GetStatic<AndroidJavaObject>("currentActivity");
    private static AndroidJavaObject jo = pClass.CallStatic<AndroidJavaObject>("getInstance");

    public static void startSpeech(string language)
    {
        jo.Call("reset");
        jo.Call("startSpeech",uObject,language);
    }

    public static string speechResult()
    {
        return jo.Call<string>("getResult");
    }

    public static string getConfidenceScore()
    {
        return jo.Call<string>("getConfidenceScore");
    }

    public static string[] getListOfResults()
    {
        return jo.Call<string[]>("getListOfWords");
    }

    public static string[] getListOfConfidenceScores()
    {
        return jo.Call<string[]>("getListOfConfidenceScores");
    }

    public static bool isResultMatches(string checkString)
    {
        return jo.Call<bool>("contains", checkString);
    }

    public static void reset()
    {
        jo.Call("reset");
    }

}
