using UnityEngine;
using System.Collections;

public class SoulEffect : MonoBehaviour {
	
	void Start () 
	{
		StartCoroutine (SoulExit());
	}

	IEnumerator SoulExit()
	{
		GetComponent<ParticleSystem> ().Pause ();
		yield return new WaitForSeconds (1);
		GetComponent<ParticleSystem> ().Play ();
	}
}
