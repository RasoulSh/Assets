using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//using UnityEngine.SceneManagement;


public class SeasonManager : MonoBehaviour 
{
//	public GUISkin Skin;
//	public Texture2D Background;
//	public Texture2D ReturnIcon;
	public GameObject SeasonButton;
	public GameObject FirstPosition;
	public GameObject SeasonMenu;
	private List<Season> Seasons;
	private GameData Data;
//	private float ScaleSize;
//	private int CenterX;
//	private int CenterY;
	private float XFactor = 0;
	private float YFactor = 0;
	private bool XRightLeft = true;
	private bool YUpDown = false;
	private int CurrentSeason;
	private GameObject MenuManager;

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
		Data = data;
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
		MenuManager = GameObject.FindGameObjectWithTag("MenuManager");
		Seasons = Data.Seasons;
		//		ScaleSize = 2048f / (float)Screen.currentResolution.width;
		//		CenterX = Screen.currentResolution.width / 2;
		//		CenterY = Screen.currentResolution.height / 2;
		//StartCoroutine (SetFactors ());
		
		bool IsAllofSeasonsCompleted = true;
		int i = 0;
		foreach(Season s in Seasons)
		{
			if (!s.SeasonCompleted)
			{
				IsAllofSeasonsCompleted = false;
				CurrentSeason = i;
				break;
			}
			i++;
		}
		if (IsAllofSeasonsCompleted)
		{
			CurrentSeason = Seasons.Count - 1;
		}
		
		float w = FirstPosition.GetComponent<RectTransform> ().rect.width * 0.7f;
		float h = FirstPosition.GetComponent<RectTransform> ().rect.height * 0.7f;
		float x = FirstPosition.GetComponent<RectTransform> ().transform.position.x;
		float y = FirstPosition.GetComponent<RectTransform> ().transform.position.y;
		for (int j = 0; j < Seasons.Count;j++) 
		{
			GameObject SeasonIcon = (GameObject)Instantiate(SeasonButton,new Vector3(x,y,0),Quaternion.Euler(Vector3.zero));
			SeasonIcon.transform.parent = SeasonMenu.transform;
			//SeasonIcon.GetComponent<transform>();
			Sprite mysprite = new Sprite();
			SetTextureToSprite(ref mysprite,Data.SeasonIcons[j].Icon);
			
			SeasonIcon.GetComponent<Image>().sprite = mysprite;
			SeasonIcon.GetComponent<Button>().onClick.AddListener(() => LoadSeason(Seasons[j],j + 1));
			if ((j + 1) % 4 == 0)
			{
				x = FirstPosition.GetComponent<RectTransform> ().transform.position.x;
				y -= (FirstPosition.GetComponent<RectTransform> ().rect.height * 1.1f);
			}
			else
			{
				x += (FirstPosition.GetComponent<RectTransform> ().rect.width * 1.15f);
			}
		}
		gameObject.SetActive (false);
	}

//	void OnGUI()
//	{
//		GUI.skin = Skin;
//		if (GUI.RepeatButton (new Rect (0, 0, Screen.currentResolution.width / 8, Screen.currentResolution.width / 8), ReturnIcon)) 
//		{
//				Application.LoadLevel ("MainMenu");
//		}
//		float BGSize = 2048 / ScaleSize;
//		float w = BGSize;
//		float h = 1024 / ScaleSize;
//		float SourceX = CenterX - (w / 2);
//		float SourceY = CenterY - (h / 2);
//		float x = SourceX;
//		float y = SourceY;
//		GUI.DrawTexture (new Rect (x, y, w, h), Background);
//		float ButtonSize = Screen.currentResolution.width / 8;
//		float CurrrentXPosition = SourceX + (ButtonSize * 1.2f);
//		float CurrrentYPosition = SourceY + (ButtonSize / 1.1f);
//		for (int i = 0; i < Seasons.Count;i++) 
//		{
//			float newX = CurrrentXPosition;
//			float newY = CurrrentYPosition;
//			if (i == CurrentSeason)
//			{
//				newX += XFactor;
//				newY += YFactor;
//			}
//			if (GUI.RepeatButton (new Rect (newX, newY, ButtonSize, ButtonSize), Data.SeasonIcons[i].Icon )) 
//				{
//						LoadSeason (Seasons[i],i + 1);
//
//				}	
//				if ((i + 1) % 5 == 0)
//				{
//				CurrrentXPosition = SourceX + (ButtonSize * 1.2f);
//					CurrrentYPosition += (ButtonSize * 1.1f);
//				}
//				else
//				{
//					CurrrentXPosition += (ButtonSize * 1.15f);
//				}
//		}
//	}

//	IEnumerator SetFactors()
//	{
//		while(true)
//		{
//			yield return new WaitForSeconds(0.033f);
//			if (XFactor >= Screen.width / 256)
//			{
//				XRightLeft = false;
//			}
//			else if (XFactor <= 0)
//			{
//				XRightLeft = true;
//			}
//
//			if (XFactor >= Screen.width / 512 && YFactor > 0)
//			{
//				YUpDown = true;
//			}
//			else if (XFactor < Screen.width / 512 && YFactor < 0)
//			{
//				YUpDown = false;
//			}
//
//			if (XRightLeft)
//			{
//				XFactor += 1f;
//			}
//			else
//			{
//				XFactor -= 1f;
//			}
//			if (YUpDown)
//			{
//				YFactor -= 0.5f;
//			}
//			else
//			{
//				YFactor += 0.5f;
//			}
//		}
//	}

	private void SetTextureToSprite(ref Sprite sp, Texture2D tex)
	{
		sp = Sprite.Create (tex, new Rect (0, 0, tex.width, tex.height), new Vector2 (0.5f, 0.5f));
//		sp.texture.width = tex.width;
//		sp.texture.height = tex.height;
//		for (int i = 1; i <= tex.width;i++)
//		{
//			for (int j = 1; j <= tex.height;j++)
//			{
//				sp.texture.SetPixel(i,j,tex.GetPixel(i,j));
//			}
//		}
//		sp.texture.Apply ();
	}

	public void LoadSeason(Season s, int SeasonNumber)
	{
		Data.SelectedSeason = s;
		Data.SeasonNo = SeasonNumber;
		//levelmanager.UpdateSeasonLevels();
		MenuManager.GetComponent<MenuManager>().GoToMenu(GameObject.FindGameObjectWithTag("LevelMenu"));
	}
}
