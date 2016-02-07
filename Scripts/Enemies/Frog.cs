using UnityEngine;
using System.Collections;

public class Frog : MonoBehaviour {

	public float Step;
	public float High;
	public float Velocity;
	public GameObject FloorChecker;
	public GameObject SightBounds;
	private TriggerParent SightTrigger;

	private Vector3 NextStep;
	private float CurrHigh;
	private float EarthY;
	private GameObject RightFloorChecker;
	private GameObject LeftFloorChecker;

	public bool CanJumpRight;
	public bool CanJumpLeft;

	private bool StartJumping;

	private Vector3 Target;
	private int Direction;

	private float LastPositionX;

	private bool Enable = false;


	void Awake () 
	{
		EarthY = transform.position.y;
		High += EarthY;
		Velocity = Velocity / 5;
		Direction = 1;
		if (SightBounds)
		{
			SightTrigger = SightBounds.GetComponent<TriggerParent>();
		}
	}

	void Start()
	{
		CreateFloorCheckers ();
	}

	void Update () 
	{
		if (Enable && !GameObject.FindGameObjectWithTag ("LevelController").GetComponent<LevelController> ().Pause) 
		{
			if (SightTrigger)
			{
				if (StartJumping)
				{
					CalculateJumping();
				}
				else if (SightTrigger.colliding)
				{
					StartJumping = true;
				}
			}
			else
			{
				CalculateJumping();
			}
		}
	}

	void CalculateJumping()
	{
		if (!GameObject.FindGameObjectWithTag ("LevelController").GetComponent<LevelController> ().Pause) 
		{
			if (CanJumpRight && CanJumpLeft) 
			{
				LastPositionX = transform.position.x;
				if (Direction == 0)
				{
					Target = new Vector3(RightFloorChecker.transform.position.x,RightFloorChecker.transform.position.y,RightFloorChecker.transform.position.z);
				}
				else if (Direction == 1)
				{
					Target = new Vector3(LeftFloorChecker.transform.position.x,LeftFloorChecker.transform.position.y,LeftFloorChecker.transform.position.z);
				}
				ClearFloorChecker();
			}
			else if (CanJumpRight)
			{
				LastPositionX = transform.position.x;
				Direction = 0;
				Target = new Vector3(RightFloorChecker.transform.position.x,RightFloorChecker.transform.position.y,RightFloorChecker.transform.position.z);
				ClearFloorChecker();
			}
			else if (CanJumpLeft)
			{
				LastPositionX = transform.position.x;
				Direction = 1;
				Target = new Vector3(LeftFloorChecker.transform.position.x,LeftFloorChecker.transform.position.y,LeftFloorChecker.transform.position.z);
				ClearFloorChecker();
			}
			Jump ();
		}
	}

	void Jump()
	{
		if (!GameObject.FindGameObjectWithTag ("LevelController").GetComponent<LevelController> ().Pause) 
		{
			if (Target != Vector3.zero) 
			{
				if (transform.position == Target)
				{
					Target = Vector3.zero;
					CreateFloorCheckers();
				}
				else
				{
					if (Direction == 0)
					{
						if (transform.position.x > (LastPositionX / 2 + Target.x / 2))
						{
							Target.y = EarthY;
						}
						else
						{
							Target.y = High;
						}
					}
					else if (Direction == 1)
					{
						if (transform.position.x < (LastPositionX / 2 + Target.x / 2))
						{
							Target.y = EarthY;
						}
						else
						{
							Target.y = High;
						}
					}
					transform.position = Vector3.MoveTowards (transform.position, Target, Velocity * Time.deltaTime);
				}
			}
		}
	}

	void CreateFloorCheckers()
	{
		RightFloorChecker = (GameObject)Instantiate (FloorChecker, new Vector3 (transform.position.x + Step, EarthY, transform.position.z), transform.rotation);
		RightFloorChecker.transform.parent = transform;
		RightFloorChecker.GetComponent<FloorCheckerController> ().Master = gameObject;
		RightFloorChecker.GetComponent<FloorCheckerController> ().Type = 0;
		LeftFloorChecker = (GameObject)Instantiate (FloorChecker, new Vector3 (transform.position.x - Step, EarthY, transform.position.z), transform.rotation);
		LeftFloorChecker.transform.parent = transform;
		LeftFloorChecker.GetComponent<FloorCheckerController> ().Master = gameObject;
		LeftFloorChecker.GetComponent<FloorCheckerController> ().Type = 1;
	}

	void ClearFloorChecker()
	{
		CanJumpLeft = false;
		CanJumpRight = false;
		Destroy(RightFloorChecker);
		Destroy(LeftFloorChecker);
	}

	void OnBecameVisible()
	{
		Enable = true;
	}
	
	void OnBecameInvisible()
	{
		Enable = false;
	}
}
