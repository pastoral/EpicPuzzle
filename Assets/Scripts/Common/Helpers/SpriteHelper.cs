using UnityEngine;

public static class SpriteHelper
{
	public static float GetWidth(this Sprite sprite)
	{
		return sprite.bounds.size.x;
	}

	public static float GetHeight(this Sprite sprite)
	{
		return sprite.bounds.size.y;
	}

	public static float GetWidthInPixels(this Sprite sprite)
	{
		return sprite.bounds.size.x * sprite.pixelsPerUnit;
	}
	
	public static float GetHeightInPixels(this Sprite sprite)
	{
		return sprite.bounds.size.y * sprite.pixelsPerUnit;
	}
	
	public static Vector2[] GetUVs(this Sprite sprite)
	{
		Vector2[] uvs = new Vector2[4];
		
		Texture texture = sprite.texture;
		
		if (texture != null)
		{
			Rect rect = sprite.textureRect;
			Vector2 rectOffset = sprite.textureRectOffset;
			
			float left   = (rect.x + rectOffset.x) / texture.width;
			float bottom = (rect.y + rectOffset.y) / texture.height;
			float right  = left   + rect.width  / texture.width;
			float top    = bottom + rect.height / texture.height;
			
			uvs[0] = new Vector2(left, bottom);
			uvs[1] = new Vector2(left, top);
			uvs[2] = new Vector2(right, bottom);
			uvs[3] = new Vector2(right, top);
		}
		
		return uvs;
	}
	
	public static void GetUVs(this Sprite sprite, Vector2[] uvs)
	{
		Texture texture = sprite.texture;
		
		if (texture != null)
		{
			Rect rect = sprite.textureRect;
			Vector2 rectOffset = sprite.textureRectOffset;
			
			float left   = (rect.x + rectOffset.x) / texture.width;
			float bottom = (rect.y + rectOffset.y) / texture.height;
			float right  = left   + rect.width  / texture.width;
			float top    = bottom + rect.height / texture.height;
			
			uvs[0].x = left;
			uvs[0].y = bottom;
			uvs[1].x = left;
			uvs[1].y = top;
			uvs[2].x = right;
			uvs[2].y = bottom;
			uvs[3].x = right;
			uvs[3].y = top;
		}
	}
	
	public static bool GetUVs(this Sprite sprite, ref float uvLeft, ref float uvTop, ref float uvRight, ref float uvBottom)
	{
		Texture texture = sprite.texture;
		
		if (texture != null)
		{
			Rect rect = sprite.textureRect;
			Vector2 rectOffset = sprite.textureRectOffset;
			
			uvLeft   = (rect.x + rectOffset.x) / texture.width;
			uvRight  = uvLeft + rect.width  / texture.width;
			uvBottom = (rect.y + rectOffset.y) / texture.height;
			uvTop    = uvBottom + rect.height / texture.height;
			
			return true;
		}
		
		return false;
	}
	
	public static bool GetUVs(this Sprite sprite, ref float uvLeft, ref float uvTop, ref float uvRight, ref float uvBottom,
	                          ref float uvMiddle1, ref float uvMiddle2, ref float uvMiddle3, ref float uvMiddle4)
	{
		Texture texture = sprite.texture;
		
		if (texture != null)
		{
			Rect rect = sprite.textureRect;
			Vector2 rectOffset = sprite.textureRectOffset;
			Vector4 border = sprite.border;
			
			uvLeft   = (rect.x + rectOffset.x) / texture.width;
			uvRight  = uvLeft + rect.width  / texture.width;
			uvBottom = (rect.y + rectOffset.y) / texture.height;
			uvTop    = uvBottom + rect.height / texture.height;
			
			uvMiddle1 = border.y / texture.height;
			uvMiddle2 = 1.0f - border.w / texture.height;
			uvMiddle3 = border.x / texture.width;
			uvMiddle4 = 1.0f - border.z / texture.width;
			
			return true;
		}
		
		return false;
	}
	
	
	public static bool GetUVs(this Sprite sprite, float marginLeft, float marginTop, float marginRight, float marginBottom,
	                          ref float uvLeft, ref float uvTop, ref float uvRight, ref float uvBottom,
	                          ref float uvMiddle1, ref float uvMiddle2, ref float uvMiddle3, ref float uvMiddle4)
	{
		Texture texture = sprite.texture;
		
		if (texture != null)
		{
			Rect rect = sprite.textureRect;
			Vector2 rectOffset = sprite.textureRectOffset;
			Vector4 border = sprite.border;
			
			uvLeft   = (rect.x + rectOffset.x) / texture.width;
			uvRight  = uvLeft + rect.width  / texture.width;
			uvBottom = (rect.y + rectOffset.y) / texture.height;
			uvTop    = uvBottom + rect.height / texture.height;
			
			uvMiddle1 = border.y / texture.height;
			uvMiddle2 = 1.0f - border.w / texture.height;
			uvMiddle3 = border.x / texture.width;
			uvMiddle4 = 1.0f - border.z / texture.width;
			
			float w = uvMiddle4 - uvMiddle3;
			float h = uvMiddle2 - uvMiddle1;
			
			if (marginLeft > 0)
			{
				uvMiddle3 += marginLeft * w;
			}
			
			if (marginRight > 0)
			{
				uvMiddle4 -= marginRight * w;
			}
			
			if (marginBottom > 0)
			{
				uvMiddle1 += marginBottom * h;
			}
			
			if (marginTop > 0)
			{
				uvMiddle2 -= marginTop * h;
			}
			
			return true;
		}
		
		return false;
	}
}
