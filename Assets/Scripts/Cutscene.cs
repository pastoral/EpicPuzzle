using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Cutscene : MonoBehaviour, IPointerClickHandler {

	[SerializeField] Text text;
	[SerializeField] Image image;
	[SerializeField] GameObject bird = null;
	[SerializeField] string[] textArray;
	[SerializeField] float letterPause;
	int currentMessageIndex;
	string message;
	[SerializeField]bool textRunning;

	// Use this for initialization
	void Start () {
		currentMessageIndex = -1;
		ShowNextMessage();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator TypeText () {
		foreach (char letter in message.ToCharArray()) {
			text.text += letter;
			//yield return 0;
			yield return new WaitForSeconds (letterPause);

		}

		//Debug.Log("Stop typing");
		textRunning = false;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (textRunning) return;
		ShowNextMessage();
	}

	void ShowNextMessage()
	{
		currentMessageIndex++;
		textRunning = true;

		if (currentMessageIndex >= textArray.Length)
		{
			Exit();
		}
		else
		{
			message = textArray[currentMessageIndex];
			text.text = "";
			StartCoroutine(TypeText ());
			//Debug.Log("Play typing");
		}
	}

	public void Exit()
	{
		image.gameObject.Play(FadeAction.FadeOut(1f));
		text.gameObject.SetActive(false);

		if (bird != null)
			bird.Play(FadeAction.FadeOut(1));

		Invoke("DeactiveCutscene", 1.05f);
	}

	void DeactiveCutscene()
	{
		gameObject.SetActive(false);
	}

}
