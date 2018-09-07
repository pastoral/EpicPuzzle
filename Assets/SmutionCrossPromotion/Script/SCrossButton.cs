using UnityEngine;
using System.Collections;

public class SCrossButton : MonoBehaviour {

	public string appstoreUrl = "";
	public string ggplayUrl = "";
	public string wpUrl = "";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OpenUrl() {
		string url = "";

		#if UNITY_IOS 

		url = appstoreUrl;

		#elif UNITY_ANDROID

		url = ggplayUrl;

		#elif UNITY_WP8 || UNITY_WP8_1

		url = wpUrl;

		#endif

		Application.OpenURL (url);

		Manager.Instance.analytics.LogEvent("Cross Promotion", "Button Press", url, 1);

	}

	public void ClosePopup() {
		gameObject.SetActive (false);

		Manager.Instance.analytics.LogEvent("Cross Promotion", "Button Press", "Cancel", 1);
	}
}
