using UnityEngine;

public class HammerScript : MonoBehaviour
{
	/// <summary>
	/// The effect prefab.
	/// </summary>
	public GameObject effectPrefab;

	// The foothold
	private Foothold _foothold;

	public Foothold Foothold
	{
		get { return _foothold; }
		set { _foothold = value; }
	}

	public void SetHammerPosition(Vector3 position)
	{
		position.x += 0.83f;
		position.y += 0.73f;

		transform.parent.localPosition = position;
	}
	
	public void OnDestroyFoothold()
	{
		// Play sound
		SoundManager.Instance.PlaySound(SoundID.Explose);

		if (_foothold != null)
		{
			if (effectPrefab != null)
			{
				GameObject effect = Instantiate(effectPrefab);
				effect.transform.SetParent(_foothold.transform.parent);
				effect.transform.localScale = Vector3.one;
				effect.transform.position = _foothold.transform.position;
			}

			_foothold.OnHammerEnd();
		}
	}

	public void OnFinished()
	{
		GameObject.Destroy(transform.parent.gameObject);
	}
}
