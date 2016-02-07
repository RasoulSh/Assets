using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour 
{
	public bool ShowGameOverScreen;

	void Update () 
	{

	}

	void OnGUI()
	{
		if (ShowGameOverScreen)
		{
			float size = Screen.width / 2;
			GUI.skin.label.fontSize = (int)(size / 15);
			float x = (Screen.width / 2) - (size / 2);
			float y = (Screen.height / 2) - (size / 4);
			float ButtonSize = size / 4;
			GUI.Box (new Rect(x,y,size,size / 2),GUIContent.none);
			GUI.BeginGroup (new Rect(x,y,size,size / 2));
			if (GUI.RepeatButton(new Rect((size / 2) - (ButtonSize / 2) , (ButtonSize / 2.5f) + (ButtonSize / 4),ButtonSize,ButtonSize / 2.5f),"Main Menu"))
			{
				Time.timeScale = 1;
				ShowGameOverScreen = false;
				Application.LoadLevel("MainMenu");
			}
			GUI.EndGroup();
		}
	}

	public void TurnOn()
	{
		ShowGameOverScreen = true;
		if (GameObject.FindGameObjectWithTag ("LevelController")) 
		{
			GameObject.FindGameObjectWithTag ("LevelController").GetComponent<LevelController>().PauseGame();
		}
	}
}
