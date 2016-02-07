using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;

public class ShopController : MonoBehaviour
{
		public GUISkin Skin;
		public Texture2D Background;
		public Texture2D Icon;
		public Texture2D ReturnIcon;
		public Texture2D ShopBox;
		private float ScaleSize;
		private int CenterX;
		private int CenterY;

		void Start()
		{
			ScaleSize = 2048f / (float)Screen.currentResolution.width;
			CenterX = Screen.currentResolution.width / 2;
			CenterY = Screen.currentResolution.height / 2;
		}

		void OnGUI()
		{
			GUI.skin = Skin;
			if (GUI.RepeatButton (new Rect (0, 0, Screen.currentResolution.width / 8, Screen.currentResolution.width / 8), ReturnIcon)) 
			{
				Application.LoadLevel ("MainMenu");
			}
			float BGSize = 2048 / ScaleSize;
			float w = BGSize;
			float h = 1024 / ScaleSize;
			float SourceX = CenterX - (w / 2);
			float SourceY = CenterY - (h / 2);
			float x = SourceX;
			float y = SourceY;
			GUI.DrawTexture (new Rect (x, y, w, h), Background);
			w = Screen.currentResolution.width / 8;
			h = w;
			x += (BGSize / 2) - (w / 2);
			y -= (BGSize / 64);
			GUI.DrawTexture (new Rect (x, y, w, h), Icon);

			w = Screen.currentResolution.width / 4;
			h = Screen.currentResolution.width / 8;
			x = SourceX + (BGSize / 6);
			y = SourceY + (BGSize / 8);
			GUI.DrawTexture (new Rect (x, y, w, h), ShopBox);
		}
}
