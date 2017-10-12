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
		LoggingManager.instance.RecordEvent (LoggingManager.EventCodes.EXIT_TO_LEVEL_SELECT);
		LoggingManager.instance.RecordLevelEnd ();
		SceneManager.LoadScene(0);
	}
}