using UnityEngine;
using System.Collections;

public class PrizeController : MonoBehaviour 
{
	public bool Live = false;
	public bool Magnet = false;
	public float MagnetTime = 30f;
	public GameObject MagnetObject;
	public bool Rolling = false;
	public float RollingTime = 30f;
	public GameObject RollingObject;
	public bool Guard = false;
	public float GuardTime = 30f;
	public GameObject GuardObject;
	public bool Gem = false;
	public bool Health = false;
	public int Coin = 0;
	private float MagnetGivingTime;
	private float GuardGivingTime;
	private float RollingGivingTime;
	private bool ShowMagnet = false;
	private bool ShowGuard = false;
	private bool ShowRolling = false;
	private HUD PlayerController;

	void Awake()
	{
		GameObject MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		PlayerController = MainCamera.GetComponent<HUD> ();
	}

	void Update()
	{
		if (Magnet)
		{
			MagnetGivingTime = Time.time;
			ShowMagnet = true;
			Magnet = false;
			MagnetObject.SetActive(true);
		}
		if (Guard) 
		{
			GuardGivingTime = Time.time;
			ShowGuard = true;
			Guard = false;
			GetComponent<PlayerMove>().AntiAttack = true;
			GuardObject.SetActive(true);
		}
		if (Rolling) 
		{
			RollingGivingTime = Time.time;
			ShowRolling = true;
			Rolling = false;
			RollingObject.SetActive (true);
			PlayerController.CanRolling = true;
		}
		if (Health)
		{
			GetComponent<Health>().currentHealth = GetComponent<Health>().maxHealth;
		}

		if (ShowMagnet)
		{
			if(Time.time - MagnetGivingTime > MagnetTime)
			{
				MagnetObject.SetActive(false);
				ShowMagnet = false;
			}
		}
		if (ShowGuard)
		{			
			if(Time.time - GuardGivingTime > GuardTime)
			{
				GuardObject.SetActive(false);
				GetComponent<PlayerMove>().AntiAttack = false;
				ShowGuard = false;
			}
		}
		if (ShowRolling)
		{			
			if(Time.time - RollingGivingTime > RollingTime)
			{
				PlayerController.CanRolling = false;
				RollingObject.SetActive (false);
				ShowRolling = false;
			}
		}
	}

	void OnGUI()
	{
		if (ShowMagnet)
		{
			Vector2 TexturePosition = Camera.main.WorldToScreenPoint (transform.position);
			TexturePosition.y += Screen.height / 7;
			TexturePosition.x -= Screen.width / 40;
			Rect MagnetRect = new Rect (TexturePosition.x,TexturePosition.y,Screen.width / 20,Screen.width / 20);
			GUI.Box (MagnetRect,"Magnet");
		}
	}

}
