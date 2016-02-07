using UnityEngine;
using System.Collections;

public class CannonballController : MonoBehaviour {
		public float Direction;
		public GameObject[] spawnOnDeath;				//objects to spawn upon death of this object (ie: a particle effect or a coin)
		public float Range;
		private float DistanceElapsed = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!GameObject.FindGameObjectWithTag("LevelController").GetComponent<LevelController>().Pause && Range != 0)
		{
			transform.Translate ( new Vector3(Direction, 0,0));
			if (Direction >= 0)
			{
				DistanceElapsed += 1f;
				if (DistanceElapsed > Range)
				{
					Explosion();
				}
			}
			else if (Direction < 0)
			{
				DistanceElapsed -= 1f;
				if (DistanceElapsed < Range)
				{
					Explosion();
				}
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
			if (col.tag == "Player") 
			{
				GetComponent<DealDamage> ().Attack (col.gameObject, 1, 7f, 10f);
				Explosion();
			}
	}
	void OnTriggerExit(Collider col)
	{
		if (!col.isTrigger && col.tag != "Enemy")
		{
			Explosion();
		}
	}

	void Explosion()
	{
		foreach(GameObject obj in spawnOnDeath)
			Instantiate(obj, transform.position, Quaternion.Euler(Vector3.zero));
		Destroy (gameObject);
	}
	
}
