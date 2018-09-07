using UnityEngine;
using System;

/// <summary>
/// This is the base class for all actions.
/// </summary>
public class BaseAction
{
	public virtual void Play(GameObject target)
	{

	}

	public virtual void Reset()
	{

	}

	public virtual void Replay(GameObject target, bool reset = true)
	{
		if (reset)
		{
			Reset();
		}
		
		Play(target);
	}

	public virtual void Stop(bool forceEnd = false)
	{

	}

	// Check if action finished
	public virtual bool IsFinished()
	{
		return true;
	}
	
	// Update action, return true if action finished
	public virtual bool Update(float deltaTime)
	{
		return true;
	}
}

public static class ActionHelper
{
	public static ActionScript Play(this GameObject go, BaseAction action, Action callback = null, bool isSelfDestroy = true)
	{
		// Add action script
		ActionScript actionScript = go.AddComponent<ActionScript>();

		// Play action
		actionScript.Play(action, callback, isSelfDestroy);

		return actionScript;
	}

	public static void ReplayAction(this GameObject go, bool reset = true)
	{
		ActionScript actionScript = go.GetComponent<ActionScript>();
		
		if (actionScript != null)
		{
			actionScript.Replay(reset);
		}
	}

	public static void ReplayAction(this GameObject go, string name, bool reset = true)
	{
		ActionScript[] actionScripts = go.GetComponents<ActionScript>();

		for (int i = 0; i < actionScripts.Length; i++)
		{
			ActionScript actionScript = actionScripts[i];

			if (actionScript.name == name)
			{
				actionScript.Replay(reset);
				break;
			}
		}
	}

	public static void StopAction(this GameObject go, bool forceEnd = false)
	{
		ActionScript actionScript = go.GetComponent<ActionScript>();
		
		if (actionScript != null)
		{
			actionScript.Stop(forceEnd);
		}
	}

	public static void StopAction(this GameObject go, string name, bool forceEnd = false)
	{
		ActionScript[] actionScripts = go.GetComponents<ActionScript>();

		for (int i = 0; i < actionScripts.Length; i++)
		{
			ActionScript actionScript = actionScripts[i];

			if (actionScript.name == name)
			{
				actionScript.Stop(forceEnd);
				break;
			}
		}
	}

	public static void StopAllActions(this GameObject go, bool forceEnd = false)
	{
		ActionScript[] actionScripts = go.GetComponents<ActionScript>();

		foreach (ActionScript actionScript in actionScripts)
		{
			actionScript.Stop(forceEnd);
		}
	}
	
	public static void PauseAction(this GameObject go)
	{
		ActionScript actionScript = go.GetComponent<ActionScript>();
		
		if (actionScript != null)
		{
			actionScript.Paused = true;
		}
	}

	public static void PauseAction(this GameObject go, string name)
	{
		ActionScript[] actionScripts = go.GetComponents<ActionScript>();

		for (int i = 0; i < actionScripts.Length; i++)
		{
			ActionScript actionScript = actionScripts[i];

			if (actionScript.name == name)
			{
				actionScript.Paused = true;
				break;
			}
		}
	}

	public static void ResumeAction(this GameObject go)
	{
		ActionScript actionScript = go.GetComponent<ActionScript>();
		
		if (actionScript != null)
		{
			actionScript.Paused = false;
		}
	}

	public static void ResumeAction(this GameObject go, string name)
	{
		ActionScript[] actionScripts = go.GetComponents<ActionScript>();

		for (int i = 0; i < actionScripts.Length; i++)
		{
			ActionScript actionScript = actionScripts[i];

			if (actionScript.name == name)
			{
				actionScript.Paused = false;
				break;
			}
		}
	}
}
