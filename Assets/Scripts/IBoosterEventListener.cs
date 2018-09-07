using UnityEngine;

public interface IBoosterEventListener
{
	void OnBoosterPressed(Booster booster);

	void OnBoosterBuyMore(Booster booster);

	void OnBoosterReleased(Booster booster);

	void OnBoosterTouchBegan(Booster booster, Vector3 position);

	void OnBoosterTouchMoved(Vector3 position);

	void OnBoosterTouchEnded(Booster booster, Vector3 position);
}

public class NullBoosterEventListener : IBoosterEventListener
{
	private static NullBoosterEventListener _instance = new NullBoosterEventListener();

	public static NullBoosterEventListener Instance
	{
		get
		{
			return _instance;
		}
	}

	public void OnBoosterPressed(Booster booster)
	{

	}

	public void OnBoosterBuyMore(Booster booster)
	{

	}
	
	public void OnBoosterReleased(Booster booster)
	{
		
	}

	public void OnBoosterTouchBegan(Booster booster, Vector3 position)
	{
		
	}
	
	public void OnBoosterTouchMoved(Vector3 position)
	{
		
	}

	public void OnBoosterTouchEnded(Booster booster, Vector3 position)
	{

	}
}
