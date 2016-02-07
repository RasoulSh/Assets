using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad 
{
	public static Information savedGame;
	public static SeasonInformation savedSeason;
	public static IconsInformation savedicons;

	public static void Save(Information info) 
	{
		savedGame = new Information (info.Lives,info.Gems,info.Coins,info.Score,info.Mute);
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/savedGames.gd");
		bf.Serialize(file, SaveLoad.savedGame);
		file.Close();
	}

	public static void Load(ref Information info) 
	{
		if(File.Exists(Application.persistentDataPath + "/savedGames.gd")) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
			SaveLoad.savedGame = (Information)bf.Deserialize(file);
			file.Close();
			bool AllowAccess = true;
			if (Application.platform == RuntimePlatform.Android)
			{
				if (savedGame.SerialNumber != SerialNumberTools.GetSerialNumberAndroid())
				{
					AllowAccess = false;
				}
			}
			if (AllowAccess)
			{
				info = new Information(savedGame.Lives,savedGame.Gems,savedGame.Coins,savedGame.Score,savedGame.Mute);
			}
			else
			{
				Save(info);
			}
		}
		else
		{
			Save(info);
		}
	}

	public static void SaveSeasons(SeasonInformation info)
	{
		List<Season> s = info.Seasons;
		savedSeason = new SeasonInformation (s);
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/savedSeasons.gd");
		bf.Serialize(file, SaveLoad.savedSeason);
		file.Close();
	}

	public static void LoadSeasons(ref SeasonInformation info)
	{
		if(File.Exists(Application.persistentDataPath + "/savedSeasons.gd") && File.Exists(Application.persistentDataPath + "/savedIcons.gd")) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedSeasons.gd", FileMode.Open);
			SaveLoad.savedSeason = (SeasonInformation)bf.Deserialize(file);
			file.Close();
			bool AllowAccess = true;
			if (Application.platform == RuntimePlatform.Android)
			{
				if (savedSeason.SerialNumber != SerialNumberTools.GetSerialNumberAndroid())
				{
					AllowAccess = false;
				}
			}
			if (AllowAccess)
			{
				info = new SeasonInformation(savedSeason.Seasons);
			}
			else
			{
				SaveSeasons(info);
			}
		}
		else
		{
			SaveSeasons(info);
		}
	}

	public static void SaveIcons(IconsInformation info)
	{
		List<StoreIcon> s = info.StoreIcons;
		savedicons = new IconsInformation (s);
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/savedIcons.gd");
		bf.Serialize(file, SaveLoad.savedicons);
		file.Close();
	}

	public static void LoadIcons(ref IconsInformation info)
	{
		if(File.Exists(Application.persistentDataPath + "/savedIcons.gd") && File.Exists(Application.persistentDataPath + "/savedSeasons.gd")) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/savedIcons.gd", FileMode.Open);
			SaveLoad.savedicons = (IconsInformation)bf.Deserialize(file);
			file.Close();
			bool AllowAccess = true;
			if (Application.platform == RuntimePlatform.Android)
			{
				if (savedicons.SerialNumber != SerialNumberTools.GetSerialNumberAndroid())
				{
					AllowAccess = false;
				}
			}
			if (AllowAccess)
			{
				info = new IconsInformation(savedicons.StoreIcons);
			}
			else
			{
				SaveIcons(info);
			}
		}
		else
		{
			SaveIcons(info);
		}
	}
}

[System.Serializable]
public class StoreIcon
{
	public byte[] Icon;

	public StoreIcon(byte[] icon)
	{
		this.Icon = icon;
	}
	public static StoreIcon ConvertToStoreIcon(SeasonIcon seasonicon)
	{
		Texture2D IconTexture = new Texture2D (seasonicon.Icon.width,seasonicon.Icon.height,TextureFormat.ARGB32,true);
		for (int i = 1; i <= seasonicon.Icon.width;i++)
		{
			for (int j = 1; j <= seasonicon.Icon.height;j++)
			{
				IconTexture.SetPixel(i,j,seasonicon.Icon.GetPixel(i,j));
			}
		}
		IconTexture.Apply();
		byte[] icon = IconTexture.EncodeToPNG ();
		StoreIcon si = new StoreIcon (icon);
		return si;
	}
	public static SeasonIcon ConvertToSeasonIcon(StoreIcon storeicon)
	{
		Texture2D icon = new Texture2D (0,0);
		icon.LoadImage (storeicon.Icon);
		SeasonIcon si = new SeasonIcon (icon);
		return si;
	}
}
