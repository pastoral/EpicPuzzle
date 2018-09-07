using UnityEngine;

public class HideAction : BaseAction
{
	public static HideAction Create()
	{
		return new HideAction();
	}

	public override void Play(GameObject target)
	{
		target.Hide();
	}
}
