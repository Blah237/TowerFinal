using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public void LoadScene()
	{
		WinScript.playerWin = false;
		LoggingManager.instance.RecordEvent (LoggingManager.EventCodes.RESTART, "Restart");
		LoggingManager.instance.RecordLevelEnd ();
		SceneManager.LoadScene(1);
	}
}
