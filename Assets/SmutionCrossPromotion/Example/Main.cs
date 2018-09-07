using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {

	// Use this for initialization
	void Start () {
		SCross.Instance.GetImage ("cyrus_bean_jump", (result, message) => {
			if (result) {
				SCross.Instance.ShowPopupScross();
			}
		});
//
//		SCross.Instance.ShowMessage ("cryus", "1.0.0", (result, message) => {
//
//		});

//		SCross.Instance.ShowPopup();
	}


	
	// Update is called once per frame
	void Update () {
	
	}
}
