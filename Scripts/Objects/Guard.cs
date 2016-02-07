using UnityEngine;
using System.Collections;

public class Guard : MonoBehaviour 
{
	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Enemy")
		{
			col.gameObject.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,13f,0));
		}
	}
}
