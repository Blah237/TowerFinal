using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelSelect : MonoBehaviour {
	
	void Start() { 
		Time.timeScale = 1; 
	}

	public void LoadScene(int level)
	{
		if (LoggingManager.instance != null)
		{
			if (LoggingManager.instance.GetLevelStarted() == true)
			{
				// If true this is an exit to level select, if false it's level select after a win
				LoggingManager.instance.RecordEvent(LoggingManager.EventCodes.EXIT_TO_LEVEL_SELECT, "Exit to level select");
				LoggingManager.instance.RecordLevelEnd();
			}
		}
		AudioManagerScript.instance.stopEffects();
		WinScript.playerWin = false;
		SceneManager.LoadScene(0);
	}

	public static void LoadSceneEscFromPause()
	{
		if (LoggingManager.instance.GetLevelStarted() == true) { // If true this is an exit to level select, if false it's level select after a win
			LoggingManager.instance.RecordEvent (LoggingManager.EventCodes.EXIT_TO_LEVEL_SELECT, "Exit to level select");
			LoggingManager.instance.RecordLevelEnd ();
		}
		AudioManagerScript.instance.stopEffects();
		WinScript.playerWin = false;
		SceneManager.LoadScene(0);
	}
}