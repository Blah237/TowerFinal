using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour {

    private bool isready = true; 

    // Use this for initialization
    void Start () {
		
	}
	
	public void LoadScene()
	{
        if (!isready) {
            return;
        }
        isready = false;
		AudioManagerScript.instance.stopEffects ();
		WinScript.playerWin = false;
		if (LoggingManager.instance.GetLevelStarted ()) { // If this isn't true, we're replaying!
			LoggingManager.instance.RecordEvent (LoggingManager.EventCodes.RESTART, "Restart");
			LoggingManager.instance.RecordLevelEnd ();
		}
		SceneManager.LoadScene(1);
	}

}
