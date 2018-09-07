using UnityEngine;

public interface IBoardEventListener
{
	void OnCellPressed(int row, int column);

	void OnCellReleased(int row, int column);

	void OnCellCancelled();

	void OnDraggedLeft();
	
	void OnDraggedRight();
	
	void OnDraggedUp();
	
	void OnDraggedDown();
}

public class NullBoardEventListener : IBoardEventListener
{
	private static NullBoardEventListener _instance = new NullBoardEventListener();

	public static NullBoardEventListener Instance
	{
		get
		{
			return _instance;
		}
	}

	public void OnCellPressed(int row, int column)
	{
		//Log.Debug("OnCellPressed({0}, {1})", row, column);
	}

	public void OnCellReleased(int row, int column)
	{
		//Log.Debug("OnCellReleased({0}, {1})", row, column);
	}
	
	public void OnCellCancelled()
	{
		//Log.Debug("OnCellCancelled");
	}

	public void OnDraggedLeft()
	{
		//Log.Debug("OnDraggedLeft");
	}
	
	public void OnDraggedRight()
	{
		//Log.Debug("OnDraggedRight");
	}
	
	public void OnDraggedUp()
	{
		//Log.Debug("OnDraggedUp");
	}
	
	public void OnDraggedDown()
	{
		//Log.Debug("OnDraggedDown");
	}
}
