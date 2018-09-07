using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CheckVersionButton : MonoBehaviour {

	public Text title;
	public Text message;

	// Use this for initialization
	void Start () {
	
	}

	public void SetText(string title, string message) {
		this.title.text = title;
		this.message.text = message;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
