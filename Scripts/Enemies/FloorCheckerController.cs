using UnityEngine;
using System.Collections;

public class FloorCheckerController : MonoBehaviour 
{
	public GameObject Master;
	public int Type = -1;
	public bool IsSolid = false;
	private bool TriggerEntered = false;
	
	

	void Start()
	{
	}

	void Update()
	{
			if (TriggerEntered && Master) 
			{
				if (Type == 0)
				{
					Master.GetComponent<Frog>().CanJumpRight = true;
				}
				else if (Type == 1)
				{
					Master.GetComponent<Frog>().CanJumpLeft = true;
				}
			}
	}
	

	void OnTriggerEnter(Collider col)
	{
			if (col.tag == "Untagged") 
			{
				TriggerEntered = true;
			}
	}
	
}
