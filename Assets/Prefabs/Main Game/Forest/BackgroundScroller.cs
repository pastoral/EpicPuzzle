using UnityEngine;
using System.Collections;

public class BackgroundScroller : MonoBehaviour {

	public float scrollSpeed; //Speed of scroll
	private Vector2 savedOffset;
	private new Renderer renderer;


	void Awake(){
		renderer = GetComponent<Renderer> ();
	}
	
	void Start () {
		savedOffset = renderer.sharedMaterial.GetTextureOffset ("_MainTex");
	}
	
	void Update () {
		float x = Mathf.Repeat (Time.time * scrollSpeed, 1); //Repeat x axis
        float y = Mathf.Repeat (Time.time * scrollSpeed / 10, 1); //Repeat y axis
		Vector2 offset = new Vector2 (x, y);
		renderer.sharedMaterial.SetTextureOffset ("_MainTex", offset);
	}
	
	void OnDisable () {
		renderer.sharedMaterial.SetTextureOffset ("_MainTex", savedOffset);
	}

	//Find more information https://unity3d.com/learn/tutorials/modules/beginner/live-training-archive/2d-scrolling-backgrounds
}
