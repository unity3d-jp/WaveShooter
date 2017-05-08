/* -*- mode:CSharp; coding:utf-8-with-signature -*-
 *
 * AtlasExporterWindow.cs - Project AnotherThread2
 * Sat Feb  4 23:33:53 2017
 *
 * Copyright (c) 2017 Yuji YASUHARA
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using UnityEditor;
using UnityEngine;

public class AtlasExporterWindow : EditorWindow
{
	private static Sprite sprite_;
	private static RenderTexture render_texture_;
	private static Material material_;
	private Texture2D texture_;
    
    [MenuItem("Window/AtlasExporter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AtlasExporterWindow));
    }
    
    void OnGUI()
    {
		GUILayout.Label("This is a tool for create texture atlas.");
		if (GUILayout.Button("convert atlas")) {
			Debug.Log("converting..");

			render_texture_ = AssetDatabase.LoadAssetAtPath("Assets/Textures/blit_render_texture.renderTexture", typeof(RenderTexture)) as RenderTexture;
			material_ = AssetDatabase.LoadAssetAtPath("Assets/Materials/blit.mat", typeof(Material)) as Material;
			sprite_ = AssetDatabase.LoadAssetAtPath("Assets/Textures/GUI/square.png", typeof(Sprite)) as Sprite;

			Texture2D tex2d = UnityEditor.Sprites.SpriteUtility.GetSpriteTexture(sprite_, true /* getAtlasData */);
			Graphics.SetRenderTarget(render_texture_);
			GL.Clear(true, true, new Color(0, 0, 0, 0));
			Graphics.Blit(tex2d, render_texture_, material_, 0 /* pass */);

			RenderTexture.active = render_texture_;
			int width = render_texture_.width;
			int height = render_texture_.height;
			texture_ = new Texture2D(width, width, TextureFormat.ARGB32, false /* mipmap */);
			texture_.ReadPixels( new Rect(0, 0, width, height), 0, 0);
			texture_.Apply();
			RenderTexture.active = null; //can help avoid errors 
     
			var atlas_path = "/Textures/GUI/atlas.png";
			byte[] bytes;
			bytes = texture_.EncodeToPNG();
			var path = Application.dataPath + atlas_path;
			System.IO.File.WriteAllBytes(path, bytes);

			var rel_path = "Assets" + atlas_path;
			AssetDatabase.ImportAsset(rel_path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ImportRecursive);
			Debug.Log("done!");
		}

		if (texture_ != null) {
			Graphics.DrawTexture(new Rect(10, 240, 100, 100), texture_);
		}
    }
}

/*
 * End of AtlasExporterWindow.cs
 */
