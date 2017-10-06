using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelScript : MonoBehaviour {
	
	void Start() { Time.timeScale = 1; }

	public void LoadScene(string level)
	{
		GameManagerScript.levelName = level;
		SceneManager.LoadScene(1);
	}
}
