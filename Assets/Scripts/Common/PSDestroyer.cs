using UnityEngine;
using System.Collections;

public class PSDestroyer : MonoBehaviour
{
	public bool onlyDeactivate;
	
	void OnEnable()
	{
		StartCoroutine(CheckAlive());
	}
	
	IEnumerator CheckAlive()
	{
		ParticleSystem ps = GetComponent<ParticleSystem>();
		
		while (ps != null)
		{
			yield return new WaitForSeconds(0.5f);

			if (!ps.IsAlive(true))
			{
				if (onlyDeactivate)
				{
					gameObject.SetActive(false);
				}
				else
				{
					Destroy(gameObject);
				}

				break;
			}
		}
	}
}
