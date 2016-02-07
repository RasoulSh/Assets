using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData : MonoBehaviour 
{
	public List<Season> Seasons = new List<Season>();
	public List<SeasonIcon> SeasonIcons = new List<SeasonIcon>();
	public Season SelectedSeason;
	public int SeasonNo;
	public int LevelNo;

	public int Lives;
	public int Gems;
	public int Coins;
	public int Score;

	public bool Mute;

	public bool InformationsLoaded;

	void Awake()
	{
		DontDestroyOnLoad (gameObject);
	}

	public void MissLive(int count)
	{
		Lives -= count;
		UpdateInformation ();
	}
	
	public void GetLive(int count)
	{
		Lives += count;
		UpdateInformation ();
	}

	public void MissGem(int count)
	{
		Gems -= count;
		UpdateInformation ();
	}

	public void GetGem(int count)
	{
		Gems += count;
		UpdateInformation ();
	}

	public void MissCoin(int count)
	{
		Coins -= count;
		UpdateInformation ();
	}
	
	public void GetCoin(int count)
	{
		Coins += count;
		UpdateInformation ();
	}

	public void GetCoinAndScore(int Coincount, int ScoreCount)
	{
		Coins += Coincount;
		Score += ScoreCount;
		UpdateInformation ();
	}

	private void UpdateInformation()
	{
		Information info = new Information(Lives,Gems,Coins,Score,Mute);
		SaveLoad.Save(info);
	}

	public void UpdateSeasonInformation()
	{
		SeasonInformation info = new SeasonInformation (Seasons);
		SaveLoad.SaveSeasons (info);
	}

//	float deltaTime = 0.0f;
//	
//	void Update()
//	{
//		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
//	}
//	
//	void OnGUI()
//	{
//		int w = Screen.width, h = Screen.height;
//		
//		GUIStyle style = new GUIStyle();
//		
//		Rect rect = new Rect(0, 0, w, h * 2 / 100);
//		style.alignment = TextAnchor.UpperLeft;
//		style.fontSize = h * 2 / 100;
//		style.normal.textColor = Color.red;
//		float msec = deltaTime * 1000.0f;
//		float fps = 1.0f / deltaTime;
//		string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
//		GUI.Label(rect, text, style);
//	}

}
