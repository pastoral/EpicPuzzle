using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PopupBehaviour : MonoBehaviour, IPointerClickHandler
{
	/// <summary>
	/// The close event.
	/// </summary>
	public UnityEvent closeEvent;

	void OnEnable()
	{
		KeyManager.AddBackEventHandler(Close);
	}

	void OnDisable()
	{
		KeyManager.RemoveBackEventHandler(Close);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Close();
	}

	public virtual void Close()
	{
		closeEvent.Invoke();
	}
}
