using UnityEngine;

public class AnimalIdleBehaviour : StateMachineBehaviour
{
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		Animal animal = animator.transform.parent.GetComponent<Animal>();

		if (animal != null)
		{
			animal.OnEnterIdle();
		}
	}
}
