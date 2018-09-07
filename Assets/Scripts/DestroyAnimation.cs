using UnityEngine;

public class DestroyAnimation : StateMachineBehaviour
{
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Transform parent = animator.transform.parent;

		if (parent != null && parent.childCount == 1)
		{
			GameObject.Destroy(parent.gameObject);
		}
		else
		{
			GameObject.Destroy(animator.gameObject);
		}
	}
}
