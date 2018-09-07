using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SCrossResourse
{
	private static readonly string resourceFile = "resourceCross.data";


	private static SCrossResourse _instance;
	
	public static SCrossResourse Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SCrossResourse();

				if (!SCrossHelper.Load<SCrossResourse>(resourceFile, ref _instance))
				{
					_instance.Reset();
				}
			}
			
			return _instance;
		}
	}

	private int _crossRatio;
	public int CrossRatio {
		get { return _crossRatio; }
		set { _crossRatio = value; }
	}

	private List<Game> _crossData;
	public List<Game> CrossData {
		get { return _crossData; }
		set { _crossData = value; }
	}

    public void Reset()
	{
		_crossData = new List<Game> ();

		_crossRatio = 0;
	}
    
    public bool Save()
	{
		return SCrossHelper.Save<SCrossResourse>(this, resourceFile);
	}
}
