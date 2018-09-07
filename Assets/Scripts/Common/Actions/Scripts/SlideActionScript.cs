using UnityEngine;

public class SlideActionScript : MonoBehaviour
{
	/// <summary>
	/// The movement speed (units per second).
	/// </summary>
	public Vector3 speed = new Vector3(1.0f, 0.0f, 0.0f);

	/// <summary>
	/// Local or world.
	/// </summary>
	public bool isLocal = false;

	void Start()
	{
		gameObject.Play(SlideAction.Create(speed, isLocal));
		
		// Self-destroy
		Destroy(this);
	}
}
