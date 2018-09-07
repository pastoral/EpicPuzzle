using UnityEngine;

public class SpinActionScript : MonoBehaviour
{
	/// <summary>
	/// The rotation speed (degrees per second).
	/// </summary>
	public float speed = 360.0f;

	void Start()
	{
		gameObject.Play(SpinAction.Create(speed));
		
		// Self-destroy
		Destroy(this);
	}
}
