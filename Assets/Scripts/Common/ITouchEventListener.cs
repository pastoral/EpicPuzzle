using UnityEngine;

public interface ITouchEventListener
{
	bool OnTouchPressed(Vector3 position);

	bool OnTouchMoved(Vector3 position);

	void OnTouchReleased(Vector3 position);
}
