using UnityEngine;
using System.Collections;

public class PlatformDefines : MonoBehaviour { // Used to define the platforms and set what each platform need
	GameObject mobileControls;
	
	void Start () {
		mobileControls = GameObject.Find("MobileControls");

		#if UNITY_EDITOR
			Debug.Log("Unity Editor");
		#endif
		
		#if UNITY_ANDROID
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			Debug.Log("Android");
		#endif

		#if UNITY_IPHONE
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			Debug.Log("iPhone");
		#endif

		#if UNITY_STANDALONE_WIN
			mobileControls.SetActive(false);
			Debug.Log("Stand Alone Windows");
		#endif
		
		#if UNITY_STANDALONE_OSX
			mobileControls.SetActive(false);
			Debug.Log("Stand Alone Mac OS X");
		#endif

	}          
}
