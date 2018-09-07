using UnityEngine;

public class ShakeActionScript : MonoBehaviour
{
	public float minX = -0.1f;
	public float maxX = 0.1f;
	public float varX = 0.01f;
	public float speedX = 0.05f;

	public float minY = -0.1f;
	public float maxY = 0.1f;
	public float varY = 0.01f;
	public float speedY = 0.05f;

	/// <summary>
	/// Local or world.
	/// </summary>
	public bool isLocal = false;

	void Start()
	{
		gameObject.Play(ShakeAction.Create(minX, maxX, varX, speedX, minY, maxY, varY, speedY, isLocal));
		
		// Self-destroy
		Destroy(this);
	}
}
