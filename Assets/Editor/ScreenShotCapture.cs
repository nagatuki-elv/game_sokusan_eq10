using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class ScreenShotCapture : MonoBehaviour {

	[MenuItem("ScreenShot/Capture _F12")]
	private static void Capture () {
		string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
		ScreenCapture.CaptureScreenshot(fileName);
		Debug.Log("Capture Successed. " + fileName);
	}
}
