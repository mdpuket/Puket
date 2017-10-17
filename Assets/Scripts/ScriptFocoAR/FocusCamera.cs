/**************************************************************************\
Class: FocusCamera
Project: 
Author: Victor Albenor

Description: 
\**************************************************************************/

using System;
using System.Collections;
using UnityEngine;
using Vuforia;

public class FocusCamera : MonoBehaviour {

    #region Variables
    // Exposed in Editor
    public bool FocusModeSet;
    public static FocusCamera Singleton;
    // Hidden in Editor

    #endregion

    #region MonoBehaviour Implementation
    void Start() {
        Singleton = this;
        FocusModeSet = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        Debug.Log(!FocusModeSet
            ? "Failed to set focus mode to continusauto (unsupported mode)."
            : "Focus should be working properly");
    }
	
	void Update() {
	    if (!FocusModeSet) {
            Debug.Log("Trying to set focus mode");
            FocusModeSet = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        }
	}

    void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus) {
            Debug.Log("pause");
            CameraDevice.Instance.Stop();
            TrackerManager.Instance.GetTracker<ObjectTracker>().Stop();
        }
        else {
            Debug.Log("unpause");
            CameraDevice.Instance.Start();
            FocusModeSet = CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
            TrackerManager.Instance.GetTracker<ObjectTracker>().Start();
        }
    }
    #endregion

    #region Private Methods

    #endregion

    #region Public Methods

    #endregion
}

