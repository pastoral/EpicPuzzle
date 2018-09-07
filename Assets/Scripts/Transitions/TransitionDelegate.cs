using UnityEngine;
using System.Collections;

public interface TransitionDelegate
{
	/// <summary>
	/// Gets the shader.
	/// </summary>
	/// <returns>The shader.</returns>
	Shader GetShader();

	/// <summary>
	/// Gets the mesh.
	/// </summary>
	/// <returns>The mesh.</returns>
	Mesh GetMesh();

	/// <summary>
	/// Gets the texture.
	/// </summary>
	/// <returns>The texture.</returns>
	Texture2D GetTexture();

	/// <summary>
	/// Called when the transition is start.
	/// </summary>
	IEnumerator Play();
}
