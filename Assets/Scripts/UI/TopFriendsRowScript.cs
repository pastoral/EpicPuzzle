using UnityEngine;
using UnityEngine.UI;

public class TopFriendsRowScript : MonoBehaviour
{
	/// <summary>
	/// The avatar.
	/// </summary>
	public Image avatar;

	/// <summary>
	/// The name.
	/// </summary>
	public Text name;

	public void Construct(string id, string name)
	{
		// Set name
		this.name.text = name;

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
