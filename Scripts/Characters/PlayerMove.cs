using UnityEngine;
using System.Collections;

//handles player movement, utilising the CharacterMotor class
[RequireComponent(typeof(CharacterMotor))]
[RequireComponent(typeof(DealDamage))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMove : MonoBehaviour 
{
	//setup
	public bool sidescroller;					//if true, won't apply vertical input
	public Transform mainCam, floorChecks;		//main camera, and floorChecks object. FloorChecks are raycasted down from to check the player is grounded.
	public Animator animator;					//object with animation controller on, which you want to animate
	public AudioClip jumpSound;					//play when jumping
	public AudioClip landSound;					//play when landing on ground
	
	//movement
	public float accel = 70f;					//acceleration/deceleration in air or on the ground
	public float airAccel = 18f;			
	public float decel = 7.6f;
	public float airDecel = 1.1f;
	[Range(0f, 5f)]
	public float rotateSpeed = 0.7f, airRotateSpeed = 0.4f;	//how fast to rotate on the ground, how fast to rotate in the air
	public float maxSpeed = 9;								//maximum speed of movement in X/Z axis
	public float slopeLimit = 40, slideAmount = 35;			//maximum angle of slopes you can walk on, how fast to slide down slopes you can't
	public float movingPlatformFriction = 7.7f;				//you'll need to tweak this to get the player to stay on moving platforms properly
	
	//jumping
	public float MinJump = 13;
	public float MaxJump = 20;
	public Vector3 jumpForce =  new Vector3(0, 13, 0);		//normal jump force
	public Vector3 secondJumpForce = new Vector3(0, 13, 0); //the force of a 2nd consecutive jump
	public Vector3 thirdJumpForce = new Vector3(0, 13, 0);	//the force of a 3rd consecutive jump
	public Vector3 rollForce =  new Vector3(13, 0, 0);
	public float JumpHeight = 0;
	public float jumpDelay = 0.1f;							//how fast you need to jump after hitting the ground, to do the next type of jump
	public float jumpLeniancy = 0.17f;						//how early before hitting the ground you can press jump, and still have it work
	[HideInInspector]
	public int onEnemyBounce;					
	
	private int onJump;
	private bool grounded = true;
	private Transform[] floorCheckers;
	private Quaternion screenMovementSpace;
	private float airPressTime, groundedCount, curAccel, curDecel, curRotateSpeed, slope;
	private Vector3 direction, moveDirection, screenMovementForward, screenMovementRight, movingObjSpeed;
	
	private CharacterMotor characterMotor;
	private EnemyAI enemyAI;
	public DealDamage dealDamage;

	private float GroundY = 0;

	private HUD TouchController;
	public bool SimulateInputRight;
	public bool SimulateInputLeft;
	public bool SimulateInputJump;
	public bool SimulateInputRolling;
	public bool SimulateInputRollingRight;
	public bool SimulateInputRollingLeft;
	private bool MovingRight = true;
	public bool IsRolling = false;
	private float RollingStartTime;
	private float RollingTime = 0.4f;
	private int RollSide;
	private bool ExecuteJump;
	private float MaxJumpTime = 0.03f;
	private float StartJumpTime = 0;
	public bool IsOnWater = false;
	public float WaterY;
	public bool AntiAttack = false;

	//Swiming Effects
	private int MoveDirection = 0;
	public GameObject RippleEffect;
	public GameObject BubbleBreathEffect;
	private float RipplePriod = 1f;
	private float CurrentRipplePriod = 0f;

	
	
	//setup
	void Awake()
	{
		//create single floorcheck in centre of object, if none are assigned
		if(!floorChecks)
		{
			floorChecks = new GameObject().transform;
			floorChecks.name = "FloorChecks";
			floorChecks.parent = transform;
			floorChecks.position = transform.position;
			GameObject check = new GameObject();
			check.name = "Check1";
			check.transform.parent = floorChecks;
			check.transform.position = transform.position;
			Debug.LogWarning("No 'floorChecks' assigned to PlayerMove script, so a single floorcheck has been created", floorChecks);
		}
		//assign player tag if not already
		if(tag != "Player")
		{
			tag = "Player";
			Debug.LogWarning ("PlayerMove script assigned to object without the tag 'Player', tag has been assigned automatically", transform);
		}
		//usual setup
		mainCam = GameObject.FindGameObjectWithTag("MainCamera").transform;
		dealDamage = GetComponent<DealDamage>();
		characterMotor = GetComponent<CharacterMotor>();
		//gets child objects of floorcheckers, and puts them in an array
		//later these are used to raycast downward and see if we are on the ground
		floorCheckers = new Transform[floorChecks.childCount];
		for (int i=0; i < floorCheckers.Length; i++)
			floorCheckers[i] = floorChecks.GetChild(i);
	}

	void Start()
	{
		TouchController = mainCam.GetComponent<HUD> ();
	}
	
	//get state of player, values and input
	void Update()
	{	
		if (!GameObject.FindGameObjectWithTag ("LevelController").GetComponent<LevelController> ().Pause) 
		{
			//handle jumping
			if (!IsRolling) JumpCalculations ();
			//adjust movement values if we're in the air or on the ground
			curAccel = (grounded) ? accel : airAccel;
			curDecel = (grounded) ? decel : airDecel;
			curRotateSpeed = (grounded) ? rotateSpeed : airRotateSpeed;
			
			//get movement axis relative to camera
			screenMovementSpace = Quaternion.Euler (0, mainCam.eulerAngles.y, 0);
			screenMovementForward = screenMovementSpace * Vector3.forward;
			screenMovementRight = screenMovementSpace * Vector3.right;
			float h = 0;
			float v = 0;
			if (!IsRolling)
			{
				//get movement input, set direction to move in
				h = Input.GetAxisRaw ("Horizontal");
				v = Input.GetAxisRaw ("Vertical");
				if (SimulateInputRight) 
				{
					h = 1;
				}
				if (SimulateInputLeft) 
				{
					h = -1;
				}

				if (h > 0)
				{
					MoveDirection = 1;
				}
				else if (h < 0)
				{
					MoveDirection = -1;
				}
				else
				{
					MoveDirection = 0;
				}

			}
			if (h > 0)
			{
				MovingRight = true;
			}
			if (h < 0)
			{
				MovingRight = false;
			}

			if (SimulateInputRolling)
			{
				IsRolling = true;
				RollSide = 0;
			}
			else if (SimulateInputRollingRight)
			{
				IsRolling = true;
				RollSide = 1;
			}
			else if (SimulateInputRollingLeft)
			{
				IsRolling = true;
				RollSide = 2;
			}

			if (IsRolling)
			{
				Roll(rollForce, RollSide);
			}
			
			//only apply vertical input to movemement, if player is not sidescroller
			if(!sidescroller)
				direction = (screenMovementForward * v) + (screenMovementRight * h);
			else
				direction = Vector3.right * h;
			moveDirection = transform.position + direction;
			
			if (GetComponent<Rigidbody>().velocity.y > 0) 
			{
				float DistanceFromGround = transform.position.y - GroundY;
				if (Camera.main.fieldOfView < (70f + DistanceFromGround * 2f)) 
				{
					Camera.main.fieldOfView += 0.1f;					
				}
			}
			else
			{
				GroundY = transform.position.y;
				if (Camera.main.fieldOfView > 70f) 
				{
					Camera.main.fieldOfView -= 0.1f;
				}
			}
			if (RippleEffect)
			{
				if (MoveDirection == 0)
				{
					RipplePriod = 1f;
				}
				else
				{
					RipplePriod = 0.1f;
				}
				if (IsOnWater)
				{
					if (CurrentRipplePriod == 0f)
					{
						CurrentRipplePriod = Time.time + RipplePriod;
					}
					if (Time.time - CurrentRipplePriod > RipplePriod)
					{

						Instantiate (RippleEffect,new Vector3(transform.position.x,WaterY,transform.position.z), Quaternion.Euler(Vector3.zero));
						CurrentRipplePriod += RipplePriod;
					}
					if (Time.time - CurrentRipplePriod > 0.1f)
					{
						Instantiate (BubbleBreathEffect,transform.position, Quaternion.Euler(Vector3.zero));
					}
				}
				else
				{
					CurrentRipplePriod = 0f;
				}
			}
		}
	}
	
	//apply correct player movement (fixedUpdate for physics calculations)
	void FixedUpdate() 
	{
		if (!GameObject.FindGameObjectWithTag ("LevelController").GetComponent<LevelController> ().Pause) 
		{
			//are we grounded
			grounded = IsGrounded ();
			TouchController.IsJumping = !grounded;
			if (!IsRolling && grounded)
			{
				TouchController.IsRolling = false;
			}
			//move, rotate, manage speed
			characterMotor.MoveTo (moveDirection, curAccel, 0.7f, true);
			if (rotateSpeed != 0 && direction.magnitude != 0)
				characterMotor.RotateToDirection (moveDirection , curRotateSpeed * 5, true);
			characterMotor.ManageSpeed (curDecel, maxSpeed + movingObjSpeed.magnitude, true);
			//set animation values
			if(animator)
			{
				animator.SetFloat("DistanceToTarget", characterMotor.DistanceToTarget);
				animator.SetBool("Grounded", grounded);
				animator.SetFloat("YVelocity", GetComponent<Rigidbody>().velocity.y);
			}
		}
	}
	
	//prevents rigidbody from sliding down slight slopes (read notes in characterMotor class for more info on friction)
	void OnCollisionStay(Collision other)
	{
		if (!GameObject.FindGameObjectWithTag ("LevelController").GetComponent<LevelController> ().Pause) 
		{
			if (other.collider.tag == "Pickup" && (IsRolling || !grounded))
			{
				dealDamage.Attack(other.collider.gameObject,1,0f,0f);
			}
			//only stop movement on slight slopes if we aren't being touched by anything else
			if (other.collider.tag != "Untagged" || grounded == false)
				return;
			//if no movement should be happening, stop player moving in Z/X axis
			if(direction.magnitude == 0 && slope < slopeLimit && GetComponent<Rigidbody>().velocity.magnitude < 2)
			{
				//it's usually not a good idea to alter a rigidbodies velocity every frame
				//but this is the cleanest way i could think of, and we have a lot of checks beforehand, so it shou
				GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		}
	}
	
	//returns whether we are on the ground or not
	//also: bouncing on enemies, keeping player on moving platforms and slope checking
	private bool IsGrounded() 
	{
		//get distance to ground, from centre of collider (where floorcheckers should be)
		float dist = GetComponent<Collider>().bounds.extents.y;
		//check whats at players feet, at each floorcheckers position
		foreach (Transform check in floorCheckers)
		{
			RaycastHit hit;
			if(Physics.Raycast(check.position, Vector3.up, out hit, dist + 0.05f))
			{
				if(!hit.transform.GetComponent<Collider>().isTrigger)
				{
						if (hit.transform.tag == "Pickup")
						{
							dealDamage.Attack(hit.transform.gameObject, 1, 0f, 0f);
						}
				}
			}
			if(Physics.Raycast(check.position, Vector3.down, out hit, dist + 0.05f))
			{
				if(!hit.transform.GetComponent<Collider>().isTrigger)
				{
					//slope control
					slope = Vector3.Angle (hit.normal, Vector3.up);
					//slide down slopes
					if(slope > slopeLimit && hit.transform.tag != "Pushable")
					{
						Vector3 slide = new Vector3(0f, -slideAmount, 0f);
						GetComponent<Rigidbody>().AddForce (slide, ForceMode.Force);
					}
					//enemy bouncing
					if (hit.transform.tag == "Enemy" && GetComponent<Rigidbody>().velocity.y != 0)
					{
						enemyAI = hit.transform.GetComponent<EnemyAI>();
						if (hit.collider.tag != "AttackTrigger")
						{
							enemyAI.BouncedOn();
							onEnemyBounce ++;
							dealDamage.Attack(hit.transform.gameObject, 1, 0f, 0f);
							if (GameObject.FindGameObjectWithTag("LevelController"))
							{
								GameObject.FindGameObjectWithTag("LevelController").GetComponent<LevelController>().LevelScore += hit.transform.gameObject.GetComponent<EnemyAI>().Score;
							}
						}
					}
					else if (hit.transform.tag == "Pickup" && GetComponent<Rigidbody>().velocity.y < 0)
					{
							dealDamage.Attack(hit.transform.gameObject, 1, 0f, 0f);
					}
					else
						onEnemyBounce = 0;
					//moving platforms
					if (hit.transform.tag == "MovingPlatform" || hit.transform.tag == "Pushable")
					{
						movingObjSpeed = hit.transform.GetComponent<Rigidbody>().velocity;
						movingObjSpeed.y = 0f;
						//9.5f is a magic number, if youre not moving properly on platforms, experiment with this number
						GetComponent<Rigidbody>().AddForce(movingObjSpeed * movingPlatformFriction * Time.fixedDeltaTime, ForceMode.VelocityChange);
					}
					else
					{
						movingObjSpeed = Vector3.zero;
					}
					//yes our feet are on something
					return true;
				}
			}
		}
		movingObjSpeed = Vector3.zero;
		//no none of the floorchecks hit anything, we must be in the air (or water)
		if (IsOnWater)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	//jumping
	private void JumpCalculations()
	{
		if (SimulateInputJump)
		{
			if (StartJumpTime == 0)
			{
				StartJumpTime = Time.time;
			}
			if (Time.time - StartJumpTime >= MaxJumpTime)
			{
				SimulateInputJump = false;
				StartJumpTime = 0;
				ExecuteJump = true;
			}
		}
		//keep how long we have been on the ground
		groundedCount = (grounded) ? groundedCount += Time.deltaTime : 0f;
		
		//play landing sound
		if(groundedCount < 0.25 && groundedCount != 0 && !GetComponent<AudioSource>().isPlaying && landSound && GetComponent<Rigidbody>().velocity.y < 1)
		{
			GetComponent<AudioSource>().volume = Mathf.Abs(GetComponent<Rigidbody>().velocity.y)/40;
			GetComponent<AudioSource>().clip = landSound;
			GetComponent<AudioSource>().Play ();
		}
		//if we press jump in the air, save the time
		if ((Input.GetButtonDown ("Jump") || ExecuteJump) && !grounded)
			airPressTime = Time.time;
		
		//if were on ground within slope limit
		if (grounded && slope < slopeLimit)
		{
			//and we press jump, or we pressed jump justt before hitting the ground
			if ((Input.GetButtonDown ("Jump") || ExecuteJump) || airPressTime + jumpLeniancy > Time.time)
			{	
				//increment our jump type if we haven't been on the ground for long
				onJump = (groundedCount < jumpDelay) ? Mathf.Min(2, onJump + 1) : 0;
				//execute the correct jump (like in mario64, jumping 3 times quickly will do higher jumps)
				if (JumpHeight > 0)
				{
					jumpForce.Set(0,JumpHeight,0);
				}
				if (jumpForce.y > MaxJump)
				{
					jumpForce.Set(0,MaxJump,0);
				}
				else if (jumpForce.y < MinJump)
				{
					jumpForce.Set(0,MinJump,0);
				}
				if (onJump == 0)
						Jump (jumpForce);
				else if (onJump == 1)
						Jump (secondJumpForce);
				else if (onJump == 2)
						Jump (thirdJumpForce);

			}
		}
	}
	
	//push player at jump force
	public void Jump(Vector3 jumpVelocity)
	{
		if(jumpSound)
		{
			GetComponent<AudioSource>().volume = 1;
			GetComponent<AudioSource>().clip = jumpSound;
			GetComponent<AudioSource>().Play ();
		}
		GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0f, GetComponent<Rigidbody>().velocity.z);
		GetComponent<Rigidbody>().AddRelativeForce (jumpVelocity, ForceMode.Impulse);
		airPressTime = 0f;
		JumpHeight = 0;
		ExecuteJump = false;
		TouchController.LastTouchPosition = Vector2.zero;
	}

	private void Roll(Vector3 rollVelocity , int Side)
	{
		if (IsOnWater)
		{
			RollSide = 0;
			IsRolling = false;
			return;
		}
		if (SimulateInputRolling || SimulateInputRollingRight || SimulateInputRollingLeft)
		{
			TouchController.IsRolling = true;
			TouchController.LastTouchPosition = Vector2.zero;
			RollingStartTime = Time.time;
			SimulateInputRolling = false;
			SimulateInputRollingRight = false;
			SimulateInputRollingLeft = false;
		}
		if (Time.time - RollingStartTime > RollingTime)
		{
			RollSide = 0;
			IsRolling = false;
		}
		else
		{
			if (Side == 0)
			{
				if (MovingRight)
				{
					GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
					GetComponent<Rigidbody>().AddForce (rollVelocity, ForceMode.Impulse);
				}
				else
				{
					GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
					rollVelocity.Set(rollVelocity.x - (rollVelocity.x * 2),rollVelocity.y, rollVelocity.z );
					GetComponent<Rigidbody>().AddForce (rollVelocity, ForceMode.Impulse);
				}
			}
			else if (Side == 1)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				GetComponent<Rigidbody>().AddForce (rollVelocity, ForceMode.Impulse);
			}
			else if (Side == 2)
			{
				GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
				rollVelocity.Set(rollVelocity.x - (rollVelocity.x * 2),rollVelocity.y, rollVelocity.z );
				GetComponent<Rigidbody>().AddForce (rollVelocity, ForceMode.Impulse);
			}
		}
	}
	public void RollAttack(GameObject Enemy)
	{
		dealDamage.Attack(Enemy, 1, 0f, 0f);
	}
}