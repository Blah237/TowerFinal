using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelSelect : MonoBehaviour {
	
	void Start() { Time.timeScale = 1; }

	public void LoadScene(int level)
	{
		SceneManager.LoadScene(0);
	}
}