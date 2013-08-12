// Native color helper
// SpriterNativeColorHelper.cs
// Spriter Data API - Unity
//  
// Authors:
//       Josh Montoute <josh@thinksquirrel.com>
//       Justin Whitfort <cptdefault@gmail.com>
//
// 
// Copyright (c) 2012 Thinksquirrel Software, LLC
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation files (the "Software"), to deal in the Software 
// without restriction, including without limitation the rights to use, copy, modify, 
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
// persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
// NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// 2dToolkit is (c) by Unikron Software Ltd. Spriter is (c) by BrashMonkey.
//

using UnityEngine;

[ExecuteInEditMode]
public class Spriter2DToolkitColorHelper : MonoBehaviour
{
	private SpriterColorHelper _helper;
	private tk2dSprite _tk2DSprite;
	public Vector2 PaddingTL;
	public Vector2 PaddingBR;
	public Vector2 Size;
	public Rect UV;


	protected void Awake()
	{
		_tk2DSprite = GetComponent<tk2dSprite>();
		_helper = GetComponent<SpriterColorHelper>();
	}

	protected void LateUpdate()
	{
		if (_tk2DSprite == null || _helper == null)
			return;
		
		Vector3 localPosition = transform.localPosition;
		localPosition.z = -_helper.depth;
		transform.localPosition = localPosition;

		_tk2DSprite.color = _helper.color;
	}
}