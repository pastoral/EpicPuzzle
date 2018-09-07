using UnityEngine;
using System.Collections;

public class MapSetting : ScriptableObject
{
	/// <summary>
	/// The foothold prefab.
	/// </summary>
	public GameObject footholdPrefab;

	/// <summary>
	/// The double foothold prefab.
	/// </summary>
	public GameObject doubleFootholdPrefab;

	/// <summary>
	/// The redirect foothold prefab.
	/// </summary>
	public GameObject redirectFootholdPrefab;

	/// <summary>
	/// The animal prefab.
	/// </summary>
	public GameObject animalPrefab;

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

		//TODO
//		if (type == FootholdType.Redirect)
//		{
//			return redirectFootholdPrefab;
//		}
		
		return null;
	}
	
	public GameObject GetAnimalPrefab()
	{
		return animalPrefab;
	}
}
