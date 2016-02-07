using UnityEngine;
using System.Collections;

public class Kangoro : MonoBehaviour {
	public float Velocity;
	public float High;
	public float StopTime;
	public GameObject FootdownDust;

	private float EarthY;
	private Vector3 Target;
	private bool IsJumping = false;
	
	void Awake () 
	{
		EarthY = transform.position.y;
		High += EarthY;
		Velocity = Velocity / 5;
		Target = new Vector3(transform.position.x,High,transform.position.z);
	}

	void Update () 
	{
		if (!GameObject.FindGameObjectWithTag ("LevelController").GetComponent<LevelController> ().Pause) 
		{
			if (transform.position.y <= EarthY && !IsJumping) 
			{
				StartCoroutine(Jump());
			}
			else if (transform.position.y >= High)
			{
				IsJumping = false;
				Target = new Vector3(transform.position.x,EarthY,transform.position.z);
			}
			MoveToDirection();
		}
	}

	void MoveToDirection()
	{
		transform.position = Vector3.MoveTowards (transform.position, Target, Velocity * Time.deltaTime);
	}

	IEnumerator Jump()
	{
		IsJumping = true;
		if (FootdownDust)
		{
			Instantiate (FootdownDust,transform.position,Quaternion.Euler(Vector3.zero));
		}
		yield return new WaitForSeconds(StopTime);
		Target = new Vector3(transform.position.x,High,transform.position.z);
	}
}
