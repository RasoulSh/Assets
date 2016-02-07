using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//class to handle buoyancy of objects whilst in a "Water" tagged object
[RequireComponent(typeof(BoxCollider))]
public class Water : MonoBehaviour 
{
	private bool Enable = false;
	public AudioClip splashSound;						//played when objects enter water
	public GameObject SplashEffect;
	public Vector3 force = new Vector3(0, 16.5f, 0);	//pushForce of the water. This is a vector3 so you can have force in any direction, for example a current or river
	public bool effectPlayerDrag;						//should the players rigidbody be effected by the drag/angular drag values of the water?
	public float resistance = 0.4f;						//the drag applied to rigidbodies in the water (but not player)
	public float angularResistance = 0.2f;				//the angular drag applied to rigidbodies in the water (but not player)
	
	private Dictionary<GameObject, float> dragStore = new Dictionary<GameObject, float>();
	private Dictionary<GameObject, float> angularStore = new Dictionary<GameObject, float>();
	
	void Awake()
	{
		if(tag != "Water")
		{
			tag = "Water";
			Debug.LogWarning("'Water' script attached to an object not tagged 'Water', it been assigned the tag 'Water'", transform);
		}
		GetComponent<Collider>().isTrigger = true;
	}
	
	//apply buoyancy
	void OnTriggerStay(Collider other)
	{
		if (Enable)
		{
			//get surface position
			float surface = transform.position.y + GetComponent<Collider>().bounds.extents.y;
			if(other.GetComponent<Rigidbody>())
			{
				//get object depth
				float depth = surface - other.transform.position.y;
				//if below surface, push object
				if(depth > 2f)
					other.GetComponent<Rigidbody>().AddForce(force, ForceMode.Force);
				//if we are near the surface, add less force, this prevents objects from "jittering" up and down on the surface
				else
					other.GetComponent<Rigidbody>().AddForce (force * (depth * 2), ForceMode.Force);
			}
		}
	}
	
	//sets drag on objects entering water
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<Fish>())
		{
			other.gameObject.GetComponent<Fish>().UnFreezeYPosition();
			return;
		}
		if (Enable)
		{
			if (other.tag == "Player")
			{
				other.gameObject.GetComponent<PlayerMove>().WaterY = other.gameObject.transform.position.y;
				other.gameObject.GetComponent<PlayerMove>().IsOnWater = true;
			}
			//rigidbody entered water?
			if(other.GetComponent<Rigidbody>())
			{
				other.GetComponent<Rigidbody> ().mass = 0.6f;
				if(splashSound)
				{
					float volume = other.GetComponent<Rigidbody>().velocity.magnitude/5;
					AudioSource.PlayClipAtPoint(splashSound, other.transform.position, volume);
				}
				Rigidbody r = other.GetComponent<Rigidbody>();
				//stop if we arent effecting player
				if (r.tag == "Player" && !effectPlayerDrag)
					return;
				
				//store objects default drag values
				dragStore.Add (r.gameObject, r.drag);
				angularStore.Add(r.gameObject, r.angularDrag);
				
				//apply new drag values to object
				r.drag = resistance;
				r.angularDrag = angularResistance;
			}
			else
			{
				if(splashSound)
					AudioSource.PlayClipAtPoint(splashSound, other.transform.position);
				if (SplashEffect)
					Instantiate(SplashEffect,other.transform.position,Quaternion.Euler(Vector3.zero));
			}
		}
	}
	
	//reset drag on objects leaving water
	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.GetComponent<Fish>())
		{
			other.gameObject.GetComponent<Fish>().FreezeYPosition();
			return;
		}
		if (Enable)
		{
			if (other.tag == "Player")
			{
				other.gameObject.GetComponent<PlayerMove>().IsOnWater = false;
			}
			if (other.GetComponent<Rigidbody> ())
			{
				other.GetComponent<Rigidbody> ().mass = 1f;
			}
			//rigidbody entered water?
			if(other.GetComponent<Rigidbody>())
			{
				Rigidbody r = other.GetComponent<Rigidbody>();
				//stop if we arent effecting player
				if(r.tag == "Player" && !effectPlayerDrag)
					return;
				
				//see if we've stored this objects default drag values
				if (dragStore.ContainsKey(r.gameObject) && angularStore.ContainsKey(r.gameObject))
				{
					//restore values
					r.drag = dragStore[r.gameObject];
					r.angularDrag = angularStore[r.gameObject];
					//remove stored values for this object
					dragStore.Remove(r.gameObject);
					angularStore.Remove (r.gameObject);
				}
				else
				{
					//restore default values incase we cant find it in list (for whatever reason)
					r.drag = 0f;
					r.angularDrag = 0.05f;
					print ("Object left water: couldn't get drag values, restored to defaults");
				}
			}
			if(splashSound)
				AudioSource.PlayClipAtPoint(splashSound, other.transform.position);
			if (SplashEffect)
				Instantiate(SplashEffect,other.transform.position,Quaternion.Euler(Vector3.zero));
		}
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