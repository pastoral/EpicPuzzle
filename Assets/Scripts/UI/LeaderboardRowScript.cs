using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRowScript : MonoBehaviour
{
	/// <summary>
	/// The avatar.
	/// </summary>
	public Image avatar;

	/// <summary>
	/// The name.
	/// </summary>
	public Text name;

	/// <summary>
	/// The score.
	/// </summary>
	public Text score;

	public void Construct(string id, string name, int score)
	{
		// Set name
		this.name.text = name;

		// Set score
		this.score.text = score.ToString();

		// Get avatar
		FBManager.GetAvatar(id, (sprite) => {
			// Set avatar
			if (avatar != null && sprite != null)
			{
				avatar.sprite = sprite;
			}
		});
	}
}
