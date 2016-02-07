using UnityEngine;
using System.Collections;

public class CannonController : MonoBehaviour {
		public GameObject Cannonball;
		private float NextActionTime = 0.0f;
		public float StopTime;
		public float Velocity;
		public float Range;
		public bool RightShooter;
		public bool LeftShooter;
		private bool Enable = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Enable && Time.time > NextActionTime) 
		{
			NextActionTime += StopTime;
			if(RightShooter)
			{
				GameObject NewCannonball = (GameObject) Instantiate (Cannonball, transform.position, transform.rotation);
				CannonballController cbc = NewCannonball.GetComponent<CannonballController> ();
				cbc.Direction = (Velocity / 10);
				cbc.Range = Range;
			}
			if (LeftShooter)
			{
				GameObject NewCannonball = (GameObject) Instantiate (Cannonball, transform.position, transform.rotation);
				CannonballController cbc = NewCannonball.GetComponent<CannonballController> ();
				cbc.Direction = 0 - (Velocity / 10);
				cbc.Range = Range * -1f ;
			}
		}
	}

	void OnBecameVisible()
	{
		NextActionTime = Time.time;
		Enable = true;
	}

	void OnBecameInvisible()
	{
		Enable = false;
	}
}
