using UnityEngine;

public class DigitAtlas : ScriptableObject
{
	/// <summary>
	/// The digits.
	/// </summary>
	[SerializeField]
	private Sprite[] digits = new Sprite[10];

	public Sprite GetSprite(int digit)
	{
		return digits[digit];
	}
}
