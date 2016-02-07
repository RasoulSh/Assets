using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour 
{
	public bool Live = false;
	public bool Magnet = false;
	public bool Rolling = false;
	public bool Guard = false;
	public bool Gem = false;
	public bool Health = false;
	public int Coin = 0;
	public GameObject SpawnOnGetPrize;

	public void CheckForPrize()
	{
		GameObject.FindGameObjectWithTag ("Player").GetComponent<PrizeController> ().Live = Live;
		GameObject.FindGameObjectWithTag ("Player").GetComponent<PrizeController> ().Magnet = Magnet;
		GameObject.FindGameObjectWithTag ("Player").GetComponent<PrizeController> ().Rolling = Rolling;
		GameObject.FindGameObjectWithTag ("Player").GetComponent<PrizeController> ().Guard = Guard;
		GameObject.FindGameObjectWithTag ("Player").GetComponent<PrizeController> ().Gem = Gem;
		GameObject.FindGameObjectWithTag ("Player").GetComponent<PrizeController> ().Health = Health;
		GameObject.FindGameObjectWithTag ("Player").GetComponent<PrizeController> ().Coin = Coin;
		if ((Live || Magnet || Rolling || Guard || Gem || Health || Coin > 0) && SpawnOnGetPrize )
		{
			Instantiate(SpawnOnGetPrize,transform.position,Quaternion.Euler(Vector3.zero));
		}
	}
}
