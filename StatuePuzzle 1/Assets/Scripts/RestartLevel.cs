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
		AudioManagerScript.instance.stopEffects ();
		WinScript.playerWin = false;
		if (LoggingManager.instance.GetLevelStarted ()) { // If this isn't true, we're replaying!
			LoggingManager.instance.RecordEvent (LoggingManager.EventCodes.RESTART, "Restart");
			LoggingManager.instance.RecordLevelEnd ();
		}
		SceneManager.LoadScene(1);
	}

}
