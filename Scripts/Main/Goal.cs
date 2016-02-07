using UnityEngine;
using System.Collections;


//add this to your "level goal" trigger
[RequireComponent(typeof(CapsuleCollider))]
public class Goal : MonoBehaviour 
{
	public float lift;				//the lifting force applied to player when theyre inside the goal
	public float loadDelay;			//how long player must stay inside the goal, before the game moves onto the next level
	public string NextLevel;

	public int ConsidTime; //In Seconds
	private float counter;
	private bool ShowMenu = false;
	private GUIManager GuiManagerComponent;
	private LevelController LevelControllerComponent;
	[HideInInspector]
	public GameData data;
	private float ScorePercent;
	private float TimePercent;
	private float CoinPercent;

	private int LastScore;
	private int LastCoins;

	
	private float Star2Percentage = 70;
	private float Star3Percentage = 90;

	private int StarsArrived = 0;

	public Texture2D StarTexture;

	
	void Awake()
	{
		GetComponent<Collider>().isTrigger = true;
	}

	void Start()
	{
		GuiManagerComponent = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GUIManager>();
		LevelControllerComponent = GameObject.FindGameObjectWithTag("LevelController").GetComponent<LevelController>();
		if (GameObject.FindGameObjectWithTag("GameData"))
		{
			data = GameObject.FindGameObjectWithTag("GameData").GetComponent<GameData>();
			LastScore = data.Score;
			LastCoins = data.Coins;
		}
	}


	//when player is inside trigger for enough time, load next level
	//also lift player upwards, to give the goal a magical sense
	void OnTriggerStay(Collider other)
	{
		if (!ShowMenu)
		{
			if(other.GetComponent<Rigidbody>())
				other.GetComponent<Rigidbody>().AddForce(Vector3.up * lift, ForceMode.Force);
			
			if (other.tag == "Player")
			{
				counter += Time.deltaTime;
				if(counter > loadDelay)
				{
					CalculatePercentage();
					data.SelectedSeason.LevelCompleted[data.LevelNo - 1] = true;
					bool SeasonComplete = true;
					foreach (bool b in data.SelectedSeason.LevelCompleted)
					{
						if (b == false)
						{
							SeasonComplete = false;
							break;
						}
					}
					data.SelectedSeason.SeasonCompleted = SeasonComplete;
					data.UpdateSeasonInformation();
					GuiManagerComponent.ShowPasueButton = false;
					AddtoProperty();
					StartCoroutine(AddToCoins());
					StartCoroutine(AddToScore());
					ShowMenu = true;
					LevelControllerComponent.PauseGame();
				}
			}
		}
	}
	
	//if player leaves trigger, reset "how long they need to stay inside trigger for level to advance" timer
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
			counter = 0f;
	}

	void OnGUI()
	{
		if (ShowMenu)
		{
			float size = Screen.width / 2;
			GUI.skin.label.fontSize = (int)(size / 15);
			float x = (Screen.width / 2) - (size / 2);
			float y = (Screen.height / 2) - (size / 4);
			float ButtonSize = size / 4;
			GUI.Box (new Rect(x,y,size,size / 2),GUIContent.none);
			GUI.BeginGroup (new Rect(x,y,size,size / 2));

			GUI.Label(new Rect(0,0,size,size/5),"Total Score: " + LastScore.ToString());
			GUI.Label(new Rect(0,size / 10,size,size/5),"Total Coins: " + LastCoins.ToString());

			y = (size/2) - ((ButtonSize / 2.5f) * 2) - (ButtonSize / 2);
			float StarSize = size / 8;
			if (StarsArrived > 0)
			{
				x = size / 4;
				GUI.DrawTexture(new Rect(x,y,StarSize,StarSize),StarTexture);
			}

			if (StarsArrived > 1)
			{
				x += (StarSize + (size / 16));
				GUI.DrawTexture(new Rect(x,y,StarSize,StarSize),StarTexture);
			}

			if (StarsArrived > 2)
			{
				x += (StarSize + (size / 16));
				GUI.DrawTexture(new Rect(x,y,StarSize,StarSize),StarTexture);
			}



			y = (size / 2) - (ButtonSize / 2.5f) - (ButtonSize / 16);
			if (GUI.RepeatButton(new Rect((ButtonSize / 8) , y,ButtonSize,ButtonSize / 2.5f),"Repeat"))
			{
				LevelControllerComponent.ResumeGame();
				Application.LoadLevel(Application.loadedLevel);
			}
			if (GUI.RepeatButton(new Rect((ButtonSize * 1) + ((ButtonSize / 8) * 2) , y ,ButtonSize,ButtonSize / 2.5f),"SelectLevel"))
			{
				LevelControllerComponent.ResumeGame();
				Application.LoadLevel("LevelManagement");
			}
			if (GUI.RepeatButton(new Rect((ButtonSize * 2) + ((ButtonSize / 8) * 3) , y ,ButtonSize,ButtonSize / 2.5f),"NextLevel"))
			{
				LevelControllerComponent.ResumeGame();
				Application.LoadLevel(NextLevel);
			}
			GUI.EndGroup();
		}
	}

	void CalculatePercentage()
	{
		int TotalCoins = GuiManagerComponent.coinsInLevel;
		int CoinsCollected = GuiManagerComponent.coinsCollected;
		int TotalScore = LevelControllerComponent.TotalScoreInLevel;
		int Score = LevelControllerComponent.LevelScore;
		int TimeSpent = (int)GuiManagerComponent.timer;
		if (Score == 0 || TotalScore == 0) 
		{
			if (TotalScore == 0)
			{
				ScorePercent = 100;
			}
			else
			{
				ScorePercent = 0;
			}
		}
		else
		{
			ScorePercent = 100f / ((float)TotalScore / (float)Score);
		}
		if (TimeSpent <= 0) 
		{
			TimePercent = 0;
		}
		else if (ConsidTime <= 0)
		{
			TimePercent = 100;
		}
		else
		{
			TimePercent = 100f / ((float)TimeSpent / (float)ConsidTime);
			if (TimePercent > 100)
			{
				TimePercent = 100;
			}
		}
		if (CoinsCollected == 0)
		{
			if (TotalCoins == 0)
			{
				CoinPercent = 100;
			}
			else
			{
				CoinPercent = 0;
			}
		}
		else
		{
			CoinPercent = 100f / ((float)TotalCoins / (float)CoinsCollected);
		}

		float AveragePercentage = (ScorePercent + TimePercent + CoinPercent) / 3;
		int Stars;
		if (AveragePercentage >= Star3Percentage)
		{
			Stars = 3;
		}
		else if (AveragePercentage >= Star2Percentage )
		{
			Stars = 2;
		}
		else
		{
			Stars = 1;
		}
		if (data.SelectedSeason.LevelStarsArrived[data.LevelNo - 1] < Stars)
		{
			data.SelectedSeason.LevelStarsArrived[data.LevelNo - 1] = Stars;
		}
		StartCoroutine(AddStars (Stars));
	}

	IEnumerator AddStars(int Number)
	{
		for (int i = 1; i <= Number;i++)
		{
			yield return new WaitForSeconds (1);
			StarsArrived ++;
		}
	}
	
	void AddtoProperty()
	{
		data.GetCoinAndScore(GuiManagerComponent.coinsCollected,LevelControllerComponent.LevelScore);
	}
	

	IEnumerator AddToCoins()
	{
		while (data.Coins > LastCoins)
		{
			yield return new WaitForSeconds (0.5f);
			LastCoins += 1;
		}
	}

	IEnumerator AddToScore()
	{
		while (LevelControllerComponent.LevelScore > 0)
		{
			yield return new WaitForSeconds (0.05f);
			LevelControllerComponent.LevelScore -= 1;
			LastScore += 1;
		}
	}

}