using UnityEngine;

public class TimelineFloatHelper : TimelineHelper<float>
{
	public TimelineFloatHelper()
	{
		
	}
	
	public TimelineFloatHelper(float[] times, float[] values)
	{
		Construct(times, values, true);
	}

	protected override void Lerp(float start, float end, float t)
	{
		_value = start + (end - start) * t;
	}
}
