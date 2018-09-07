using UnityEngine;
using UnityEngine.EventSystems;

public class DialogBehaviour : MonoBehaviour, IPointerClickHandler
{
	void OnEnable()
	{
		KeyManager.AddBackEventHandler(Close);
	}

	void OnDisable()
	{
		KeyManager.RemoveBackEventHandler(Close);
	}

	public virtual void Close()
	{
		
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Close();
	}
}
