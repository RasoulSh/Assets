using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour 
{
	public GameObject FireEffect;

	void OnBecameVisible()
	{
		FireEffect.SetActive (true);
	}
	void OnBecameInvisible()
	{
		FireEffect.SetActive (false);
	}
}
