using UnityEngine;
using System.Collections;
//using UnityEngine.SceneManagement;

public class GiftController : MonoBehaviour 
{

		void OnGUI()
		{
				if (GUI.RepeatButton (new Rect (0, 0, 100f, 50f), "Back")) 
				{
						Application.LoadLevel("MainMenu");
				}
		}
}
