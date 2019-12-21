using UnityEngine;
using System.Collections;

public class PlatformDefines : MonoBehaviour { // Used to define the platforms and set what each platform need
	GameObject joystick;
	
	void Start () {
		joystick = GameObject.Find("Fixed Joystick");

		#if UNITY_EDITOR
			Debug.Log("Unity Editor");
		#endif
		
		#if UNITY_ANDROID
			Debug.Log("Android");
		#endif

		#if UNITY_IPHONE
			Debug.Log("iPhone");
		#endif

		#if UNITY_STANDALONE_WIN
			joystick.SetActive(false);
			Debug.Log("Stand Alone Windows");
		#endif
		
		#if UNITY_STANDALONE_OSX
			joystick.SetActive(false);
			Debug.Log("Stand Alone Mac OS X");
		#endif

	}          
}
