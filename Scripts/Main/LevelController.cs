using UnityEngine;
using System.Collections;

public class LevelController : MonoBehaviour {
	private GameData data;
	private AudioListener Listener;
	public bool Pause = false;
	public int TotalScoreInLevel;
	public int LevelScore;
	public GameObject GameDataObject;

	void Start()
	{
		if (!GameObject.FindGameObjectWithTag ("GameData"))
		{
			GameObject newgamedata = (GameObject)Instantiate(GameDataObject,transform.position,transform.rotation);
			newgamedata.name = "GameData";
			newgamedata.GetComponent<GameData>().SeasonNo = 1;
			newgamedata.GetComponent<GameData>().LevelNo = 1;
			newgamedata.GetComponent<GameData>().SelectedSeason = newgamedata.GetComponent<GameData>().Seasons[0];
			GameObject.FindGameObjectWithTag("Finish").GetComponent<Goal>().data = newgamedata.GetComponent<GameData>();
		}
		GameObject GameDataObj = GameObject.FindGameObjectWithTag ("GameData");
		Listener = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<AudioListener> ();
		if (GameDataObj)
		{
			data = GameObject.FindGameObjectWithTag ("GameData").GetComponent<GameData> ();
			if (Listener && data.Mute)
			{
				Listener.enabled = false;
			}
		}
		GameObject[] Enemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject Enemy in Enemies)
		{
			if (Enemy.GetComponent<EnemyAI>())
			{
				TotalScoreInLevel += Enemy.GetComponent<EnemyAI>().Score;
			}
		}
	}

	public void PauseGame()
	{
				
		
		GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody> ().Sleep ();
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>().animator.SetFloat("DistanceToTarget", 0);
		Pause = true;
	}

	public void ResumeGame()
	{
		GameObject.FindGameObjectWithTag ("Player").GetComponent<Rigidbody> ().WakeUp ();
		Time.timeScale = 1;
		Pause = false;
	}
}
