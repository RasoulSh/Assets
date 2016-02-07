using UnityEngine;
using System.Collections;

//ATTACH TO MAIN CAMERA, shows your health and coins
public class GUIManager : MonoBehaviour 
{	
	public GUISkin guiSkin;					//assign the skin for GUI display
	public Font GemsFont;
	public Font TimerFont;
	public Font LivesFont;
	public Font CoinsFont;
	public Texture2D HUDMain;
	public Texture2D LiveIcon;
	public Texture2D ShareIcon;
	private bool isProcessing = false;
	private Texture2D Screenshot;
	private bool TakeScreenShot = false;

	private bool ShowMenu = false;
	public bool ShowPasueButton = true;
	


	[HideInInspector]
	public int coinsCollected;

	public int coinsInLevel;
	private Health health;
	private GameData data;
	private GameOver gameover;
	private LevelController LevelControllerComponent;
	private int MaxHealth;

	public float timer;

	
	//setup, get how many coins are in this level
	void Start()
	{
		Screenshot = new Texture2D(Screen.width, Screen.height);
		coinsInLevel = GameObject.FindGameObjectsWithTag("Coin").Length;		
		health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
		if (GameObject.FindGameObjectWithTag("GameData"))
		{
			data = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
			gameover = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameOver>();
		}
		if (GameObject.FindGameObjectWithTag("LevelController"))
		{
			LevelControllerComponent = GameObject.FindGameObjectWithTag("LevelController").GetComponent<LevelController>();
		}
		MaxHealth = health.maxHealth;
	}

	void Update()
	{
		if (!LevelControllerComponent.Pause) 
		{
			timer += Time.deltaTime;
		}
		if (gameover && gameover.ShowGameOverScreen)
		{
			ShowPasueButton = false;
		}
		else
		{
			ShowPasueButton = true;
		}
	}
	
	//show current health and how many coins you've collected
	void OnGUI()
	{
		GUI.skin = guiSkin;
		GUI.skin.label.alignment = TextAnchor.MiddleLeft;
		float Mainx = -35 ;
		float Mainy = Screen.height / 50;
		float HUDSize = Screen.currentResolution.width / 2.5f;
		GUI.DrawTexture (new Rect(Mainx,Mainy,HUDSize,HUDSize / 2),HUDMain );
		float w = HUDSize / 8;
		float h = HUDSize / 8;
		float x = Mainx + (HUDSize / 1.8f);
		float y = Mainy + ((HUDSize / 2) / 4.2f);
		GUI.DrawTexture(new Rect (x,y,w,h),LiveIcon);
	
		w = HUDSize / 4;
		h = (HUDSize / 2) / 6;
		x = Mainx + (HUDSize / 1.55f);
		y = Mainy + ((HUDSize / 2) / 3.4f);
		if (data != null) 
		{
				GUI.skin.label.font = LivesFont;
				GUI.skin.label.normal.textColor = Color.white;
				GUI.skin.label.fontSize = (int)(HUDSize / 20);
				GUI.Label (new Rect(x,y,w,h), data.Lives.ToString());
		}
		x += HUDSize / 256;
		y += ((HUDSize / 2) / 3.8f);
		if (coinsInLevel > 0) 
		{
			GUI.skin.label.font = CoinsFont;				
			GUI.skin.label.normal.textColor = Color.yellow;
			GUI.skin.label.fontSize = (int)(HUDSize / 20);
			GUI.Label (new Rect(x,y,w,h), coinsCollected.ToString() + " / " + coinsInLevel.ToString());
		}	
		h = HUDSize / 128;
		w = HUDSize / 3.5f;
		x = Mainx + (HUDSize / 1.88f);
		y = Mainy + (HUDSize / 10);

		
		//w = w / (MaxHealth / health.currentHealth);
		float HealthbarWidth = w;
		HealthbarWidth = w	/ ((float)MaxHealth / (float)health.currentHealth);
		if(health)
				GUI.Box ( new Rect(x,y,HealthbarWidth,h),GUIContent.none);

		float minutes = Mathf.Floor(timer / 60); float seconds = Mathf.RoundToInt(timer%60);
		string minutesString = minutes.ToString();
		string secondsString = seconds.ToString();
		if(minutes < 10)
		{
			minutesString = "0" + minutes.ToString(); 
		} 
		if(seconds < 10)
		{
			secondsString = "0" + Mathf.RoundToInt(seconds).ToString(); 
		} 
		w = HUDSize / 4;
		h = (HUDSize / 2) / 6;
		x = Mainx + (HUDSize / 1.9f);
		y = Mainy + ((HUDSize / 2) / 25f);
		GUI.skin.label.font = TimerFont;
		GUI.skin.label.fontSize = (int)(HUDSize / 18);
		GUI.skin.label.normal.textColor = Color.white;
		GUI.Label(new Rect(x,y,w,h), minutesString + ":" + secondsString); 
		w = HUDSize / 4;
		h = HUDSize / 3;
		x = Mainx + (HUDSize / 5f);
		y = Mainy + (HUDSize / 10f);
		if (data != null) 
		{
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.skin.label.font = GemsFont;
			GUI.skin.label.fontSize = (int)(HUDSize / 8);
			GUI.Label (new Rect(x,y,w,h), data.Gems.ToString());
		}

		w = HUDSize / 2.5f;
		h = HUDSize / 5;
		x = Screen.width  - (HUDSize * 0.6f);
		y = Screen.currentResolution.height / 22;
		if (LevelControllerComponent) 
		{
			GUI.skin.label.alignment = TextAnchor.MiddleRight;
			GUI.skin.label.font = LivesFont;
			GUI.skin.label.normal.textColor = Color.grey;
			GUI.skin.label.fontSize = (int)(HUDSize / 10);
			GUI.Label (new Rect(x,y,w,h), LevelControllerComponent.LevelScore.ToString());
		}


		w = HUDSize / 5;
		h = HUDSize / 5;
		x = Screen.width - (HUDSize / 5);
		y = Screen.currentResolution.height / 12;
		if (ShareIcon)
		{
			if (GUI.Button (new Rect(x,y,w,h), ShareIcon))
			{
				if (data != null)
				{
					TakeScreenShot = true;
				}
			}
		}
		GUI.skin = null;
		x = Screen.currentResolution.width - (HUDSize / 7) ;
		y = HUDSize / 128;
		w = HUDSize / 8;
		h = HUDSize / 16;
		if (ShowPasueButton)
		{
			if (GUI.Button (new Rect (x, y, w, h), "Pause")) 
			{
				Time.timeScale = 0;
				GameObject.FindGameObjectWithTag("LevelController").GetComponent<LevelController>().PauseGame();
				ShowMenu = true;
			}
		}
		if (ShowMenu) 
		{
			float size = Screen.width / 2;
			x = (Screen.width / 2) - (size / 2);
			y = (Screen.height / 2) - (size / 4);
			float ButtonSize = size / 4;
			GUI.Box (new Rect(x,y,size,size / 2),GUIContent.none);
			GUI.BeginGroup (new Rect(x,y,size,size / 2));
			if (GUI.RepeatButton(new Rect((size / 2) - (ButtonSize / 2) , (ButtonSize / 8),ButtonSize,ButtonSize / 2.5f),"Resume"))
			{
				GameObject.FindGameObjectWithTag("LevelController").GetComponent<LevelController>().ResumeGame();
				ShowMenu = false;
			}
			if (GUI.RepeatButton(new Rect((size / 2) - (ButtonSize / 2) , (ButtonSize / 2.5f) + (ButtonSize / 4),ButtonSize,ButtonSize / 2.5f),"Main Menu"))
			{
				Time.timeScale = 1;
				Application.LoadLevel("MainMenu");
			}
			GUI.EndGroup();
		}
	}

	IEnumerator OnPostRender()
	{
		if (TakeScreenShot)
		{
			yield return new WaitForEndOfFrame ();
			Screenshot.ReadPixels(new Rect(0,0,Screen.width,Screen.height),0,0);
			Screenshot.Apply();
			TakeScreenShot = false;
			string shareText = "Here is my score on Boxes: " + data.Score.ToString();
			string subject = "Boxes Score";
			string gameLink = "";
			string imageName = "ScreenShot";
			if (!isProcessing) 
			{
				isProcessing = true;
				
				
				byte[] dataToSave = Screenshot.EncodeToJPG ();
				
				string destination = Application.persistentDataPath + "/" + imageName + ".jpg" ;
				System.IO.File.WriteAllBytes(destination, dataToSave);

				
				if(Application.platform == RuntimePlatform.Android)
				{
					
					AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
					AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
					intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
					AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
					AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse","file://" + destination);
					intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
					intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), shareText + gameLink);
					intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), subject);
					intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
					AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
					AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
					
					currentActivity.Call("startActivity", intentObject);
					
				}
				
				isProcessing = false;
				
			}
		}
	}

}