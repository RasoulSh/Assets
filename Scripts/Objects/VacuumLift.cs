using UnityEngine;
using System.Collections;

public class VacuumLift : MonoBehaviour 
{
	public float lift;

	void OnTriggerStay(Collider other)
	{
		if (!GameObject.FindGameObjectWithTag ("LevelController").GetComponent<LevelController> ().Pause) 
		{
			if(other.GetComponent<Rigidbody>())
				other.GetComponent<Rigidbody>().AddForce(Vector3.up * lift, ForceMode.Force);
		}
	}
}
