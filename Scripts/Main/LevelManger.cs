using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;

public class LevelManger : MonoBehaviour 
{
	public GUISkin Skin;
	public Texture2D Background;
	public Texture2D ReturnIcon;
	public Texture2D LevelIcon;
	public Texture2D StarTexture;
	private Season CurrSeason;
	private string CurrSeasonName;
	private float ScaleSize;
	private int CenterX;
	private int CenterY;

	private float XFactor = 0;
	private float YFactor = 0;
	private bool XRightLeft = true;
	private bool YUpDown = false;
	private int CurrentLevel;

	void Start()
	{
		GameData Data = GameObject.FindGameObjectWithTag ("GameData").GetComponent<GameData> ();
		CurrSeason = Data.SelectedSeason;
		CurrSeasonName = "Episode " + Data.SeasonNo.ToString();
		ScaleSize = 2048f / (float)Screen.currentResolution.width;
		CenterX = Screen.currentResolution.width / 2;
		CenterY = Screen.currentResolution.height / 2;

		StartCoroutine (SetFactors ());

		bool IsAllofLevelsCompleted = true;
		int i = 0;
		foreach(bool IsComplete in CurrSeason.LevelCompleted)
		{
			if (!IsComplete)
			{
				IsAllofLevelsCompleted = false;
				CurrentLevel = i;
				break;
			}
			i++;
		}
		if (IsAllofLevelsCompleted)
		{
			CurrentLevel = CurrSeason.LevelCompleted.Length - 1;
		}
	}

	void OnGUI()
	{
		GUI.skin = Skin;
		if (!GameObject.FindGameObjectWithTag("GameData").GetComponent<GameOver>().ShowGameOverScreen)
		{
			if (GUI.RepeatButton (new Rect (0, 0, Screen.currentResolution.width / 8, Screen.currentResolution.width / 8), ReturnIcon)) 
			{
				Application.LoadLevel ("SeasonManagement");
			}
		}
		float BGSize = 2048 / ScaleSize;
		float w = BGSize;
		float h = 1024 / ScaleSize;
		float SourceX = CenterX - (w / 2);
		float SourceY = CenterY - (h / 2);
		float x = SourceX;
		float y = SourceY;
		GUI.DrawTexture (new Rect (x, y, w, h), Background);
		float ButtonSize = Screen.currentResolution.width / 8;
		GUI.skin.label.fontSize = (int)(ButtonSize / 2);
		GUI.Label (new Rect (ButtonSize * 3.7f, 0, (ButtonSize * 4), ButtonSize), CurrSeasonName);
		float CurrrentXPosition = SourceX + (ButtonSize * 1.8f);
		float CurrrentYPosition = SourceY + (ButtonSize / 0.7f);
		for (int i = 0; i < CurrSeason.Levels.Length;i++) 
		{
			float newX = CurrrentXPosition;
			float newY = CurrrentYPosition;
			if (i == CurrentLevel)
			{
				newX += XFactor;
				newY += YFactor;
			}
			if (GUI.RepeatButton (new Rect (newX, newY, ButtonSize, ButtonSize), LevelIcon )) 
			{
				LoadLevel (CurrSeason.Levels[i] , i + 1);
				
			}
			GUI.skin.label.fontSize = (int)(ButtonSize / 2);
			GUI.Label(new Rect(newX + (ButtonSize / 2.8f),newY  - (ButtonSize / 5),ButtonSize / 2 , ButtonSize / 2),(i+1).ToString() );
			if (CurrSeason.LevelCompleted[i])
			{
				for (int j = 1; j <= CurrSeason.LevelStarsArrived[i];j++)
				{
					GUI.DrawTexture (new Rect((CurrrentXPosition - ButtonSize / 4.5f) + ((ButtonSize / 3.8f) * j),CurrrentYPosition  - (ButtonSize / 2),ButtonSize / 4 , ButtonSize / 4),StarTexture);
				}
			}
			if ((i + 1) % 5 == 0)
			{
				CurrrentXPosition = SourceX + (ButtonSize * 1.2f);
				CurrrentYPosition += (ButtonSize * 1.1f);
			}
			else
			{
				CurrrentXPosition += (ButtonSize * 1.15f);
			}
		}
	}

	void LoadLevel(string lvl, int lvlNo)
	{
		GameData Data = GameObject.FindGameObjectWithTag ("GameData").GetComponent<GameData> ();
		if (Data.Lives > 0)
		{
			Data.LevelNo = lvlNo;
			Application.LoadLevel (lvl);
		}
		else
		{
			GameObject.FindGameObjectWithTag("GameData").GetComponent<GameOver>().TurnOn();
		}
	}

	IEnumerator SetFactors()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.033f);
			if (XFactor >= Screen.width / 256)
			{
				XRightLeft = false;
			}
			else if (XFactor <= 0)
			{
				XRightLeft = true;
			}
			
			if (XFactor >= Screen.width / 512 && YFactor > 0)
			{
				YUpDown = true;
			}
			else if (XFactor < Screen.width / 512 && YFactor < 0)
			{
				YUpDown = false;
			}
			
			if (XRightLeft)
			{
				XFactor += 1f;
			}
			else
			{
				XFactor -= 1f;
			}
			if (YUpDown)
			{
				YFactor -= 0.5f;
			}
			else
			{
				YFactor += 0.5f;
			}
		}
	}
}
