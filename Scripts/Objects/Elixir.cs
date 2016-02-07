using UnityEngine;
using System.Collections;

public class Elixir : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

		void OnTriggerEnter(Collider col)
		{
				if (col.gameObject.tag == "Player") 
				{
					GameData Data = GameObject.FindGameObjectWithTag ("GameData").GetComponent<GameData> ();
					Data.GetLive(1);
					Destroy (gameObject);
				}
						
		}
}
