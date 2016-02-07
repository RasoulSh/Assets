using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour 
{
	private RigidbodyConstraints FirstConstraints;
	private bool FallDown = false;
	private float CurrentTime;
	private float TimePeriod = 0.1f;
	private int Counter = 0;

	void Start()
	{
		FirstConstraints = GetComponent<Rigidbody> ().constraints;
	}

	void Update()
	{
		if (FallDown)
		{
			if (Counter >= 10)
			{
				CurrentTime = 0f;
				Counter = 0;
				FallDown = false;
			}
			if (CurrentTime == 0f)
			{
				CurrentTime = Time.time;
			}
			if (Time.time - CurrentTime >= TimePeriod)
			{
				Counter++;
				transform.Translate(0,-0.2f,0);
				CurrentTime = Time.time;
			}
		}
	}


	public void FreezeYPosition()
	{
		GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionY;
		FallDown = true;
	}

	public void UnFreezeYPosition()
	{
		GetComponent<Rigidbody>().constraints = FirstConstraints;
	}

}
