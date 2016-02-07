using UnityEngine;
using System.Collections;

[System.Serializable]
public class Season
{
	public string[] Levels;
	public  bool SeasonCompleted;
	public bool[] LevelCompleted;
	public int[] LevelStarsArrived;
}
[System.Serializable]
public class SeasonIcon
{
	public Texture2D Icon;

	public SeasonIcon(Texture2D icon)
	{
		this.Icon = icon;
	}
}
