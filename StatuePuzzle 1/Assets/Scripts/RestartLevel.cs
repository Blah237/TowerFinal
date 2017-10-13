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
		LoggingManager.instance.RecordEvent (LoggingManager.EventCodes.RESTART);
		SceneManager.LoadScene(1);
	}
}
