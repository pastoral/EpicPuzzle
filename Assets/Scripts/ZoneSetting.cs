using UnityEngine;

public class ZoneSetting : ScriptableObject
{
	/// <summary>
	/// The background prefab.
	/// </summary>
	public GameObject backgroundPrefab;

	/// <summary>
	/// The foothold prefab.
	/// </summary>
	public GameObject footholdPrefab;
	
	/// <summary>
	/// The double foothold prefab.
	/// </summary>
	public GameObject doubleFootholdPrefab;
	
	/// <summary>
	/// The time foothold prefab.
	/// </summary>
	public GameObject timeFootholdPrefab;

	/// <summary>
	/// The redirect left foothold prefab.
	/// </summary>
	public GameObject redirectLeftFootholdPrefab;
	
	/// <summary>
	/// The redirect up foothold prefab.
	/// </summary>
	public GameObject redirectUpFootholdPrefab;
	
	/// <summary>
	/// The redirect right foothold prefab.
	/// </summary>
	public GameObject redirectRightFootholdPrefab;
	
	/// <summary>
	/// The redirect down foothold prefab.
	/// </summary>
	public GameObject redirectDownFootholdPrefab;

	/// <summary>
	/// The clothes.
	/// </summary>
	public Sprite clothesLeft;
	public Sprite clothesUp;
	public Sprite clothesDown;

	/// <summary>
	/// The cap.
	/// </summary>
	public Sprite capLeft;
	public Sprite capUp;
	public Sprite capDown;
	
	/// <summary>
	/// The map icon.
	/// </summary>
	public Sprite mapIcon;

	/// <summary>
	/// The foothold icon.
	/// </summary>
	public Sprite footholdIcon;

	/// <summary>
	/// The win cyrus.
	/// </summary>
	public Sprite winCyrus;

	/// <summary>
	/// The lose cyrus.
	/// </summary>
	public Sprite loseCyrus;

	public GameObject GetFootholdPrefab(FootholdType type)
	{
		if (type == FootholdType.Normal)
		{
			return footholdPrefab;
		}
		
		if (type == FootholdType.Double)
		{
			return doubleFootholdPrefab;
		}
		
		if (type == FootholdType.Time)
		{
			return timeFootholdPrefab;
		}
		
		if (type == FootholdType.RedirectLeft)
		{
			return redirectLeftFootholdPrefab;
		}
		
		if (type == FootholdType.RedirectUp)
		{
			return redirectUpFootholdPrefab;
		}
		
		if (type == FootholdType.RedirectRight)
		{
			return redirectRightFootholdPrefab;
		}
		
		if (type == FootholdType.RedirectDown)
		{
			return redirectDownFootholdPrefab;
		}

		return null;
	}
}
