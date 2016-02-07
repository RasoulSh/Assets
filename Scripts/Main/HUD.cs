using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour 
{
		public GUISkin HUDSkin;
		public Texture2D LeftButton;
		public Texture2D RightButton;
		//public Texture2D JumpButton;

		public GameObject Player;
		private PlayerMove PlayerMoveComponent;
		public bool CanRolling = false;
		public bool PushJumpButton = false;
		private Rect leftIconRect;
		private Rect rightIconRect;
		//private Rect jumpIconRect;
		public bool IsJumping = false;
		public bool IsRolling = false;
		private float MinimumVerticalDrag;
		private float MinimumHorizentalDrag;

		public Vector2 LastTouchPosition;

		void Awake()
		{
			MinimumVerticalDrag = Screen.height / 15;
			MinimumHorizentalDrag = Screen.width / 10;
		}

		void Start()
		{
			PlayerMoveComponent = Player.GetComponent<PlayerMove> ();

			float HUDSize = Screen.height / 3.5f;
			float x = 0;
			float y = Screen.height - HUDSize;
			
			leftIconRect = new Rect(x,y,HUDSize,HUDSize);
			x += HUDSize + 1f;
			rightIconRect = new Rect (x,y,HUDSize,HUDSize);
		}

		void Update()
		{	
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				if (CanRolling) PlayerMoveComponent.SimulateInputRolling = true;
			}
			if (Input.touchCount < 1) 
			{
				PlayerMoveComponent.SimulateInputRight = false;
				PlayerMoveComponent.SimulateInputLeft = false;
				//PlayerMoveComponent.SimulateInputJump = false;
				PushJumpButton = false;
			}
			foreach(Touch t in Input.touches)
			{
				bool IsMovingButton = false;
				Vector2 vec = t.position;
				vec.y = Screen.height - vec.y;
				Rect leftrect = new Rect (0, Screen.height / 2,leftIconRect.width,Screen.height / 2);
				if(leftrect.Contains(vec))
				{
					PlayerMoveComponent.SimulateInputRight = false;
					PlayerMoveComponent.SimulateInputLeft = true;
					IsMovingButton = true;
				}
				else
				{
					PlayerMoveComponent.SimulateInputLeft = false;
				}
				Rect rightrect = new Rect (leftIconRect.width,Screen.height / 2, rightIconRect.width * 1.3f, Screen.height / 2);
				if(rightrect.Contains(vec))
				{
					PlayerMoveComponent.SimulateInputLeft = false;
					PlayerMoveComponent.SimulateInputRight = true;
					IsMovingButton = true;
				}
				else
				{
					PlayerMoveComponent.SimulateInputRight = false;
				}
				
				if (t.phase == TouchPhase.Began)
				{
					LastTouchPosition = t.position;
				}
				if (!IsMovingButton)
				{
					if (!IsJumping)
					{
						if (t.phase == TouchPhase.Moved && LastTouchPosition != Vector2.zero)
						{
							if (t.position.y > LastTouchPosition.y)
							{
								PlayerMoveComponent.SimulateInputJump = true;
								PlayerMoveComponent.JumpHeight = ((t.position.y - LastTouchPosition.y) / (Screen.height / 85));
								PushJumpButton = true;
							}
							else
							{
								PushJumpButton = false;
							}
						}
						else
						{
							PushJumpButton = false;
						}	
					}
					if (!IsRolling && !IsJumping)
					{
						if (t.phase == TouchPhase.Moved && LastTouchPosition != Vector2.zero)
						{
							if (t.position.y < LastTouchPosition.y && (LastTouchPosition.y - t.position.y) > MinimumVerticalDrag)
							{
								if (CanRolling) PlayerMoveComponent.SimulateInputRolling = true;
							}
						}	
					}
					if (!IsRolling && IsJumping)
					{
						if (t.phase == TouchPhase.Moved && LastTouchPosition != Vector2.zero)
						{
							if (t.position.x > LastTouchPosition.x  && (t.position.x - LastTouchPosition.x) > MinimumHorizentalDrag)
							{
								if (CanRolling) PlayerMoveComponent.SimulateInputRollingRight = true;
							}
							else if (t.position.x < LastTouchPosition.x  && (LastTouchPosition.x - t.position.x) > MinimumHorizentalDrag)
							{
								if (CanRolling) PlayerMoveComponent.SimulateInputRollingLeft = true;
							}
						}	
					}	
				}
			}
		}

		void OnGUI()
		{
			GUI.skin = HUDSkin;
			GUI.RepeatButton (leftIconRect, LeftButton);
			GUI.RepeatButton (rightIconRect, RightButton);
		}

}
