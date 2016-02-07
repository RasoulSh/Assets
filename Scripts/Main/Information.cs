using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Information
{
	public int Lives;
	public int Gems;
	public int Coins;
	public int Score;
	public bool Mute;
	public string SerialNumber;
	
	public Information (int lives, int gems, int coins, int score, bool Mute) 
	{
		this.Lives = lives;
		this.Gems = gems;
		this.Coins = coins;
		this.Score = score;
		this.Mute = Mute;
		if (Application.platform == RuntimePlatform.Android)
		{
			this.SerialNumber = SerialNumberTools.GetSerialNumberAndroid();
		}
	}
	
}

[System.Serializable]
public class SeasonInformation
{
	public List<Season> Seasons = new List<Season>();
	public string SerialNumber;
	
	public SeasonInformation(List<Season> seasons)
	{
		this.Seasons = seasons;
		if (Application.platform == RuntimePlatform.Android)
		{
			this.SerialNumber = SerialNumberTools.GetSerialNumberAndroid();
		}
	}
}

[System.Serializable]
public class IconsInformation
{
	public List<StoreIcon> StoreIcons = new List<StoreIcon> ();
	public string SerialNumber;

	public IconsInformation(List<StoreIcon> seasonicons)
	{
		StoreIcons = seasonicons;
		if (Application.platform == RuntimePlatform.Android)
		{
			this.SerialNumber = SerialNumberTools.GetSerialNumberAndroid();
		}
	}
}

public static class SerialNumberTools
{
	public static string GetSerialNumberAndroid()
	{
		AndroidJavaObject jo = new AndroidJavaObject("android.os.Build");
		return jo.GetStatic<string>("SERIAL");
	}
}

