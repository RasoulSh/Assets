using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour 
{
		public GUISkin Skin;
		public Texture2D Background;
		public Texture2D Logo;
		public Texture2D PlayTexture;
		public Texture2D PlayTouchTexture;
		public Texture2D ShopTexture;
		public Texture2D ShopTouchTexture;
		public Texture2D SettingsTexture;
		public Texture2D SettingsTouchTexture;
		public Texture2D EsprisTexture;
		public Texture2D EsprisTouchTexture;
		public Texture2D MuteTexture;
		public Texture2D MuteTouchTexture;
		public Texture2D UnmuteTexture;
		public Texture2D UnmuteTouchTexture;
		public Texture2D GiftTexture;
		public Texture2D GiftTouchTexture;
		public Texture2D QuitTouchTexture;
		public Texture2D QuitTexture;
		private Texture2D SoundTexture;
		private float ScaleSize;
		private int CenterX;
		private int CenterY;
		private bool Mute;
		private bool MuteButtonPush = false;
		
		void LoadSeasonsFromFile(GameData data)
		{
			Information info = new Information(data.Lives,data.Gems,data.Coins,data.Score,data.Mute);
			SeasonInformation Sinfo = new SeasonInformation(data.Seasons);
			List<StoreIcon> storeicons = new List<StoreIcon> ();
			foreach (SeasonIcon si in data.SeasonIcons)
			{
				storeicons.Add(StoreIcon.ConvertToStoreIcon (si));
			}
			IconsInformation Iconinfo = new IconsInformation (storeicons);
			SaveLoad.Load (ref info);
			SaveLoad.LoadSeasons (ref Sinfo);
			SaveLoad.LoadIcons (ref Iconinfo);
			data.Seasons = Sinfo.Seasons;
			List<SeasonIcon> seasonicons = new List<SeasonIcon> ();
			foreach (StoreIcon sti in Iconinfo.StoreIcons)
			{
				seasonicons.Add(StoreIcon.ConvertToSeasonIcon (sti));
			}
			data.SeasonIcons = seasonicons;
			data.Lives = info.Lives;
			data.Gems = info.Gems;
			data.Coins = info.Coins;
			data.Score = info.Score;
			data.Mute = info.Mute;
			
		}
		
		void Start()
		{
			GameData data = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
			if (!data.InformationsLoaded)
			{
				LoadSeasonsFromFile(data);
				foreach (Season s in data.Seasons) 
				{
					if (s.Levels.Length > 0)
					{
						if (s.LevelCompleted.Length < 1 )
						{
							s.LevelCompleted = new bool[s.Levels.Length];
						}
						if (s.LevelStarsArrived.Length < 1)
						{
							s.LevelStarsArrived = new int[s.Levels.Length];
						}
					}
				}
				data.InformationsLoaded = true;
			}
			ScaleSize = 2048f / (float)Screen.currentResolution.width;
			CenterX = Screen.currentResolution.width / 2;
			CenterY = Screen.currentResolution.height / 2;
			Mute = data.Mute;
			if (Mute) 
			{
					SoundTexture = UnmuteTexture;
			}
			else
			{
					SoundTexture = MuteTexture;
			}
		}
		void OnGUI()
		{
			GUI.skin = Skin;
			float BGSize = 2048 / ScaleSize;
			float w = BGSize;
			float h = BGSize;
			float SourceX = CenterX - (w / 2);
			float SourceY = CenterY - (h / 2);
			float x = SourceX;
			float y = SourceY;
			GUI.DrawTexture (new Rect(x,y,w,h),Background);
			w = Logo.width / ScaleSize;
			h = Logo.height / ScaleSize;
			x = (510 / ScaleSize) + SourceX;
			y = (300 / ScaleSize) + SourceY;
			GUI.DrawTexture (new Rect(x,y,w,h),Logo);
			w = PlayTexture.width / ScaleSize;
			h = PlayTexture.height / ScaleSize;
			x = (817 / ScaleSize) + SourceX;
			y = (758 / ScaleSize) + SourceY;
			if (GUI.RepeatButton (new Rect (x, y, w, h), PlayTexture)) 
			{
					StartCoroutine (PlayGame ());
			}
			w = ShopTexture.width / ScaleSize;
			h = ShopTexture.height / ScaleSize;
			x = (1042 / ScaleSize) + SourceX;
			y = (918 / ScaleSize) + SourceY;
			if (GUI.RepeatButton (new Rect (x, y, w, h), ShopTexture)) 
			{
					StartCoroutine (Shop ());
			}
			w = EsprisTexture.width / ScaleSize;
			h = EsprisTexture.height / ScaleSize;
			x = (1334 / ScaleSize) + SourceX;
			y = (920 / ScaleSize) + SourceY;
			if (GUI.RepeatButton (new Rect (x, y, w, h), EsprisTexture)) 
			{
					StartCoroutine (Espris ());
			}
//				w = SettingsTexture.width / ScaleSize;
//				h = SettingsTexture.height / ScaleSize;
//				x = (1530 / ScaleSize) + SourceX;
//				y = (1167 / ScaleSize) + SourceY;
//				if (GUI.RepeatButton (new Rect (x, y, w, h) , SettingsTexture)) 
//				{
//						StartCoroutine (Settings ());
//				}
			w = MuteTexture.width / ScaleSize;
			h = MuteTexture.height / ScaleSize;
			x = (562 / ScaleSize) + SourceX;
			y = (1210 / ScaleSize) + SourceY;
			Rect MuteRect = new Rect (x, y, w, h);
			GUI.RepeatButton (MuteRect, SoundTexture);
			if (Application.platform == RuntimePlatform.Android)
			{
				if (Input.touchCount > 0)
				{
					foreach (Touch t in Input.touches)
					{
						Vector2 vec = t.position;
						vec.y = Screen.height - vec.y;
						if (MuteRect.Contains(vec))
						{
							if (!MuteButtonPush)
							{
								MuteButtonPush = true;
								StartCoroutine (MuteSwitch ());	
							}
						}
						else
						{
							MuteButtonPush = false;
						}
					}
				}
				else
				{
					MuteButtonPush = false;	
				}
			}
			else if (Application.isEditor)
			{
				if (Input.GetMouseButton (0)) 
				{
					Vector2 vec = Input.mousePosition;
					vec.y = Screen.height - vec.y;
					if (MuteRect.Contains(vec))
					{
						if (!MuteButtonPush)
						{
							MuteButtonPush = true;
							StartCoroutine (MuteSwitch ());	
						}
					}
					else
					{
						MuteButtonPush = false;
					}
				}
				else
				{
					MuteButtonPush = false;
				}
			}
			w = GiftTexture.width / ScaleSize;
			h = GiftTexture.height / ScaleSize;
			x = (900 / ScaleSize) + SourceX;
			y = (1250 / ScaleSize) + SourceY;
			if (GUI.RepeatButton (new Rect (x, y, w, h), GiftTexture)) 
			{
					StartCoroutine (Gift ());
			}
			w = QuitTexture.width / ScaleSize;
			h = QuitTexture.height / ScaleSize;
			x = (1296 / ScaleSize) + SourceX;
			y = (1191 / ScaleSize) + SourceY;
			if (GUI.RepeatButton (new Rect (x, y, w, h), QuitTexture)) 
			{
				GameData data = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
				Information info = new Information(data.Lives,data.Gems,data.Coins,data.Score,data.Mute);
				SaveLoad.Save(info);
				StartCoroutine (Quit ());
			}
			GUI.skin = null;
			if (GUI.Button(new Rect (0,0,Screen.width / 16, Screen.width / 32),"Rate Game"))
			{
				Application.OpenURL ("market://details?id=com.glimgames.motori");
			}
			if (GUI.Button(new Rect (0,Screen.height / 8,Screen.width / 16, Screen.width / 32),"Invite Friends"))
			{
				Sharing.ShareText("Hi, I'm just playing that beautifull game and inviting you to try Boxes Game: " + "cafebazaar.ir/app/com.glimgames.motori");
			}
		}

		IEnumerator PlayGame()
		{
				PlayTexture = PlayTouchTexture;
				yield return new WaitForSeconds(0.2f);
				Application.LoadLevel("SeasonManagement");
		}

		IEnumerator Shop()
		{
				ShopTexture = ShopTouchTexture;
				yield return new WaitForSeconds(0.2f);
				Application.LoadLevel("Shop");
		}

		IEnumerator Espris()
		{
				Texture2D temp = EsprisTexture;
				EsprisTexture = EsprisTouchTexture;
				yield return new WaitForSeconds(0.2f);;
				EsprisTexture = temp;
		}

		IEnumerator Settings()
		{
				Texture2D temp = SettingsTexture;
				SettingsTexture = SettingsTouchTexture;
				yield return new WaitForSeconds(0.2f);;
				SettingsTexture = temp;
				Application.LoadLevel("Settings");
		}

		IEnumerator MuteSwitch()
		{
			if (MuteButtonPush) 
			{
				Texture2D temp;
				if (Mute) 
				{
					temp = MuteTexture;
					SoundTexture = UnmuteTouchTexture;
				} 
				else
				{
					temp = UnmuteTexture;
					SoundTexture = MuteTouchTexture;
				}
				yield return new WaitForSeconds(0.2f);;
				SoundTexture = temp;
				Mute = !Mute;
				GameData data = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
				data.Mute = !data.Mute;
				GetComponent<AudioListener> ().enabled = !Mute;
			}
		}

		IEnumerator Gift()
		{
				Texture2D temp = GiftTexture;
				GiftTexture = GiftTouchTexture;
				yield return new WaitForSeconds(0.2f);;
				GiftTexture = temp;
				Application.LoadLevel("Gift");
		}

		IEnumerator Quit()
		{
				Texture2D temp = QuitTexture;
				QuitTexture = QuitTouchTexture;
				yield return new WaitForSeconds(0.2f);;
				QuitTexture = temp;
				Application.Quit ();
		}
			
}
