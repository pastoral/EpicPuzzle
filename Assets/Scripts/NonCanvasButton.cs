using UnityEngine;
using System.Collections;
using System;

public class NonCanvasButton : MonoBehaviour, ITouchEventListener {

	public BoxCollider2D bound;

	public Action touchPressEvent = null;

	// Use this for initialization
	void Start () {
		TouchManager.Instance.AddEventListener(this, 2);
	}
	
	// Update is called once per frame
	void Update () {
		
	
	}

	public bool OnTouchPressed(Vector3 position)
	{
		if (gameObject.activeSelf)
		if (bound.bounds.Contains (position)) {
			OnTouchPressed ();
			return true;
		}
		return false;
	}

	public bool OnTouchMoved(Vector3 position)
	{
		return false;
	}

	public void OnTouchReleased(Vector3 position)
	{
	}

	public void OnTouchPressed()
	{
		gameObject.Play (GetJellyAction());

		Invoke ("ActionPerformance", 0.3f);
	}

	public void ActionPerformance()
	{
		if (touchPressEvent != null)
			touchPressEvent ();
	}

	// Get world position of the specified screen position
	Vector3 GetWorldPosition(Vector3 position)
	{
		return Camera.main.ScreenToWorldPoint(position);
	}

	public static BaseAction GetJellyAction()
	{
		var scale1 = ScaleAction.ScaleTo (new Vector3(1.1f, 0.9f, 1), 0.1f);
		var scale2 = ScaleAction.ScaleTo (new Vector3(0.9f, 1.1f, 1), 0.1f);
		var scale3 = ScaleAction.ScaleTo (new Vector3(1f, 1f, 1), 0.1f);
		var sequence = SequenceAction.Create(scale1, scale2, scale3);

		return sequence;
	}

	void OnDestroy()
	{
		TouchManager.SafeRemoveEventListener(this);
	}

}


