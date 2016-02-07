using UnityEngine;
using System.Collections;

public class DeathTrigger : MonoBehaviour 
{
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.GetComponent<Health>())
		{
			other.gameObject.GetComponent<Health>().currentHealth = 0;
		}
	}
}
