using UnityEngine;

[System.Serializable]
public class Character
{
	public char c;
	public Sprite sprite;
}

public class Atlas : ScriptableObject
{
	/// <summary>
	/// The characters.
	/// </summary>
	[SerializeField]
	private Character[] _characters;

	public Sprite Get(char c)
	{
		int count = _characters.Length;

		for (int i = 0; i < count; i++)
		{
			if (_characters[i].c == c)
			{
				return _characters[i].sprite;
			}
		}

		return null;
	}
}
