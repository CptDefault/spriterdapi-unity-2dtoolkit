// NGUI import/export plugin
// SpriterDataNGUI.cs
// Spriter Data API - Unity
//  
// Authors:
//       Josh Montoute <josh@thinksquirrel.com>
//       Justin Whitfort <cptdefault@gmail.com>
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

// Some notes:
//
// This class extends SpriterDataUnity to provide hooks specific to 2DToolkit.



using System;
using System.IO;
using System.Linq;
using BrashMonkey.Spriter.Data;
using BrashMonkey.Spriter.Data.ObjectModel;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BrashMonkey.Spriter.DataPlugins.Native
{
	class SpriterData2DToolkit : SpriterDataUnity
	{
		#region Menu items

		// Allow import of a Spriter file
		[MenuItem("Spriter/[2DToolkit]Create new character")]
		public static void CreateAtlas()
		{
			var data = new SpriterData2DToolkit();

			string lastPath = EditorPrefs.GetString("SpriterNGUILastPath");
			if (String.IsNullOrEmpty(lastPath))
				lastPath = Application.dataPath;

			lastPath = EditorUtility.OpenFilePanel("Open Spriter File", lastPath, "scml");
			EditorPrefs.SetString("SpriterNGUILastPath", lastPath);

			if (!String.IsNullOrEmpty(lastPath))
			{
				data.LoadData(lastPath);
			}
		}

		// Convenience item for debugging
		// TODO: Remove before shipping
		[MenuItem("Spriter/[2DToolkit]Reimport Last")]
		public static void CreateLastAtlas()
		{
			var data = new SpriterData2DToolkit();

			if (!String.IsNullOrEmpty(EditorPrefs.GetString("SpriterNGUILastPath")))
			{
				data.LoadData(EditorPrefs.GetString("SpriterNGUILastPath"));
			}
		}

		#endregion

		private tk2dSpriteCollection _collection;

		protected override string GetSaveFolder()
		{
			foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
			{
				string path = AssetDatabase.GetAssetPath(obj);

				if (File.Exists(path))
				{

					path = Path.GetDirectoryName(path);
					return path + "/";
				}
			}
			return "Assets/";
		}

		protected override Rect GetSpriteInfo(SpriterColorHelper helper)
		{
			//2dToolkit takes care of all of this; have SpriterUnity leave the scale/offset alone
			return new Rect(0, 0, 1, 1);
		}

		protected override void AddSprite(ISpriterTimelineObject obj, GameObject go)
		{
			go.AddComponent<Spriter2DToolkitColorHelper>();
			
			var sprite = go.AddComponent<tk2dSprite>();
			sprite.SetSprite(_collection.spriteCollection, obj.targetFile.folderName+"/"+obj.targetFile.name);

			var size = new Vector3(obj.targetFile.width, obj.targetFile.height, 1);//sprite.CurrentSprite.GetUntrimmedBounds().size;
			sprite.scale = new Vector3(20, 20, 1);//size / 10);
		}

		#region Sprite atlas creation
		protected override void CreateSpriteAtlas()
		{
			// Create paths and names based on selection

			_collection = tk2dSpriteCollectionEditor.CreateSpriteCollection(GetSaveFolder(),  entity.name + " collection");

			// Textures
			var texLoad =
				files.Where(file => file.type == FileType.Image || file.type == FileType.Unknown)
					 .Select(file =>
						 {
							 var definition = new tk2dSpriteCollectionDefinition
								 {
									 name = file.folderName + "/" + file.name,
									 texture = (Texture2D) Resources.LoadAssetAtPath(rootLocation + "/" + file.name, typeof (Texture)),
									 anchor = tk2dSpriteCollectionDefinition.Anchor.UpperLeft
								 };
							 definition.anchor = tk2dSpriteCollectionDefinition.Anchor.UpperLeft;
							 return definition;
						 })
					 .ToArray();

			_collection.textureParams = texLoad;

			if (!tk2dSpriteCollectionBuilder.Rebuild(_collection))
				EditorUtility.DisplayDialog("Failed to commit sprite collection", 
				"Please check the console for more details.", "Ok");

		}

		#endregion


	}
}